using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SimpleVersion.Formatters
{
    public class Semver1Format : IVersionFormat
    {
        public void Apply(VersionInfo info, VersionResult result)
        {
            var labelParts = new List<string>(info.Label);

            // if no label, add height to meta data
            if (labelParts.Count != 0)
                labelParts.Add(result.HeightPadded);

            // add short sha if required
            if (info.Branches.AddShortShaToNonRelease && labelParts.Count > 0)
            {
                var addShortSha = true;
                foreach (var pattern in info.Branches.Release)
                {
                    if (Regex.IsMatch(result.BranchName, pattern))
                    {
                        addShortSha = false;
                        break;
                    }
                }

                if (addShortSha)
                {
                    var shortSha = result.Sha.Substring(0, 7);
                    labelParts.Add(shortSha);
                }
            }

            var label = string.Join("-", labelParts);
            var format = info.Version;
            
            if (!string.IsNullOrWhiteSpace(label))
                format += $"-{label}";
            
            result.Formats["Semver1"] = format;
        }
    }
}
