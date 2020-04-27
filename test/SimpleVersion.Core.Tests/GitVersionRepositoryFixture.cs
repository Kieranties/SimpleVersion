// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using GitTools.Testing;
using SimpleVersion.Configuration;
using SimpleVersion.Exceptions;
using SimpleVersion.Pipeline;
using SimpleVersion.Serialization;
using Xunit;
using static SimpleVersion.Core.Tests.Utils;

namespace SimpleVersion.Core.Tests
{
    public class GitVersionRepositoryFixture
    {
        private readonly ISerializer _serializer;

        public GitVersionRepositoryFixture()
        {
            _serializer = new Serializer();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   \t\t")]
        public void Ctor_EmptyPath_Throws(string path)
        {
            // Arrange
            Action action = () => new GitVersionRepository(path, _serializer);

            // Act / Assert
            action.Should().Throw<ArgumentException>()
                .WithMessage("Path must be provided. (Parameter 'path')");
        }

        public static IEnumerable<object[]> InvalidPaths()
        {
            yield return new[] { System.Environment.GetLogicalDrives()[0] };
            yield return new[] { Path.Combine(Directory.GetCurrentDirectory(), "does not exist") };
        }

        [Theory]
        [MemberData(nameof(InvalidPaths))]
        public void Ctor_InvalidPath_Throws(string path)
        {
            // Arrange / Act
            Action action = () => new GitVersionRepository(path, _serializer);

            // Assert
            action.Should().Throw<DirectoryNotFoundException>()
                .And.Message.Should().Be($"Could not find git repository at '{path}' or any parent directory.");
        }

        [Fact]
        public void Ctor_Null_Serializer_Throws()
        {
            // Arrange
            using var fixture = new EmptyRepositoryFixture();

            // Act
            Action action = () => new GitVersionRepository(fixture.RepositoryPath, null);

            // Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("serializer");
        }

        [Fact]
        public void Ctor_NoCommits_Throws()
        {
            // Arrange
            using var fixture = new EmptyRepositoryFixture();

            // Act
            Action action = () => new GitVersionRepository(fixture.RepositoryPath, _serializer);

            // Assert
            action.Should().Throw<GitException>()
                .And.Message.Should().Be("Could not find the current branch tip. Unable to identify the current commit.");
        }

        [Fact]
        public void Ctor_NoConfig_Throws()
        {
            // Arrange
            using var fixture = new EmptyRepositoryFixture();
            fixture.MakeACommit();

            // Act
            Action action = () => new GitVersionRepository(fixture.RepositoryPath, _serializer);

            // Assert
            action.Should().Throw<InvalidOperationException>()
                .And.Message.Should().Be($"Could not read '{Constants.ConfigurationFileName}', has it been committed?");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   \t\t")]
        public void GetConfiguration_EmptyBranch_ReturnsConfigurationForCurrentTip(string branch)
        {
            // Arrange
            using var fixture = new EmptyRepositoryFixture();

            var initConfig = GetSampleConfiguration();
            Utils.WriteRepoConfig(fixture, initConfig, _serializer);

            var sut = new GitVersionRepository(fixture.RepositoryPath, _serializer);

            // Act
            var result = sut.GetConfiguration(branch);

            // Assert
            result.Should().BeEquivalentTo(new VersionConfiguration
            {
                Version = "1.2.3.4",
                OffSet = 10,
                Label = { "this", "that" },
                Metadata = { "much", "meta" }
            });
        }

        [Fact]
        public void GetConfiguration_UnmatchedBranch_ReturnsConfigurationForCurrentTip()
        {
            // Arrange
            using var fixture = new EmptyRepositoryFixture();

            var initConfig = GetSampleConfiguration();
            Utils.WriteRepoConfig(fixture, initConfig, _serializer);

            var sut = new GitVersionRepository(fixture.RepositoryPath, _serializer);

            // Act
            var result = sut.GetConfiguration("feature/x");

            // Assert
            result.Should().BeEquivalentTo(new VersionConfiguration
            {
                Version = "1.2.3.4",
                OffSet = 10,
                Label = { "this", "that" },
                Metadata = { "much", "meta" }
            });
        }

        [Fact]
        public void GetConfiguration_MatchedBranch_ReturnsConfigurationForMatch()
        {
            // Arrange
            using var fixture = new EmptyRepositoryFixture();

            var initConfig = GetSampleConfiguration();
            WriteRepoConfig(fixture, initConfig, _serializer);

            var sut = new GitVersionRepository(fixture.RepositoryPath, _serializer);

            // Act
            var result = sut.GetConfiguration("release/candidate");

            // Assert
            result.Should().BeEquivalentTo(new VersionConfiguration
            {
                Version = "1.2.3.4",
                OffSet = 10,
                Label = { "rc" },
                Metadata = { "much", "meta" }
            });
        }

        [Fact]
        public void Process_NullContext_Throws()
        {
            // Arrange
            using var fixture = new EmptyRepositoryFixture();
            var initConfig = GetSampleConfiguration();
            WriteRepoConfig(fixture, initConfig, _serializer);
            var sut = new GitVersionRepository(fixture.RepositoryPath, _serializer);

            // Act
            Action action = () => sut.Process(null);

            // Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("context");
        }

        [Fact]
        public void Process_First_Commit_Height_Is_One()
        {
            // Arrange
            using var fixture = new EmptyRepositoryFixture();
            var initConfig = GetSampleConfiguration();
            WriteRepoConfig(fixture, initConfig, _serializer);
            var sut = new GitVersionRepository(fixture.RepositoryPath, _serializer);
            var context = new MockVersionContext();

            // Act
            sut.Process(context);

            // Assert
            context.Result.Height.Should().Be(1);
        }

        [Fact]
        public void Apply_Single_Branch_Increments_Each_Commit()
        {
            // Arrange
            using var fixture = new EmptyRepositoryFixture();
            var initConfig = GetSampleConfiguration();
            WriteRepoConfig(fixture, initConfig, _serializer);
            var sut = new GitVersionRepository(fixture.RepositoryPath, _serializer);
            var context = new MockVersionContext();

            fixture.MakeACommit();
            fixture.MakeACommit();
            fixture.MakeACommit();
            fixture.MakeACommit();

            // Act
            sut.Process(context);

            // Assert
            context.Result.Height.Should().Be(5);
        }

        [Fact]
        public void Apply_Modified_No_Version_Or_Label_Changes_Does_Not_Reset()
        {
            // Arrange
            using var fixture = new EmptyRepositoryFixture();
            var config = GetSampleConfiguration();
            WriteRepoConfig(fixture, config, _serializer);

            fixture.MakeACommit();
            fixture.MakeACommit();
            fixture.MakeACommit();
            fixture.MakeACommit();

            config.Metadata.Add("example");
            WriteRepoConfig(fixture, config, _serializer);

            // Act
            var sut = new GitVersionRepository(fixture.RepositoryPath, _serializer);
            
            //sut.Process(context);

            //// Assert
            //context.Result.Height.Should().Be(6);
        }

        private RepositoryConfiguration GetSampleConfiguration()
        {
            return new RepositoryConfiguration
            {
                Version = "1.2.3.4",
                OffSet = 10,
                Label = { "this", "that" },
                Metadata = { "much", "meta" },
                Branches =
                {
                    Overrides =
                    {
                        new BranchOverrideConfiguration
                        {
                            Match = "^release/.+",
                            Label = new List<string> { "rc" }
                        }
                    }
                }
            };
        }
    }
}
