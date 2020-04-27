// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using GitTools.Testing;
using SimpleVersion.Configuration;
using SimpleVersion.Environment;
using SimpleVersion.Pipeline;

namespace SimpleVersion.Core.Tests
{
    public sealed class Utils
    {
        public static RepositoryConfiguration GetRepositoryConfiguration(
            string version,
            IEnumerable<string> label = null,
            IEnumerable<string> meta = null)
        {
            var info = new RepositoryConfiguration
            {
                Version = version,
                Branches =
                {
                    Release =
                    {
                        "^refs/heads/master$",
                        "^refs/heads/release/.+$"
                    }
                }
            };

            if (label != null)
            {
                info.Label.AddRange(label);
            }

            if (meta != null)
            {
                info.Metadata.AddRange(meta);
            }

            return info;
        }

        public static VersionResult GetVersionResult(
            int height,
            string version = "1.0.0",
            bool release = true)
        {
            var branchName = release ? "release/example" : "feature/example";

            return new VersionResult
            {
                BranchName = branchName,
                CanonicalBranchName = "refs/heads/" + branchName,
                Sha = "4ca82d2c58f48007bf16d69ebf036fc4ebfdd059",
                Height = height,
                IsRelease = release,
                Version = version
            };
        }

        public static void WriteRepoConfig(
            RepositoryFixtureBase fixture, RepositoryConfiguration config, ISerializer serializer)
        {
            var json = serializer.Serialize(config);
            var path = Path.Combine(fixture.RepositoryPath, Constants.ConfigurationFileName);
            File.WriteAllText(path, json);
            fixture.Repository.Index.Add(Constants.ConfigurationFileName);
            fixture.Repository.Index.Write();
            fixture.MakeACommit();
        }

        internal class MockVersionContext : IVersionContext
        {
            public VersionConfiguration Configuration { get; set; } = new VersionConfiguration();

            public IVersionEnvironment Environment { get; set; }

            public VersionResult Result { get; set; } = new VersionResult();
        }
    }
}
