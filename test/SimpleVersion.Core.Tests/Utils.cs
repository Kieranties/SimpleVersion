// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using SimpleVersion.Model;
using System.Collections.Generic;

namespace SimpleVersion.Core.Tests
{
    public static class Utils
    {
        public static Settings GetConfiguration(string version, IEnumerable<string> label = null, IEnumerable<string> meta = null)
        {
            var info = new Settings
            {
                Version = version,
                Branches = new BranchInfo
                {
                    Release =
                    {
                        "^refs/heads/master$",
                        "^refs/heads/release/.+$"
                    }
                }
            };

            if (label != null)
                info.Label.AddRange(label);

            if (meta != null)
                info.Metadata.AddRange(meta);

            return info;
        }

        public static VersionResult GetVersionResult(int height, bool release = true)
        {
            var branchName = release ? "release/example" : "feature/example";

            return new VersionResult
            {
                BranchName = branchName,
                CanonicalBranchName = "refs/heads/" + branchName,
                Sha = "4ca82d2c58f48007bf16d69ebf036fc4ebfdd059",
                Sha7 = "4ca82d2",
                Height = height
            };
        }
    }
}
