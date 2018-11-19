using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleVersion.Formatters
{
    public class Semver1Format : IVersionFormat
    {
        public void Apply(VersionInfo info, VersionResult result)
        {
            var format = info.Version;

            var label = string.Join("-", info.Label);

            if (!string.IsNullOrWhiteSpace(label))
            {
                label += $"-{result.HeightPadded}";
                format += $"-{label}";
            }

            result.Formats["Semver1"] = format;
        }
    }
}
