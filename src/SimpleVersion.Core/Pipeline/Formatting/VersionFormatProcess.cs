using System;

namespace SimpleVersion.Pipeline.Formatting
{
    public class VersionFormatProcess : ICalculatorProcess
    {
        public void Apply(VersionContext context)
        {
            var versionString = context.Configuration.Version;
            if (versionString.Contains("*"))
                versionString = versionString.Replace("*", context.Result.Height.ToString());

            if (Version.TryParse(versionString, out var version))
            {
                context.Result.Major = version.Major > -1 ? version.Major : 0;
                context.Result.Minor = version.Minor > -1 ? version.Minor : 0;
                context.Result.Patch = version.Build > -1 ? version.Build : 0;

                context.Result.Version = $"{context.Result.Major}.{context.Result.Minor}.{context.Result.Patch}";
                if(version.Revision > -1)
                {
                    context.Result.Version += $".{version.Revision}";
                    context.Result.Revision = version.Revision;
                }
            }
            else
            {
                throw new InvalidOperationException($"Version '{versionString}' is not in a valid format");
            }
        }
    }
}
