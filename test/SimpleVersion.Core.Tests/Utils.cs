// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using GitTools.Testing;
using Newtonsoft.Json;
using SimpleVersion.Model;
using System;
using System.Collections.Generic;
using System.IO;

namespace SimpleVersion.Core.Tests
{
    public static class Utils
    {
        public static Configuration GetConfiguration(string version, IEnumerable<string> label = null, IEnumerable<string> meta = null)
        {
            var info = new Configuration
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
                info.MetaData.AddRange(meta);

            return info;
        }

        public static void WriteConfiguration(Configuration config, RepositoryFixtureBase fixture, bool commit = true)
        {
            // write the version file
            var json = JsonConvert.SerializeObject(config, Formatting.Indented);
            var fullPath = Path.Combine(fixture.RepositoryPath, Constants.VersionFileName);
            File.WriteAllText(fullPath, json);
            fixture.Repository.Index.Add(Constants.VersionFileName);
            fixture.Repository.Index.Write();
            if (commit)
                fixture.MakeACommit();
        }

        public static VersionResult GetVersionResult(int height, bool release = true)
        {
            var branchName = release ? "release/example" : "feature/example";

            return new VersionResult
            {
                BranchName = branchName,
                CanonicalBranchName = "refs/heads/" + branchName,
                Sha = "4ca82d2c58f48007bf16d69ebf036fc4ebfdd059",
                Height = height
            };
        }
    }
}
