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
            var result = GetInfo(out var info);
            
            new VersionFormat().Apply(info, result);
            new Semver1Format().Apply(info, result);
            new Semver2Format().Apply(info, result);

            return result;
        }

        private VersionResult GetInfo(out VersionInfo info)
        {
            // get the commits for the version file
            var history = GetVersionFileCommits().ToArray();
            if (history.Count() == 0)
                throw new InvalidOperationException($"No commits found for '{Constants.VersionFileName}'");

            // iterate over the commits
            var current = history[0];
            var currentVersion = GetVersionModel(current);
            for (var i = 1; i < history.Count(); i++)
            {
                var next = history[i];
                var nextVersion = GetVersionModel(next);
                if (nextVersion.Equals(currentVersion))
                {
                    current = next;
                    currentVersion = nextVersion;
                }
                else
                {
                    // if the versions are the same, exit loop
                    break;
                }
            }

            info = currentVersion;
            return new VersionResult
            {
                Height = CountCommits(current, Repository.Head.Tip) + info.OffSet,
                BranchName = Repository.Head.FriendlyName,
                Sha = Repository.Head.Tip.Sha
            };
        }

        private VersionInfo GetVersionModel(Commit commit)
        {
            var blob = commit.Tree[Constants.VersionFileName].Target as Blob;
            return _reader.Read(blob.GetContentText());
        }

        private int CountCommits(Commit from, Commit to)
        {
            var filter = new CommitFilter
            {
                FirstParentOnly = true,
                ExcludeReachableFrom = from.Sha,
                IncludeReachableFrom = to.Sha
            };

            return Repository
                .Commits
                .QueryBy(filter)
                .Count() + 1;
        }

        private IEnumerable<Commit> GetVersionFileCommits()
        {
            var filter = new CommitFilter
            {
                FirstParentOnly = true,
                IncludeReachableFrom = Repository.Head,
                SortBy = CommitSortStrategies.Topological | CommitSortStrategies.Time
            };

            return Repository
                .Commits
                .QueryBy(Constants.VersionFileName, filter)
                .Select(x => x.Commit);
        }

        private void ValidatePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Null or empty repository path", nameof(path));
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
