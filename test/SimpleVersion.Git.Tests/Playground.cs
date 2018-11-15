using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using GitTools.Testing;
using System.Linq;
using System.IO;
using LibGit2Sharp;

namespace SimpleVersion.Git.Tests
{
    public class Playground
    {
        [Fact]
        public void Can_Count_Commits()
        {
            using(var fixture = new EmptyRepositoryFixture())
            {
                fixture.MakeACommit();
                fixture.MakeACommit();
                fixture.MakeACommit();
                fixture.MakeACommit();
                fixture.MakeACommit();

                var sut = new HeightCalculator(fixture.Repository);
                var result = sut.Count();

                Assert.Equal(5, result);
            }
        }

        [Fact]
        public void Can_Count_Commits_To_File()
        {
            using(var fixture = new EmptyRepositoryFixture())
            {
                // Create the file
                var versionFileName = "version.json";
                var versionFile = Path.Combine(fixture.RepositoryPath, versionFileName);
                File.WriteAllText(versionFile, "original");

                fixture.Repository.Index.Add(versionFileName);
                fixture.Repository.Index.Write();
                fixture.MakeACommit();
                fixture.MakeACommit();
                fixture.MakeACommit();

                File.WriteAllText(versionFile, "Updated");
                fixture.Repository.Index.Add(versionFileName);
                fixture.Repository.Index.Write();
                fixture.MakeACommit();
                fixture.MakeACommit();

                var sut = new HeightCalculator(fixture.Repository);
                var result = sut.Count(versionFileName);

                Assert.Equal(2, result);
            }
        }
    }
}
