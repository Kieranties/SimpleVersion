using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SimpleVersion.Formatters
{
    public class Semver1Format : IVersionFormat
    {
        public void Apply(VersionInfo info, VersionResult result)
        {
            var labelParts = new List<string>(info.Label);

            if (!info.Version.Contains("*"))
            {
                // if we have a label, ensure height is included
                if (labelParts.Count != 0 && !labelParts.Contains("*"))
                    labelParts.Add("*");
            }

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
            label = label.Replace("*", result.HeightPadded);

            var format = result.Version;
            
            if (!string.IsNullOrWhiteSpace(label))
                format += $"-{label}";
            
            result.Formats["Semver1"] = format;
        }
    }
}
