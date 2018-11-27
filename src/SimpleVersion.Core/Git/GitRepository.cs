using LibGit2Sharp;
using SimpleVersion.Formatters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SimpleVersion.Git
{
    public class GitRepository : IDisposable
    {
        private readonly IVersionInfoReader _reader;

        public GitRepository(IVersionInfoReader reader, string path)
        {
            var resolvedPath = LocateRepo(path);

            _reader = reader;
            Repository = new Repository(resolvedPath);
        }

        public Repository Repository { get; }

        public VersionResult GetResult()
        {
            var result = GetInfo(out var info);
            
            new VersionFormat().Apply(info, result);
            new Semver1Format().Apply(info, result);
            new Semver2Format().Apply(info, result);

            return result;
        }

        private VersionResult GetInfo(out VersionInfo info)
        {
            info = GetVersionModel(Repository.Head?.Tip);
            if (info == null)
                throw new InvalidOperationException($"No commits found for '{Constants.VersionFileName}'");

            // Get the state of this tree to compare for diffs
            var tipTree = Repository.Head.Tip.Tree;

            // Initialise count - The current commit counts, include offset
            var height = 1 + info.OffSet;

            // skip the first commit as that is our baseline
            var commits = GetReachableCommits().Skip(1).GetEnumerator();

            while (commits.MoveNext())
            {
                // Get the current tree
                var next = commits.Current.Tree;
                // Perform a diff
                var diff = Repository.Diff.Compare<TreeChanges>(next, tipTree);
                // If a change to the file is found, stop counting
                if (HasVersionChange(diff, commits.Current, info))
                    break;

                // Increment height
                height++;
            }

            return new VersionResult
            {
                Height = height,
                BranchName = Repository.Head.FriendlyName,
                Sha = Repository.Head.Tip.Sha
            };
        }

        private bool HasVersionChange(TreeChanges diff, Commit commit, VersionInfo model)
        {
            if (diff.Any(d => d.Path == Constants.VersionFileName))
            {
                var version = GetVersionModel(commit);
                return !model.Equals(version);
            }

            return false;
        }

        private VersionInfo GetVersionModel(Commit commit)
        {
            var gitObj = commit?.Tree[Constants.VersionFileName]?.Target;
            if (gitObj == null)
                return null;

            return _reader.Read((gitObj as Blob).GetContentText());
        }

        private IEnumerable<Commit> GetReachableCommits()
        {
            var filter = new CommitFilter
            {
                FirstParentOnly = true,
                IncludeReachableFrom = Repository.Head,
                SortBy = CommitSortStrategies.Reverse
            };

            return Repository.Commits.QueryBy(filter).Reverse();
        }

        private string LocateRepo(string path)
        {
            var resolvedPath = Repository.Discover(path);

            if (string.IsNullOrWhiteSpace(resolvedPath))
            {
                throw new DirectoryNotFoundException($"Could not find git repository at '{path}' or any parent directory");
            }
            return resolvedPath;
        }

        public void Dispose()
        {
            Repository?.Dispose();
        }
    }
}
