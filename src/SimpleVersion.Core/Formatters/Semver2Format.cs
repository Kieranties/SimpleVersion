using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SimpleVersion.Formatters
{
    public class Semver2Format : IVersionFormat
    {
        public void Apply(VersionInfo info, VersionResult result)
        {
            var labelParts = new List<string>(info.Label);
            var metaParts = new List<string>(info.MetaData);

            if (!info.Version.Contains("*"))
            {
                if (labelParts.Count == 0)
                {
                    if (!metaParts.Contains("*"))
                        metaParts.Add("*");
                }
                else if (!labelParts.Contains("*"))
                {
                    labelParts.Add("*");
                }
            }

            // add short sha if required
            if (info.Branches.AddShortShaToNonRelease)
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

            var label = string.Join(".", labelParts);
            label = label.Replace("*", result.Height.ToString());

            var meta = string.Join(".", metaParts);
            meta = meta.Replace("*", result.Height.ToString());

            var format = result.Version;

            if (!string.IsNullOrWhiteSpace(label))
                format += $"-{label}";

            if (!string.IsNullOrWhiteSpace(meta))
                format += $"+{meta}";

            result.Formats["Semver2"] = format;
        }
    }
}
