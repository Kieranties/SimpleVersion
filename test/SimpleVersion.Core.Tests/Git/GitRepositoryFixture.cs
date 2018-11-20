using FluentAssertions;
using GitTools.Testing;
using SimpleVersion.Git;
using System;
using Xunit;

namespace SimpleVersion.Core.Tests.Git
{
    public class GitRepositoryFixture
    {
        [Fact]
        public void GetResult_NoFile_ShouldThrow()
        {
            using (var fixture = new EmptyRepositoryFixture())
            {
                var sut = new GitRepository(new JsonVersionInfoReader(), fixture.RepositoryPath);

                Action action = () => sut.GetResult();

                action.Should().Throw<InvalidOperationException>()
                    .WithMessage($"No commits found for '{Constants.VersionFileName}'");
            }
        }
               
        [Fact]
        public void Playground()
        {
            var writer = new JsonVersionInfoWriter();
            using(var fixture = new EmptyRepositoryFixture())
            {
                var sut = new GitRepository(new JsonVersionInfoReader(), fixture.RepositoryPath);

                // arbitrary commits should not count
                fixture.MakeACommit();
                fixture.MakeACommit();
                fixture.MakeACommit();

                // write the version file
                var info = new VersionInfo { Version = "0.1.0" };
                writer.ToFile(info, fixture.RepositoryPath);
                fixture.Repository.Index.Add(Constants.VersionFileName);
                fixture.Repository.Index.Write();
                fixture.MakeACommit();

                var result = sut.GetResult();
                fixture.ApplyTag(result.Formats["Semver2"]);


                fixture.MakeACommit();
                fixture.MakeACommit();
                fixture.MakeACommit();

                // write the version file
                info = new VersionInfo { Version = "0.1.0", Label = { "alpha1" } };
                writer.ToFile(info, fixture.RepositoryPath);

                // commit the version file
                fixture.Repository.Index.Add(Constants.VersionFileName);
                fixture.Repository.Index.Write();
                fixture.MakeACommit();

                fixture.MakeACommit();
                fixture.MakeACommit();
                fixture.MakeACommit();
                fixture.MakeACommit();
                fixture.MakeACommit();
                fixture.MakeACommit();
                fixture.MakeACommit();
                fixture.MakeACommit();
                fixture.MakeACommit();
                // calculate the version

                result = sut.GetResult();
                fixture.ApplyTag(result.Formats["Semver2"]);

                result.Should().NotBeNull();
            }
        }
    }
}
