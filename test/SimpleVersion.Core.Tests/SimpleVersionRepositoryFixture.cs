// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System.IO;
using GitTools.Testing;
using SimpleVersion.Model;
using SimpleVersion.Serialization;

namespace SimpleVersion.Core.Tests
{
    /// <summary>
    /// Creates a repository containing an initial commit on master with some base configuration.
    /// </summary>
    public class SimpleVersionRepositoryFixture : EmptyRepositoryFixture
    {
        private static readonly Settings _defaultConfiguration = new Settings
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

        public SimpleVersionRepositoryFixture(Settings config)
        {
            SetConfig(config);
        }

        public Settings GetConfig()
        {
            var content = File.ReadAllText(Path.Combine(this.RepositoryPath, Constants.VersionFileName));
            return Serializer.Deserialize<Settings>(content);
        }

        public void SetConfig(Settings config, bool commit = true)
        {
            var json = Serializer.Serialize(config);
            var fullPath = Path.Combine(this.RepositoryPath, Constants.VersionFileName);
            File.WriteAllText(fullPath, json);
            this.Repository.Index.Add(Constants.VersionFileName);
            this.Repository.Index.Write();
            if (commit)
            {
                this.MakeACommit();
            }
        }
    }
}
