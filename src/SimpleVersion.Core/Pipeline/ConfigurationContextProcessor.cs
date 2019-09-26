// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using LibGit2Sharp;
using SimpleVersion.Abstractions.Exceptions;
using SimpleVersion.Abstractions.Pipeline;
using SimpleVersion.Comparers;
using SimpleVersion.Model;
using SimpleVersion.Serialization;

namespace SimpleVersion.Pipeline
{
    /// <summary>
    /// Resolves the configuration for the version calculation.
    /// </summary>
    public class ConfigurationContextProcessor : IVersionContextProcessor
    {
        private static readonly ConfigurationVersionLabelComparer _comparer = new ConfigurationVersionLabelComparer();

        /// <inheritdoc/>
        public void Apply(IVersionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (!(context is VersionContext repoContext))
                throw new InvalidCastException(Resources.Exception_CouldNotConvertContextType(typeof(VersionContext)));

            var tip = repoContext.Repository.Head?.Tip;
            if (tip == null)
                throw new GitException(Resources.Exception_CouldNotFindBranchTip);

            var config = GetConfiguration(tip, repoContext)
                        ?? throw new InvalidOperationException(Resources.Exception_CouldNotReadSettingsFile(Constants.VersionFileName));

            context.Configuration = config;

            PopulateHeight(repoContext);
        }

        private void PopulateHeight(VersionContext context)
        {
            // Get the state of this tree to compare for diffs
            var tipTree = context.Repository.Head.Tip.Tree;

            // Initialize count - The current commit counts, include offset
            var height = 1 + context.Configuration.OffSet;

            // skip the first commit as that is our baseline
            var commits = GetReachableCommits(context.Repository).Skip(1).GetEnumerator();

            while (commits.MoveNext())
            {
                // Get the current tree
                var next = commits.Current.Tree;

                // Perform a diff
                var diff = context.Repository.Diff.Compare<TreeChanges>(next, tipTree);

                // If a change to the file is found, stop counting
                if (HasVersionChange(diff, commits.Current, context))
                    break;

                // Increment height
                height++;
            }

            context.Result.Height = height;
        }

        private static bool HasVersionChange(
            TreeChanges diff,
            Commit commit,
            VersionContext context)
        {
            if (diff.Any(d => d.Path == Constants.VersionFileName))
            {
                var commitConfig = GetConfiguration(commit, context);
                return commitConfig != null && !_comparer.Equals(context.Configuration, commitConfig);
            }

            return false;
        }

        private static IEnumerable<Commit> GetReachableCommits(IRepository repo)
        {
            var filter = new CommitFilter
            {
                FirstParentOnly = true,
                IncludeReachableFrom = repo.Head,
                SortBy = CommitSortStrategies.Reverse
            };

            return repo.Commits.QueryBy(filter).Reverse();
        }

        private static Settings? GetConfiguration(Commit commit, VersionContext context)
        {
            var gitObj = commit?.Tree[Constants.VersionFileName]?.Target;
            if (gitObj == null)
                return null;

            var blob = gitObj as Blob;
            if (blob == null)
                return null;

            var config = Read(blob.GetContentText());
            if (config != null)
                ApplyConfigOverrides(config, context);
            return config;
        }

        private static void ApplyConfigOverrides(Settings config, VersionContext context)
        {
            if (config == null)
                return;

            var match = config
                .Branches
                .Overrides
                .FirstOrDefault(x => Regex.IsMatch(context.Result.CanonicalBranchName, x.Match, RegexOptions.IgnoreCase));

            if (match != null)
            {
                var label = ApplyParts(match.Label ?? config.Label, match.PrefixLabel, match.PostfixLabel, match.InsertLabel);
                config.Label.Clear();
                config.Label.AddRange(label);

                var meta = ApplyParts(match.Metadata ?? config.Metadata, match.PrefixMetadata, match.PostfixMetadata, match.InsertMetadata);
                config.Metadata.Clear();
                config.Metadata.AddRange(meta);
            }
        }

        private static List<string> ApplyParts(List<string>? baseList, List<string>? pre, List<string>? post, IDictionary<int, string>? inserts)
        {
            var result = new List<string>();
            if (baseList != null)
                result.AddRange(baseList);

            if (inserts != null)
            {
                foreach (var entry in inserts)
                {
                    result.Insert(entry.Key, entry.Value);
                }
            }

            if (pre != null)
                result.InsertRange(0, pre);

            if (post != null)
                result.AddRange(post);

            return result;
        }

        private static Settings? Read(string rawConfiguration)
        {
            Settings? result = null;
            try
            {
                result = Serializer.Deserialize<Settings>(rawConfiguration);
            }
            finally
            {
                // TODO handle logger of invalid parsing
                Debug.WriteLine("Settings are in an incorrect format");
            }

            return result;
        }
    }
}
