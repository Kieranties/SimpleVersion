using SimpleVersion.Git;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleVersion
{
    public class VersionCalculator
    {
        private readonly GitRepository _repo;

        public VersionCalculator(string path)
        {
            _repo = new GitRepository(new JsonVersionInfoReader(), path);
        }

        public VersionResult Calculate()
        {
            var info = _repo.GetResult();



            return new VersionResult();
        }
    }
}
