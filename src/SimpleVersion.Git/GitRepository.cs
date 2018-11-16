using LibGit2Sharp;
using System.Collections.Generic;
using System.Linq;

namespace SimpleVersion.Git
{
    public class GitRepository : IRepository
    {
        private readonly IVersionModelReader _reader;
        private Repository _repo;

        public GitRepository(IVersionModelReader reader, string path)
        {
            _reader = reader;
            Repository = new Repository(path);
        }

        public Repository Repository { get; }

        public (int height, VersionModel model) GetResult()
        {
            var model = GetVersionModel(Repository.Head.Tip);
            var height = GetHeight(model);

            return (height, model);
        }

        private int GetHeight(VersionModel model)
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

        private bool HasChange(TreeChanges diff, Commit commit, VersionModel model)
        {
            if (diff.Any(d => d.Path == Constants.VersionFileName))
            {
                var version = GetVersionModel(commit);
                return version.Version != model.Version;
            }

            return false;
        }

        private VersionModel GetVersionModel(Commit commit)
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
    }
}
