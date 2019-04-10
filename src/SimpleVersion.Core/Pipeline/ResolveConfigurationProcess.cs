// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LibGit2Sharp;
using Newtonsoft.Json;
using SimpleVersion.Comparers;
using SVM = SimpleVersion.Model;

namespace SimpleVersion.Pipeline
{
    public class ResolveConfigurationProcess : ICalculatorProcess
    {
        private static readonly ConfigurationVersionLabelComparer _comparer = new ConfigurationVersionLabelComparer();

        public void Apply(VersionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (string.IsNullOrWhiteSpace(context.RepositoryPath))
                throw new ArgumentException($"{nameof(context.RepositoryPath)} must be a directory");

            using (var repo = new Repository(context.RepositoryPath))
            {
                if (string.IsNullOrWhiteSpace(context.Result.CanonicalBranchName))
                    context.Result.CanonicalBranchName = repo.Head.CanonicalName;

                if (string.IsNullOrWhiteSpace(context.Result.BranchName))
                    context.Result.BranchName = repo.Head.FriendlyName;

                var config = GetConfiguration(repo.Head?.Tip, context)
                    ?? throw new InvalidOperationException($"Could not read '{Constants.VersionFileName}', has it been committed?");

                context.Configuration = config;

                PopulateResult(context, repo);
            }
        }

        private void PopulateResult(VersionContext context, Repository repo)
        {
            // Get the state of this tree to compare for diffs
            var tipTree = repo.Head.Tip.Tree;

            // Initialise count - The current commit counts, include offset
            var height = 1 + context.Configuration.OffSet;

            // skip the first commit as that is our baseline
            var commits = GetReachableCommits(repo).Skip(1).GetEnumerator();

            while (commits.MoveNext())
            {
                // Get the current tree
                var next = commits.Current.Tree;

                // Perform a diff
                var diff = repo.Diff.Compare<TreeChanges>(next, tipTree);

                // If a change to the file is found, stop counting
                if (HasVersionChange(diff, commits.Current, context))
                    break;

                // Increment height
                height++;
            }

            context.Result.Sha = repo.Head.Tip.Sha;
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

        private static IEnumerable<Commit> GetReachableCommits(Repository repo)
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

            var firstMatch = config.Branches
                .Overrides.FirstOrDefault(x => Regex.IsMatch(context.Result.CanonicalBranchName, x.Match, RegexOptions.IgnoreCase));

            if (firstMatch != null)
            {
                if (firstMatch.Label != null)
                {
                    config.Label.Clear();
                    config.Label.AddRange(firstMatch.Label);
                }

                if (firstMatch.MetaData != null)
                {
                    config.MetaData.Clear();
                    config.MetaData.AddRange(firstMatch.MetaData);
                }
            }
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
