// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

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

            return VersionCalculator
                .Default()
                .GetResult(path);
        }
    }
}
