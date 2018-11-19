using System.Text.RegularExpressions;

namespace SimpleVersion.Formatters
{
    public class VersionFormat : IVersionFormat
    {
        private static readonly Regex _regex = new Regex(@"(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+)(?:\.(?<revision>\d+))?", RegexOptions.Compiled);

        public void Apply(VersionResult result, VersionInfo info)
        {
            var match = _regex.Match(info.Version);
            if (match.Success)
            {
                result.Major = int.Parse(match.Groups["major"].Value);
                result.Minor = int.Parse(match.Groups["minor"].Value);
                result.Patch = int.Parse(match.Groups["patch"].Value);
                var revisionGroup = match.Groups["revision"];
                if (revisionGroup.Success)
                    result.Major = int.Parse(revisionGroup.Value);
                else
                    result.Revision = 0;
            }
        }
    }
}
