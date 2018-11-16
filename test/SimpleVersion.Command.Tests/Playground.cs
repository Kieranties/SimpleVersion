using Xunit;
using System.IO;
using GitTools.Testing;
using SimpleVersion;

namespace SimpleVersion.Command.Tests
{
    public class Playground
    {
        [Fact]
        public void Can_Count_Commits_to_File()
        {
            var writer = new JsonVersionModelWriter();
            var versionFile = new VersionModel
            {
                Version = "1.2.3"
            };

            using (var fixture = new EmptyRepositoryFixture())
            {
                // Create the file
                var versionFilePath = Path.Combine(fixture.RepositoryPath, Constants.VersionFileName);
                writer.ToFile(versionFile, versionFilePath);

                fixture.Repository.Index.Add(Constants.VersionFileName);
                fixture.Repository.Index.Write();
                fixture.MakeACommit();
                fixture.MakeACommit();
                fixture.MakeACommit();

                versionFile.Version = "1.2.3.4"
                File.WriteAllText(versionFile, "{ \"version\": \"1.2.3\" }");
                fixture.Repository.Index.Add(versionFileName);
                fixture.Repository.Index.Write();
                fixture.MakeACommit();
                fixture.MakeACommit();

                var result = 10;

                Assert.Equal(3, result);
            }
        }
    }
}
