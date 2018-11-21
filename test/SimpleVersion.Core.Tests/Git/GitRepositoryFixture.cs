using FluentAssertions;
using GitTools.Testing;
using SimpleVersion.Git;
using System;
using Xunit;

namespace SimpleVersion.Core.Tests.Git
{
    public class GitRepositoryFixture
    {
        private readonly JsonVersionInfoWriter _writer;
        private readonly JsonVersionInfoReader _reader;

        public GitRepositoryFixture()
        {
            _writer = new JsonVersionInfoWriter();
            _reader = new JsonVersionInfoReader();
        }

        [Fact]
        public void GetResult_NoCommits_ShouldThrow()
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
        public void GetResult_CommitsForFile_ShouldThrow()
        {
            using (var fixture = new EmptyRepositoryFixture())
            {
                var sut = new GitRepository(new JsonVersionInfoReader(), fixture.RepositoryPath);

                fixture.MakeACommit();
                fixture.MakeACommit();
                fixture.MakeACommit();

                Action action = () => sut.GetResult();

                action.Should().Throw<InvalidOperationException>()
                    .WithMessage($"No commits found for '{Constants.VersionFileName}'");
            }
        }

        [Fact]
        public void GetResult_First_Commit_Height_Is_One()
        {
            var writer = new JsonVersionInfoWriter();
            using (var fixture = new EmptyRepositoryFixture())
            {
                var sut = new GitRepository(new JsonVersionInfoReader(), fixture.RepositoryPath);

                // write the version file
                var info = new VersionInfo { Version = "0.1.0" };
                WriteVersion(info, fixture);

                var result = GetResult(sut, fixture);

                result.Height.Should().Be(1);
            }
        }


        [Fact]
        public void GetResult_Single_Branch_Increments_Each_Commit()
        {
            var writer = new JsonVersionInfoWriter();
            using (var fixture = new EmptyRepositoryFixture())
            {
                var sut = new GitRepository(new JsonVersionInfoReader(), fixture.RepositoryPath);

                // write the version file
                var info = new VersionInfo { Version = "0.1.0" };
                WriteVersion(info, fixture); // 1

                fixture.MakeACommit(); // 2
                fixture.MakeACommit(); // 3
                fixture.MakeACommit(); // 4
                fixture.MakeACommit(); // 5

                var result = GetResult(sut, fixture);

                result.Height.Should().Be(5);
            }
        }

        [Fact]
        public void GetResult_Modfied_No_Version_Or_Label_Changes_Does_Not_Reset()
        {
            var writer = new JsonVersionInfoWriter();
            using (var fixture = new EmptyRepositoryFixture())
            {
                var sut = new GitRepository(new JsonVersionInfoReader(), fixture.RepositoryPath);

                // write the version file
                var info = new VersionInfo { Version = "0.1.0" };
                WriteVersion(info, fixture); // 1

                fixture.MakeACommit(); // 2
                fixture.MakeACommit(); // 3
                fixture.MakeACommit(); // 4
                fixture.MakeACommit(); // 5

                info.MetaData.Add("example");
                WriteVersion(info, fixture); // 6

                var result = GetResult(sut, fixture);

                result.Height.Should().Be(6);
            }
        }

        [Fact]
        public void GetResult_Feature_Branch_No_Change_Increments_Merge_Once()
        {
            var writer = new JsonVersionInfoWriter();
            using (var fixture = new EmptyRepositoryFixture())
            {
                var sut = new GitRepository(new JsonVersionInfoReader(), fixture.RepositoryPath);

                // write the version file
                var info = new VersionInfo { Version = "0.1.0" };
                WriteVersion(info, fixture); // 1

                fixture.MakeACommit(); // 2
                fixture.MakeACommit(); // 3
                fixture.MakeACommit(); // 4
                fixture.MakeACommit(); // 5

                fixture.BranchTo("feature/other");
                fixture.MakeACommit(); // feature 1
                fixture.MakeACommit(); // feature 2
                fixture.MakeACommit(); // feature 3

                fixture.Checkout("master");
                fixture.MergeNoFF("feature/other"); // 6

                var result = GetResult(sut, fixture);

                result.Height.Should().Be(6);
            }
        }

        [Fact]
        public void GetResult_Feature_Branch_Changes_Version_Resets_On_Merge()
        {
            var writer = new JsonVersionInfoWriter();
            using (var fixture = new EmptyRepositoryFixture())
            {
                var sut = new GitRepository(new JsonVersionInfoReader(), fixture.RepositoryPath);

                // write the version file
                var info = new VersionInfo { Version = "0.1.0" };
                WriteVersion(info, fixture); // 1

                fixture.MakeACommit(); // 2
                fixture.MakeACommit(); // 3
                fixture.MakeACommit(); // 4
                fixture.MakeACommit(); // 5

                fixture.BranchTo("feature/other");
                fixture.MakeACommit(); // feature 1
                info = new VersionInfo { Version = "0.1.1" };
                WriteVersion(info, fixture); // 1
                fixture.MakeACommit(); // feature 2
                fixture.MakeACommit(); // feature 3

                fixture.Checkout("master");
                fixture.MergeNoFF("feature/other"); // 1

                var result = GetResult(sut, fixture);

                result.Height.Should().Be(1);
            }
        }

        [Fact]
        public void GetResult_Feature_Branch_Changes_Version_Resets_On_Each_Merge()
        {
            var writer = new JsonVersionInfoWriter();
            using (var fixture = new EmptyRepositoryFixture())
            {
                var sut = new GitRepository(new JsonVersionInfoReader(), fixture.RepositoryPath);

                // write the version file
                var info = new VersionInfo { Version = "0.1.0" };
                WriteVersion(info, fixture); // 1

                fixture.MakeACommit(); // 2
                fixture.MakeACommit(); // 3
                fixture.MakeACommit(); // 4
                fixture.MakeACommit(); // 5

                fixture.BranchTo("feature/other");
                fixture.MakeACommit(); // feature 1
                info = new VersionInfo { Version = "0.1.1" };
                WriteVersion(info, fixture); // 1
                fixture.MakeACommit(); // feature 2
                fixture.MakeACommit(); // feature 3

                fixture.Checkout("master");
                fixture.MergeNoFF("feature/other"); // 1

                fixture.Checkout("feature/other");
                fixture.MakeACommit(); // feature 1
                info = new VersionInfo { Version = "0.1.2" };
                WriteVersion(info, fixture); // 1
                fixture.MakeACommit(); // feature 2
                fixture.MakeACommit(); // feature 3

                fixture.Checkout("master");
                fixture.MergeNoFF("feature/other"); // 1

                var result = GetResult(sut, fixture);

                result.Height.Should().Be(1);
            }
        }

        private void WriteVersion(VersionInfo info, RepositoryFixtureBase fixture)
        {
            // write the version file
            _writer.ToFile(info, fixture.RepositoryPath);
            fixture.Repository.Index.Add(Constants.VersionFileName);
            fixture.Repository.Index.Write();
            fixture.MakeACommit();
        }

        private VersionResult GetResult(GitRepository sut, RepositoryFixtureBase fixture)
        {
            var result = sut.GetResult();
            fixture.ApplyTag(result.Formats["Semver2"]);

            return result;
        }
    }
}
