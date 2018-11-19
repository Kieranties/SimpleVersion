using Cake.Core;
using Cake.Core.Annotations;
using SimpleVersion;
using SimpleVersion.Git;

namespace Cake.SimpleVersion
{
    public static class CakeAliases
    {
        [CakeMethodAlias]
        public static VersionResult SimpleVersion(
            this ICakeContext context, 
            string path = null)
        {            
            if(string.IsNullOrWhiteSpace(path))
                path = context.Environment.WorkingDirectory.FullPath;
            
            var reader = new JsonVersionInfoReader();
            var repo = new GitRepository(reader, path);

            return repo.GetResult();
        }
    }
}
