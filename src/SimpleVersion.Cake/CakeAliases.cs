using System;
using Cake.Core;
using Cake.Core.Annotations;
using SimpleVersion;
using SimpleVersion.Git;
using LogLevel = Cake.Core.Diagnostics.LogLevel;
using Verbosity = Cake.Core.Diagnostics.Verbosity;

namespace Cake.SimpleVersion
{
    public static class CakeAliases
    {
        [CakeMethodAlias]
        public static string SimpleVersion(this ICakeContext context, string path = null)
        {

            context.Log.Write(Verbosity.Normal, LogLevel.Information, "In custom task", Array.Empty<object>());

            if(string.IsNullOrWhiteSpace(path))
                path = context.Environment.WorkingDirectory.FullPath;
            
            var reader = new JsonVersionInfoReader();
            var repo = new GitRepository(reader, path);

            var (height, version) = repo.GetInfo();

            var formatter = new Semver2Formatter();
            var result = formatter.Format(height, version);

            return result.FullVersion;
        }
    }
}
