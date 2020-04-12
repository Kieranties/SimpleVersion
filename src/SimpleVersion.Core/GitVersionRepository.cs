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

namespace SimpleVersion
{
    /// <summary>
    /// Represents a git repostiory.
    /// </summary>
    public class GitVersionRepository : IVersionRepository
    {
        private static readonly VersionConfigurationLabelComparer _comparer = new VersionConfigurationLabelComparer();

        private readonly IRepository _repo;
        private readonly ISerializer _serializer;
        private readonly RepositoryConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="GitVersionRepository"/> class.
        /// </summary>
        /// <param name="path">The path to the repository.</param>
        /// <param name="serializer">The serializer for reading documents.</param>
        public GitVersionRepository(string path, ISerializer serializer)
        {
            // Resolve the underlying repo
            _repo = GetRepository(path);
            _serializer = Assert.ArgumentNotNull(serializer, nameof(serializer));
            _configuration = GetRespositoryConfiguration();
        }

        /// <inheritdoc/>
        public VersionConfiguration GetConfiguration(string? canonicalBranchName)
        {
            return GetBranchConfiguration(_configuration, canonicalBranchName ?? _repo.Head.CanonicalName);
        }

        /// <inheritdoc/>
        public void UpdateContext(VersionRequestContext context)
        {
            Assert.ArgumentNotNull(context, nameof(context));

            if (context.Result.BranchName == null)
            {
                context.Result.BranchName = _repo.Head.FriendlyName;
            }

            if (context.Result.CanonicalBranchName == null)
            {
                context.Result.CanonicalBranchName = _repo.Head.CanonicalName;
            }

            context.Result.Sha = _repo.Head.Tip.Sha;
            context.Result.RepositoryPath = _repo.Info.Path;
            context.Result.Height = GetHeight(context);
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

        private IEnumerable<Commit> GetReachableCommits()
        {
            var filter = new CommitFilter
            {
                FirstParentOnly = true,
                IncludeReachableFrom = _repo.Head,
                SortBy = CommitSortStrategies.Topological
            };

            return _repo.Commits.QueryBy(filter);
        }

        private RepositoryConfiguration GetRespositoryConfiguration()
        {
            var commit = _repo.Head?.Tip;

            if (commit == null)
            {
                throw new GitException(Resources.Exception_CouldNotFindBranchTip);
            }

            var configuration = GetCommitConfiguration(commit);

            if (configuration == null)
            {
                throw new InvalidOperationException(Resources.Exception_CouldNotReadConfigurationFile(Constants.VersionFileName));
            }

            return configuration;
        }

        private RepositoryConfiguration? GetCommitConfiguration(Commit commit)
        {
            var gitObj = commit?.Tree[Constants.VersionFileName]?.Target;
            if (gitObj == null)
            {
                return null;
            }

            var blob = gitObj as Blob;
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
           VersionRequestContext context)
        {
            if (diff.Any(d => d.Path == Constants.VersionFileName))
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

        private int GetHeight(VersionRequestContext context)
        {
            // Initialize count - The current commit counts, include offset
            var height = 1 + context.Configuration.OffSet;

            // skip the first commit as that is our baseline
            var commits = GetReachableCommits().Skip(1);

            // Get the state of this tree to compare for diffs
            var prevTree = _repo.Head.Tip.Tree;
            foreach (var commit in commits)
            {
                // Get the current tree
                var currentTree = commit.Tree;

                // Perform a diff
                var diff = _repo.Diff.Compare<TreeChanges>(currentTree, prevTree, new CompareOptions { Similarity = SimilarityOptions.None });

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

            return height;
        }
    }
}
