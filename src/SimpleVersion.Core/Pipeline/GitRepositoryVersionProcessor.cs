// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using LibGit2Sharp;
using SimpleVersion.Comparers;
using SimpleVersion.Configuration;
using SimpleVersion.Exceptions;
using SimpleVersion.Pipeline;

namespace SimpleVersion
{
    /// <summary>
    /// Process a git repository and applies to <see cref="VersionContext"/>.
    /// </summary>
    public class GitRepositoryVersionProcessor : IVersionProcessor
    {
        private const string _noBranchCheckedOut = "(no branch)";
        private static readonly VersionConfigurationLabelComparer _comparer = new VersionConfigurationLabelComparer();

        private readonly ISerializer _serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="GitRepositoryVersionProcessor"/> class.
        /// </summary>
        /// <param name="serializer">The serializer for reading documents.</param>
        public GitRepositoryVersionProcessor(ISerializer serializer)
        {
            _serializer = Assert.ArgumentNotNull(serializer, nameof(serializer));
        }

        /// <inheritdoc/>
        public void Process(IVersionContext context)
        {
            Assert.ArgumentNotNull(context, nameof(context));

            // Fetch repository configuration
            using var repo = GetRepository(context.WorkingDirectory);
            var repoConfig = GetRespositoryConfiguration(repo);

            // Set branches
            var canonicalBranchName = context.Environment?.CanonicalBranchName;

            if (string.IsNullOrWhiteSpace(canonicalBranchName))
            {
                canonicalBranchName = repo.Head.CanonicalName;
            }

            context.Result.BranchName = context.Environment?.BranchName!;
            if (string.IsNullOrWhiteSpace(context.Result.BranchName))
            {
                context.Result.BranchName = repo.Head.FriendlyName;
            }

            // Compose base result data
            context.Result.CanonicalBranchName = canonicalBranchName;
            context.Result.Sha = repo.Head.Tip.Sha;

            // Value returned from repository has trailing slash so need to get parent twice.
            context.Result.RepositoryPath = Directory.GetParent(repo.Info.Path).Parent.FullName;
            context.Result.IsRelease = repoConfig.Branches.Release.Any(x => Regex.IsMatch(canonicalBranchName, x));

            // Build context for further processing
            context.Configuration = GetBranchConfiguration(repoConfig, canonicalBranchName);

            // Update height based on context
            SetHeight(context, repo);
        }

        private static IRepository GetRepository(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException(Resources.Exception_PathMustBeProvided, nameof(path));
            }

            var resolvedPath = Repository.Discover(path);

            if (string.IsNullOrWhiteSpace(resolvedPath))
            {
                throw new DirectoryNotFoundException(Resources.Exception_CouldNotFindGitRepository(path));
            }

            return new Repository(resolvedPath);
        }

        private static VersionConfiguration GetBranchConfiguration(RepositoryConfiguration config, string branchName)
        {
            if (string.IsNullOrWhiteSpace(branchName) || branchName == _noBranchCheckedOut)
            {
                throw new GitException(Resources.Exception_CouldNotIdentifyBranchName);
            }

            var match = config
                .Branches
                .Overrides
                .FirstOrDefault(x => Regex.IsMatch(branchName, x.Match, RegexOptions.IgnoreCase));

            if (match != null)
            {
                var label = ApplyParts(
                    match.Label ?? config.Label,
                    match.PrefixLabel,
                    match.PostfixLabel,
                    match.InsertLabel);

                var meta = ApplyParts(
                    match.Metadata ?? config.Metadata,
                    match.PrefixMetadata,
                    match.PostfixMetadata,
                    match.InsertMetadata);

                return new VersionConfiguration
                {
                    Version = config.Version,
                    OffSet = config.OffSet,
                    Label = label,
                    Metadata = meta
                };
            }

            return config;
        }

        private static List<string> ApplyParts(List<string>? baseList, List<string>? pre, List<string>? post, IDictionary<int, string>? inserts)
        {
            var result = new List<string>();
            if (baseList != null)
            {
                result.AddRange(baseList);
            }

            if (inserts != null)
            {
                foreach (var entry in inserts)
                {
                    result.Insert(entry.Key, entry.Value);
                }
            }

            if (pre != null)
            {
                result.InsertRange(0, pre);
            }

            if (post != null)
            {
                result.AddRange(post);
            }

            return result;
        }

        private static IEnumerable<Commit> GetReachableCommits(IRepository repo)
        {
            var filter = new CommitFilter
            {
                FirstParentOnly = true,
                IncludeReachableFrom = repo.Head,
                SortBy = CommitSortStrategies.Topological
            };

            return repo.Commits.QueryBy(filter);
        }

        private RepositoryConfiguration GetRespositoryConfiguration(IRepository repo)
        {
            var commit = repo.Head.Tip;

            if (commit == null)
            {
                throw new GitException(Resources.Exception_CouldNotFindBranchTip);
            }

            var configuration = GetCommitConfiguration(commit);

            if (configuration == null)
            {
                throw new InvalidOperationException(Resources.Exception_CouldNotReadConfigurationFile(Constants.ConfigurationFileName));
            }

            return configuration;
        }

        private RepositoryConfiguration? GetCommitConfiguration(Commit commit)
        {
            var gitObj = commit.Tree[Constants.ConfigurationFileName]?.Target;
            var blob = gitObj?.Peel<Blob>();
            if (blob == null)
            {
                return null;
            }

            return Read(blob.GetContentText());
        }

        private RepositoryConfiguration? Read(string rawSettings)
        {
            RepositoryConfiguration? result = null;
            try
            {
                result = _serializer.Deserialize<RepositoryConfiguration>(rawSettings);
            }

            // TODO: Re-enable rule
#pragma warning disable CA1031 // Do not catch general exception types
            catch
#pragma warning restore CA1031 // Do not catch general exception types
            {
                // TODO handle logger of invalid parsing
                Debug.WriteLine("Settings are in an incorrect format");
            }

            return result;
        }

        private bool HasVersionChange(
           TreeChanges diff,
           Commit commit,
           IVersionContext context)
        {
            if (diff.Any(d => d.Path == Constants.ConfigurationFileName))
            {
                var configAtCommit = GetCommitConfiguration(commit);
                if (configAtCommit == null)
                {
                    return false;
                }

                var branchConfigAtCommit = GetBranchConfiguration(configAtCommit, context.Result.CanonicalBranchName);
                return !_comparer.Equals(context.Configuration, branchConfigAtCommit);
            }

            return false;
        }

        private void SetHeight(IVersionContext context, IRepository repo)
        {
            // Initialize count - The current commit counts, include offset
            var height = 1 + context.Configuration.OffSet;

            // skip the first commit as that is our baseline
            var commits = GetReachableCommits(repo).Skip(1);

            // Get the state of this tree for commit comparison
            var prevTree = repo.Head.Tip.Tree;
            foreach (var commit in commits)
            {
                // Get the current tree
                var currentTree = commit.Tree;

                // Compare current and previous trees
                var diff = repo.Diff.Compare<TreeChanges>(currentTree, prevTree, new CompareOptions { Similarity = SimilarityOptions.None });

                // If a change to the file is found, stop counting
                if (HasVersionChange(diff, commit, context))
                {
                    break;
                }

                // Increment height
                height++;

                // Use current commit as a base for next iteration, instead of accessing the tip.
                // This way we don't re-check same commit multiple times.
                // Must make no difference as changes have accumulative nature.
                prevTree = currentTree;
            }

            context.Result.Height = height;
        }
    }
}