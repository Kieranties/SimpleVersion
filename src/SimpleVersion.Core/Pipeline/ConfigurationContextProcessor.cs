// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LibGit2Sharp;
using Newtonsoft.Json;
using SimpleVersion.Abstractions.Pipeline;
using SimpleVersion.Comparers;
using SVM = SimpleVersion.Model;

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
                throw new InvalidCastException(Resources.CouldNotConvertContextType.FormatWith(typeof(VersionContext)));

            var config = GetConfiguration(repoContext.Repository.Head?.Tip, repoContext)
                        ?? throw new InvalidOperationException(Resources.CouldNotReadSimpleVersionFile.FormatWith(Constants.VersionFileName));

            context.Configuration = config;

            PopulateHeight(repoContext);
        }

        private void PopulateHeight(VersionContext context)
        {
            // Get the state of this tree to compare for diffs
            var tipTree = context.Repository.Head.Tip.Tree;

            // Initialise count - The current commit counts, include offset
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

        private static SVM.Configuration GetConfiguration(Commit commit, VersionContext context)
        {
            var gitObj = commit?.Tree[Constants.VersionFileName]?.Target;
            if (gitObj == null)
                return null;

            var config = Read((gitObj as Blob).GetContentText());
            ApplyConfigOverrides(config, context);
            return config;
        }

        private static void ApplyConfigOverrides(SVM.Configuration config, VersionContext context)
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

        private static List<string> ApplyParts(List<string> baseList, List<string> pre, List<string> post, IDictionary<int, string> inserts)
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

        private static SVM.Configuration Read(string rawConfiguration)
        {
            try
            {
                return JsonConvert.DeserializeObject<SVM.Configuration>(rawConfiguration);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch
            {
                // TODO handle logger of invalid parsing
                return null;
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }
    }
}
