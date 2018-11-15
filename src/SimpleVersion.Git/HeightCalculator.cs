using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleVersion.Git
{
    public class HeightCalculator
    {
        private readonly IRepository _repo;

        public HeightCalculator(IRepository repo)
        {
            _repo = repo;
        }

        public int Count(string path = null)
        {
            if(path == null)
                return _repo.Head.Commits.Count();


            // Get the current tree
            Tree last = _repo.Head.Tip.Tree;
            // Initialise count - the current commit counts
            var count = 1;
            // Skip the first commit (it's the tip)
            var enumerator = _repo.Head.Commits.Skip(1).GetEnumerator();
            while (enumerator.MoveNext())
            {
                // Increment count
                count++;
                // Get the current tree
                var next = enumerator.Current.Tree;
                // Perform a diff
                var diff = _repo.Diff.Compare<TreeChanges>(last, next);
                // If a change to the file is found, stop counting
                if (diff.All(d => d.Path != path))
                    break;
                // Update the next diff tree
                last = next;
            }

            return count;
        }
    }
}
