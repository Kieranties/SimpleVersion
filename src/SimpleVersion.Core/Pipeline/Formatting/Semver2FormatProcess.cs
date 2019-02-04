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
            var AddBranchName = true;

            // Order of operations count here. Each part of the 'label' is added in the oder in which it is processed.

            if (labelParts.Count == 0)
            {
                foreach (var pattern in context.Configuration.Branches.Release)
                {
                    if (Regex.IsMatch(context.Result.BranchName, pattern))
                    {
                        AddBranchName = false;
                        break;
                    }
                }
            }
            else
            {
                AddBranchName = false;
            }

            if (!context.Configuration.Version.Contains("*"))
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

            if (AddBranchName)
            {
                string ShortName = context.Result.BranchName.Remove(0, ((context.Result.BranchName.LastIndexOf('/') + 1)));
                labelParts.Insert(0, ShortName);
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
