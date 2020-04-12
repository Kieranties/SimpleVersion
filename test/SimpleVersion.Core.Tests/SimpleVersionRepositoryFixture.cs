// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System.IO;
using GitTools.Testing;
using SimpleVersion.Configuration;
using SimpleVersion.Serialization;

namespace SimpleVersion.Core.Tests
{
    /// <summary>
    /// Creates a repository containing an initial commit on master with some base configuration.
    /// </summary>
    public class SimpleVersionRepositoryFixture : EmptyRepositoryFixture
    {
        private static readonly RepositoryConfiguration _defaultConfiguration = new RepositoryConfiguration
        {
            Version = "0.1.0",
            Branches =
            {
                Release =
                {
                    "^refs/heads/master$",
                    "^refs/heads/release/.+$"
                }
            }
        };

        public SimpleVersionRepositoryFixture() : this(_defaultConfiguration)
        {
        }

        public SimpleVersionRepositoryFixture(RepositoryConfiguration config)
        {
            SetConfig(config);
        }

        public VersionConfiguration GetConfig()
        {
            var content = File.ReadAllText(Path.Combine(this.RepositoryPath, Constants.ConfigurationFileName));
            return Serializer.Deserialize<VersionConfiguration>(content);
        }

        public void SetConfig(VersionConfiguration config, bool commit = true)
        {
            var json = Serializer.Serialize(config);
            var fullPath = Path.Combine(this.RepositoryPath, Constants.ConfigurationFileName);
            File.WriteAllText(fullPath, json);
            this.Repository.Index.Add(Constants.ConfigurationFileName);
            this.Repository.Index.Write();
            if (commit)
            {
                this.MakeACommit();
            }
        }
    }
}
