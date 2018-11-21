using System;
using System.Collections.Generic;

namespace SimpleVersion.Core.Tests.Formatters
{
    public static class Utils
    {
        public static VersionInfo GetVersionInfo(string version, IEnumerable<string> label = null, IEnumerable<string> meta = null)
        {
            var info = new VersionInfo
            {
                Version = version,
                Branches = new BranchInfo
                {
                    Release =
                    {
                        "^master$",
                        "^release/.+$"
                    }
                }
            };

            if (label != null)
                info.Label.AddRange(label);

            if (meta != null)
                info.MetaData.AddRange(meta);

            return info;
        }

        public static VersionResult GetVersionResult(int height, bool release = true)
        {
            return new VersionResult
            {
                BranchName = release ? "release/example" : "feature/example",
                Sha = "4ca82d2c58f48007bf16d69ebf036fc4ebfdd059",
                Height = height
            };
        }
    }
}
