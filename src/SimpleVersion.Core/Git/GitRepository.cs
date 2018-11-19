using LibGit2Sharp;
using SimpleVersion.Formatters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SimpleVersion.Git
{
    public class GitRepository
    {
        private readonly IVersionInfoReader _reader;

        public GitRepository(IVersionInfoReader reader, string path)
        {
            ValidatePath(path);

            _reader = reader;
            Repository = new Repository(path);
        }

        public Repository Repository { get; }

        public VersionResult GetResult()
        {
            var result = new VersionResult();
            var version = GetInfo(out var height);

            result.Height = height;
            result.BranchName = Repository.Head.FriendlyName;
            result.Sha = Repository.Head.Tip.Sha;

            new VersionFormat().Apply(version, result);
            new Semver1Format().Apply(version, result);
            new Semver2Format().Apply(version, result);

            return result;
        }

        private VersionInfo GetInfo(out int height)
        {
            var model = GetVersionModel(Repository.Head.Tip);
            height = GetHeight(model);

            return model;
        }

        private int GetHeight(VersionInfo model)
        {
            // Get the current tree
            Tree last = Repository.Head.Tip.Tree;
            // Initialise count - the current commit counts
            var count = 1;
            // skip the first commit as that is our baseline
            var commits = GetReachableCommits().Skip(1).GetEnumerator();

            while (commits.MoveNext())
            {
                // Get the current tree
                var next = commits.Current.Tree;
                // Perform a diff
                var diff = Repository.Diff.Compare<TreeChanges>(last, next);
                // If a change to the file is found, stop counting
                if (HasChange(diff, commits.Current, model))
                    break;

                // Update the next diff tree
                last = next;
                // Increment count
                count++;
            }

            return count;
        }

        private bool HasChange(TreeChanges diff, Commit commit, VersionInfo model)
        {
            if (diff.Any(d => d.Path == Constants.VersionFileName))
            {
                var version = GetVersionModel(commit);
                return !version.Equals(model);
            }

            return false;
        }

        private VersionInfo GetVersionModel(Commit commit)
        {
            var blob = commit.Tree[Constants.VersionFileName].Target as Blob;
            return _reader.Read(blob.GetContentText());
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

        private void ValidatePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Null or empty repostiory path", nameof(path));
            }

            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException($"Directory '{path}' does not exist");
            }

            var gitPath = Path.Combine(path, ".git");
            if (!Directory.Exists(gitPath))
            {
                throw new DirectoryNotFoundException($"Could not find git repository '{gitPath}'");
            }
        }
    }
}
