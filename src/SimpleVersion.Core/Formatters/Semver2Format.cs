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

            // if no label, add height to meta data
            if (labelParts.Count == 0)
                metaParts.Insert(0, result.Height.ToString());
            else
                labelParts.Add(result.Height.ToString());

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
            var meta = string.Join(".", metaParts);


            var format = info.Version;

            if (!string.IsNullOrWhiteSpace(label))
                format += $"-{label}";

            if (!string.IsNullOrWhiteSpace(meta))
                format += $"+{meta}";

            result.Formats["Semver2"] = format;
        }
    }
}
