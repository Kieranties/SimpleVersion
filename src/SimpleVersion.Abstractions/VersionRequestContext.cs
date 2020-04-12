// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using SimpleVersion.Configuration;
using SimpleVersion.Environment;

namespace SimpleVersion
{
    public class VersionRequestContext
    {
        public VersionRequestContext(
            IVersionEnvironment environment,
            IVersionRepository repo)
        {
            Environment = environment;
            Result = InitResult();
            Configuration = repo.GetConfiguration(Result.CanonicalBranchName);
            repo.UpdateContext(this);

        }

        public IVersionEnvironment Environment { get; }

        public VersionConfiguration Configuration { get; }

        public VersionResult Result { get; }

        private VersionResult InitResult()
        {
            var result = new VersionResult();

            if (Environment.CanonicalBranchName != null)
            {
                result.CanonicalBranchName = Environment.CanonicalBranchName;
            }

            if (Environment.BranchName != null)
            {
                result.BranchName = Environment.BranchName;
            }

            return result;
        }
    }
}
