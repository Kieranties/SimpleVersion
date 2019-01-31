using Cake.Core;
using Cake.Core.Annotations;
using SimpleVersion.Model;
using SimpleVersion.Pipeline;

namespace Cake.SimpleVersion
{
    public static class CakeAliases
    {
        [CakeMethodAlias]
        public static VersionResult SimpleVersion(
            this ICakeContext context,
            string path = null)
        {
            if (string.IsNullOrWhiteSpace(path))
                path = context.Environment.WorkingDirectory.FullPath;

            context.Log.Write(Core.Diagnostics.Verbosity.Normal, Core.Diagnostics.LogLevel.Information, "Path: {0}", path);
            context.Log.Write(Core.Diagnostics.Verbosity.Normal, Core.Diagnostics.LogLevel.Information, "ContextPath: {0}", context.Environment.WorkingDirectory.FullPath);

            return VersionCalculator
                .Default()
                .GetResult(path);
        }
    }
}
