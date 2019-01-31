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
                context.Result.Version = version.ToString();
                context.Result.Major = version.Major;
                context.Result.Minor = version.Minor;
                context.Result.Patch = version.Build > -1 ? version.Build : 0;
                context.Result.Revision = version.Revision > -1 ? version.Revision : 0;
            }
            else
            {
                throw new InvalidOperationException($"Version '{versionString}' is not in a valid format");
            }
        }
    }
}
