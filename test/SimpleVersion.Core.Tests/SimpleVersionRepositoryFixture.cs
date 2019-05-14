// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using GitTools.Testing;
using Newtonsoft.Json;
using SimpleVersion.Model;
using System.IO;

namespace SimpleVersion.Core.Tests
{
    /// <summary>
    /// Creates a repository containg an intial commit on master with some base configuration.
    /// </summary>
    public class SimpleVersionRepositoryFixture : EmptyRepositoryFixture
    {
        public SimpleVersionRepositoryFixture() : this(_defaultConfiguration)
        {
        }

        public SimpleVersionRepositoryFixture(Configuration config)
        {
            SetConfig(config);
        }

        public Configuration GetConfig()
        {
            var content = File.ReadAllText(Path.Combine(this.RepositoryPath, Constants.VersionFileName));
            return JsonConvert.DeserializeObject<Configuration>(content);
        }

        public void SetConfig(Configuration config, bool commit = true)
        {
            var json = JsonConvert.SerializeObject(config, Formatting.Indented);
            var fullPath = Path.Combine(this.RepositoryPath, Constants.VersionFileName);
            File.WriteAllText(fullPath, json);
            this.Repository.Index.Add(Constants.VersionFileName);
            this.Repository.Index.Write();
            if (commit)
                this.MakeACommit();
        }

        private static readonly Configuration _defaultConfiguration = new Configuration
        {
            Version = "0.1.0",
            Branches = new BranchInfo
            {
                Release =
                {
                    "^refs/heads/master$",
                    "^refs/heads/release/.+$"
                }
            }
        };
    }
}
