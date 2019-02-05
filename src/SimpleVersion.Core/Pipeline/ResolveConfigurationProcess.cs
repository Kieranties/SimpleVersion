using LibGit2Sharp;
using SVM = SimpleVersion.Model;
using Newtonsoft.Json;
using SimpleVersion.Comparers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleVersion.Pipeline
{
    public class ResolveConfigurationProcess : ICalculatorProcess
    {
        private readonly ConfigurationVersionLabelComparer _comparer = new ConfigurationVersionLabelComparer();

        public void Apply(VersionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (string.IsNullOrWhiteSpace(context.RepositoryPath))
                throw new ArgumentException($"{nameof(context.RepositoryPath)} must be a directory");

            var repo = new Repository(context.RepositoryPath);
            var config = GetConfiguration(repo.Head?.Tip)
                ?? throw new InvalidOperationException($"No commits found for '{Constants.VersionFileName}'");

            context.Configuration = config;

            PopulateResult(context, repo);
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
                if (HasVersionChange(diff, commits.Current, context.Configuration))
                    break;

                // Increment height
                height++;
            }

            context.Result.BranchName = repo.Head.FriendlyName;
            context.Result.Sha = repo.Head.Tip.Sha;
            context.Result.Height = height;
        }

        private bool HasVersionChange(TreeChanges diff, Commit commit, SVM.Configuration config)
        {
            if (diff.Any(d => d.Path == Constants.VersionFileName))
            {
                var commitConfig = GetConfiguration(commit);
                return !_comparer.Equals(config, commitConfig);
            }

            return false;
        }

        private IEnumerable<Commit> GetReachableCommits(Repository repo)
        {
            var filter = new CommitFilter
            {
                FirstParentOnly = true,
                IncludeReachableFrom = repo.Head,
                SortBy = CommitSortStrategies.Reverse
            };

            return repo.Commits.QueryBy(filter).Reverse();
        }

        private SVM.Configuration GetConfiguration(Commit commit)
        {
            var gitObj = commit?.Tree[Constants.VersionFileName]?.Target;
            if (gitObj == null)
                return null;

            return Read((gitObj as Blob).GetContentText());
        }

        private SVM.Configuration Read(string rawConfiguration) => JsonConvert.DeserializeObject<SVM.Configuration>(rawConfiguration);
    }
}
