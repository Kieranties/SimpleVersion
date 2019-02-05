using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SimpleVersion.Pipeline.Formatting
{
    public class Semver2FormatProcess : ICalculatorProcess
    {
        public void Apply(VersionContext context)
        {
            var labelParts = new List<string>(context.Configuration.Label);
            var metaParts = new List<string>(context.Configuration.MetaData);

            if (!context.Configuration.Version.Contains("*"))
            {
                // if we have a label, ensure height is included
                if (labelParts.Count != 0 && !labelParts.Contains("*"))
                    labelParts.Add("*");
            }

            // add short sha if required
            var addShortSha = true;
            foreach (var pattern in context.Configuration.Branches.Release)
            {
                if (Regex.IsMatch(context.Result.BranchName, pattern))
                {
                    addShortSha = false;
                    break;
                }
            }

            if (addShortSha)
            {
                var shortSha = context.Result.Sha.Substring(0, 7);
                labelParts.Add(shortSha);
            }

            var label = string.Join(".", labelParts);
            label = label.Replace("*", context.Result.Height.ToString());

            var meta = string.Join(".", metaParts);
            meta = meta.Replace("*", context.Result.Height.ToString());

            var format = context.Result.Version;

            if (!string.IsNullOrWhiteSpace(label))
                format += $"-{label}";

            if (!string.IsNullOrWhiteSpace(meta))
                format += $"+{meta}";

            context.Result.Formats["Semver2"] = format;
        }
    }
}
