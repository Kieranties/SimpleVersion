// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System.IO;
using GitTools.Testing;
using SimpleVersion.Configuration;

namespace SimpleVersion.Core.Tests
{
    /// <summary>
    /// Creates a repository containing an initial commit on master with some base settings.
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

        private readonly ISerializer _serializer;

        public SimpleVersionRepositoryFixture(ISerializer serializer) : this(_defaultConfiguration, serializer)
        {
        }

        public SimpleVersionRepositoryFixture(RepositoryConfiguration config, ISerializer serializer)
        {
            _serializer = serializer;
            SetConfig(config);
        }

        public RepositoryConfiguration GetConfig()
        {
            var content = File.ReadAllText(Path.Combine(this.RepositoryPath, Constants.ConfigurationFileName));
            return _serializer.Deserialize<RepositoryConfiguration>(content);
        }

        public void SetConfig(RepositoryConfiguration config, bool commit = true)
        {
            var json = _serializer.Serialize(config);
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
