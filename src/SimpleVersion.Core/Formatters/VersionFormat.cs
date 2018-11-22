using System;

namespace SimpleVersion.Formatters
{
    public class VersionFormat : IVersionFormat
    {
        public void Apply(VersionInfo info, VersionResult result)
        {
            var versionString = info.Version;
            if (versionString.Contains("*"))
                versionString = versionString.Replace("*", result.Height.ToString());

            if (Version.TryParse(versionString, out var version))
            {
                result.Version = version.ToString();
                result.Major = version.Major;
                result.Minor = version.Minor;
                result.Patch = version.Build > -1 ? version.Build : 0;
                result.Revision = version.Revision > -1 ? version.Revision : 0;
            }
            else
            {
                throw new InvalidOperationException($"Version '{info.Version}' is not in a valid format");
            }
        }
    }
}
