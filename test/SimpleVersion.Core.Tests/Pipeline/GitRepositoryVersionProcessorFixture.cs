// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using GitTools.Testing;
using LibGit2Sharp;
using NSubstitute;
using SimpleVersion.Configuration;
using SimpleVersion.Exceptions;
using SimpleVersion.Pipeline;
using SimpleVersion.Serialization;
using Xunit;

namespace SimpleVersion.Core.Tests.Pipeline
{
    public class GitRepositoryVersionProcessorFixture
    {
        private readonly ISerializer _serializer;
        private readonly IVersionContext _context;

        public GitRepositoryVersionProcessorFixture()
        {
            _serializer = new Serializer();
            _context = Substitute.For<IVersionContext>();
            _context.Result.Returns(new VersionResult());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   \t\t")]
        public void Process_EmptyWorkingDirectory_Throws(string path)
        {
            // Arrange
            _context.WorkingDirectory.Returns(path);
            var sut = new GitRepositoryVersionProcessor(_serializer);

            // Act
            Action action = () => sut.Process(_context);

            // Assert
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
        public void Ctor_InvalidWorkingDirecotory_Throws(string path)
        {
            // Arrange
            _context.WorkingDirectory.Returns(path);
            var sut = new GitRepositoryVersionProcessor(_serializer);

            // Act
            Action action = () => sut.Process(_context);

            // Assert
            action.Should().Throw<DirectoryNotFoundException>()
                .And.Message.Should().Be($"Could not find git repository at '{path}' or any parent directory.");
        }

        [Fact]
        public void Ctor_Null_Serializer_Throws()
        {
            // Act
            Action action = () => new GitRepositoryVersionProcessor(null);

            // Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("serializer");
        }

        [Fact]
        public void GetResult_NoCommits_Throws()
        {
            // Arrange
            using var fixture = new EmptyRepositoryFixture();
            _context.WorkingDirectory.Returns(fixture.RepositoryPath);
            var sut = new GitRepositoryVersionProcessor(_serializer);

            // Act
            Action action = () => sut.Process(_context);

            // Assert
            action.Should().Throw<GitException>()
                .And.Message.Should().Be("Could not find the current branch tip. Unable to identify the current commit.");
        }

        [Fact]
        public void GetResult_NoConfig_Throws()
        {
            // Arrange
            using var fixture = new EmptyRepositoryFixture();
            fixture.MakeACommit();
            _context.WorkingDirectory.Returns(fixture.RepositoryPath);
            var sut = new GitRepositoryVersionProcessor(_serializer);

            // Act
            Action action = () => sut.Process(_context);

            // Assert
            action.Should().Throw<InvalidOperationException>()
                .And.Message.Should().Be($"Could not read '{Constants.ConfigurationFileName}', has it been committed?");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   \t\t")]
        [InlineData("(no branch)")]
        public void GetResult_InvalidCanonicalName_Throws(string branchName)
        {
            using var fixture = new SimpleVersionRepositoryFixture(_serializer);
            _context.Environment.CanonicalBranchName.Returns(branchName);
            _context.WorkingDirectory.Returns(fixture.RepositoryPath);
            var sut = new GitRepositoryVersionProcessor(_serializer);

            // set to headless state
            var commit = fixture.Repository.Head.Tip;
            Commands.Checkout(fixture.Repository, fixture.Repository.Head.Tip);

            // Act
            Action action = () => sut.Process(_context);

            // Assert
            action.Should().Throw<GitException>()
                .And.Message.Should().Be("The branch name could not be resolved from the repository or the environment. Ensure you have checked out a branch (not a commit).");
        }

        [Fact]
        public void GetResult_EnvironmentCanonicalBranchName_UsesEnvironmentValueInResult()
        {
            // Arrange
            using var fixture = new SimpleVersionRepositoryFixture(_serializer);
            _context.WorkingDirectory.Returns(fixture.RepositoryPath);
            var expected = "FROMENV";
            _context.Environment.CanonicalBranchName.Returns(expected);

            var sut = new GitRepositoryVersionProcessor(_serializer);

            // Act
            sut.Process(_context);

            // Assert
            _context.Result.CanonicalBranchName.Should().Be(expected);
        }

        [Fact]
        public void GetResult_EnvironmentBranchName_UsesEnvironmentValueInResult()
        {
            // Arrange
            using var fixture = new SimpleVersionRepositoryFixture(_serializer);
            _context.WorkingDirectory.Returns(fixture.RepositoryPath);
            var expected = "FROMENV";
            _context.Environment.BranchName.Returns(expected);

            var sut = new GitRepositoryVersionProcessor(_serializer);

            // Act
            sut.Process(_context);

            // Assert
            _context.Result.BranchName.Should().Be(expected);
        }

        [Fact]
        public void GetResult_NoEnvironmentCanonicalBranchName_UsesRepoValueInResult()
        {
            // Arrange
            using var fixture = new SimpleVersionRepositoryFixture(_serializer);
            _context.WorkingDirectory.Returns(fixture.RepositoryPath);
            var sut = new GitRepositoryVersionProcessor(_serializer);

            // Act
            sut.Process(_context);

            // Assert
            _context.Result.CanonicalBranchName.Should().Be(fixture.Repository.Head.CanonicalName);
        }

        [Fact]
        public void GetResult_NoEnvironmentBranchName_UsesRepoValueInResult()
        {
            // Arrange
            using var fixture = new SimpleVersionRepositoryFixture(_serializer);
            _context.WorkingDirectory.Returns(fixture.RepositoryPath);
            var sut = new GitRepositoryVersionProcessor(_serializer);

            // Act
            sut.Process(_context);

            // Assert
            _context.Result.BranchName.Should().Be(fixture.Repository.Head.FriendlyName);
        }

        [Fact]
        public void GetResult_SetsSha()
        {
            // Arrange
            using var fixture = new SimpleVersionRepositoryFixture(_serializer);
            _context.WorkingDirectory.Returns(fixture.RepositoryPath);
            var sut = new GitRepositoryVersionProcessor(_serializer);

            // Act
            sut.Process(_context);

            // Assert
            _context.Result.Sha.Should().Be(fixture.Repository.Head.Tip.Sha);
        }

        [Fact]
        public void GetResult_SetsRepositoryPath()
        {
            // Arrange
            using var fixture = new SimpleVersionRepositoryFixture(_serializer);
            _context.WorkingDirectory.Returns(fixture.RepositoryPath);
            var sut = new GitRepositoryVersionProcessor(_serializer);

            // Act
            sut.Process(_context);

            // Assert
            _context.Result.RepositoryPath.Should().Be(fixture.RepositoryPath);
        }

        [Theory]
        [InlineData("feature/testing", false)]
        [InlineData("release/testing", true)]
        public void GetResult_SetsIsRelease(string branchName, bool isRelease)
        {
            // Arrange
            using var fixture = new SimpleVersionRepositoryFixture(_serializer);
            _context.WorkingDirectory.Returns(fixture.RepositoryPath);
            var sut = new GitRepositoryVersionProcessor(_serializer);

            fixture.BranchTo(branchName);

            // Act
            sut.Process(_context);

            // Assert
            _context.Result.IsRelease.Should().Be(isRelease);
        }

        [Fact]
        public void GetResult_First_Commit_Height_Is_One()
        {
            // Arrange
            using var fixture = new SimpleVersionRepositoryFixture(_serializer);
            _context.WorkingDirectory.Returns(fixture.RepositoryPath);
            var sut = new GitRepositoryVersionProcessor(_serializer);

            // Act
            sut.Process(_context);

            // Assert
            _context.Result.Height.Should().Be(1);
        }

        [Fact]
        public void GetResult_Single_Branch_Increments_Each_Commit()
        {
            // Arrange
            using var fixture = new SimpleVersionRepositoryFixture(_serializer);
            _context.WorkingDirectory.Returns(fixture.RepositoryPath);
            var sut = new GitRepositoryVersionProcessor(_serializer);

            fixture.MakeACommit();
            fixture.MakeACommit();
            fixture.MakeACommit();
            fixture.MakeACommit();

            // Act
            sut.Process(_context);

            // Assert
            _context.Result.Height.Should().Be(5);
        }

        [Fact]
        public void GetResult_Modified_No_Version_Or_Label_Changes_Does_Not_Reset()
        {
            // Arrange
            using var fixture = new SimpleVersionRepositoryFixture(_serializer);
            _context.WorkingDirectory.Returns(fixture.RepositoryPath);

            fixture.MakeACommit();
            fixture.MakeACommit();
            fixture.MakeACommit();
            fixture.MakeACommit();

            var config = fixture.GetConfig();
            config.Metadata.Add("example");
            fixture.SetConfig(config);

            var sut = new GitRepositoryVersionProcessor(_serializer);

            // Act
            sut.Process(_context);

            // Assert
            _context.Result.Height.Should().Be(6);
        }

        [Fact]
        public void GetResult_Feature_Branch_No_Change_Increments_Merge_Once()
        {
            // Arrange
            using var fixture = new SimpleVersionRepositoryFixture(_serializer);
            _context.WorkingDirectory.Returns(fixture.RepositoryPath);

            fixture.MakeACommit();
            fixture.MakeACommit();
            fixture.MakeACommit();
            fixture.MakeACommit();

            fixture.BranchTo("feature/other");
            fixture.MakeACommit();
            fixture.MakeACommit();
            fixture.MakeACommit();

            fixture.Checkout("master");
            fixture.MergeNoFF("feature/other");

            var sut = new GitRepositoryVersionProcessor(_serializer);

            // Act
            sut.Process(_context);

            _context.Result.Height.Should().Be(6);
        }

        [Fact]
        public void GetResult_Feature_Branch_Changes_Version_Resets_On_Merge()
        {
            // Arrange
            using var fixture = new SimpleVersionRepositoryFixture(_serializer);
            _context.WorkingDirectory.Returns(fixture.RepositoryPath);

            var config = fixture.GetConfig();

            fixture.MakeACommit();
            fixture.MakeACommit();
            fixture.MakeACommit();
            fixture.MakeACommit();

            fixture.BranchTo("feature/other");
            fixture.MakeACommit();

            config.Version = "0.1.1";
            fixture.SetConfig(config);

            fixture.MakeACommit();
            fixture.MakeACommit();

            fixture.Checkout("master");
            fixture.MergeNoFF("feature/other");

            var sut = new GitRepositoryVersionProcessor(_serializer);

            // Act
            sut.Process(_context);

            _context.Result.Height.Should().Be(1);
        }

        [Fact]
        public void GetResult_Feature_Branch_Changes_Version_Resets_On_Each_Merge()
        {
            // Arrange
            using var fixture = new SimpleVersionRepositoryFixture(_serializer);
            _context.WorkingDirectory.Returns(fixture.RepositoryPath);

            var config = fixture.GetConfig();

            fixture.MakeACommit();
            fixture.MakeACommit();
            fixture.MakeACommit();
            fixture.MakeACommit();

            fixture.BranchTo("feature/other");
            fixture.MakeACommit();

            config.Version = "0.1.1";
            fixture.SetConfig(config);

            fixture.MakeACommit();
            fixture.MakeACommit();

            fixture.Checkout("master");
            fixture.MergeNoFF("feature/other");

            fixture.Checkout("feature/other");
            fixture.MakeACommit();

            config.Version = "0.1.2";
            fixture.SetConfig(config);

            fixture.MakeACommit();
            fixture.MakeACommit();

            fixture.Checkout("master");
            fixture.MergeNoFF("feature/other");

            var sut = new GitRepositoryVersionProcessor(_serializer);

            // Act
            sut.Process(_context);

            _context.Result.Height.Should().Be(1);
        }

        [Fact]
        public void GetResult_Feature_Branch_Sets_BranchName()
        {
            // Arrange
            using var fixture = new SimpleVersionRepositoryFixture(_serializer);
            _context.WorkingDirectory.Returns(fixture.RepositoryPath);

            fixture.MakeACommit();
            fixture.MakeACommit();
            fixture.MakeACommit();
            fixture.MakeACommit();

            fixture.BranchTo("feature/other");
            fixture.MakeACommit();
            fixture.MakeACommit();
            fixture.MakeACommit();

            var sut = new GitRepositoryVersionProcessor(_serializer);

            // Act
            sut.Process(_context);

            _context.Result.BranchName.Should().Be("feature/other");
            _context.Result.CanonicalBranchName.Should().Be("refs/heads/feature/other");
        }

        [Fact]
        public void GetResult_BranchOverride_AppliesOverride()
        {
            // Arrange
            var expectedLabel = new List<string> { "{branchName}" };
            var expectedMeta = new List<string> { "meta" };

            var config = new RepositoryConfiguration
            {
                Version = "0.1.0",
                Branches =
                        {
                            Overrides =
                            {
                                new BranchOverrideConfiguration
                                {
                                    Match = "feature/other",
                                    Label = expectedLabel,
                                    Metadata = expectedMeta
                                }
                            }
                        }
            };

            using var fixture = new SimpleVersionRepositoryFixture(config, _serializer);
            _context.WorkingDirectory.Returns(fixture.RepositoryPath);

            fixture.MakeACommit();
            fixture.MakeACommit();
            fixture.MakeACommit();
            fixture.MakeACommit();

            fixture.BranchTo("feature/other");
            fixture.MakeACommit();
            fixture.MakeACommit();
            fixture.MakeACommit();

            var sut = new GitRepositoryVersionProcessor(_serializer);

            // Act
            sut.Process(_context);

            _context.Configuration.Label.Should().BeEquivalentTo(expectedLabel, options => options.WithStrictOrdering());
            _context.Configuration.Metadata.Should().BeEquivalentTo(expectedMeta, options => options.WithStrictOrdering());
        }

        [Fact]
        public void GetResult_AdvancedBranchOverride_AppliesOverride()
        {
            // Arrange
            var expectedLabel = new List<string> { "preL1", "preL2", "L1", "insertedL", "L2", "postL1", "postL2" };
            var expectedMeta = new List<string> { "preM1", "preM2", "M1", "insertedM", "M2", "postM1", "postM2" };

            var config = new RepositoryConfiguration
            {
                Version = "0.1.0",
                Label = { "L1", "L2" },
                Metadata = { "M1", "M2" },
                Branches =
                        {
                            Overrides =
                            {
                                new BranchOverrideConfiguration
                                {
                                    Match = "feature/other",
                                    PrefixLabel = new List<string> { "preL1", "preL2" },
                                    PostfixLabel = new List<string> { "postL1", "postL2" },
                                    InsertLabel = new Dictionary<int, string> { [1] = "insertedL" },
                                    PrefixMetadata = new List<string> { "preM1", "preM2" },
                                    PostfixMetadata = new List<string> { "postM1", "postM2" },
                                    InsertMetadata = new Dictionary<int, string> { [1] = "insertedM" }
                                }
                            }
                        }
            };

            using var fixture = new SimpleVersionRepositoryFixture(config, _serializer);
            _context.WorkingDirectory.Returns(fixture.RepositoryPath);

            fixture.MakeACommit();
            fixture.MakeACommit();
            fixture.MakeACommit();
            fixture.MakeACommit();

            fixture.BranchTo("feature/other");
            fixture.MakeACommit();
            fixture.MakeACommit();
            fixture.MakeACommit();

            var sut = new GitRepositoryVersionProcessor(_serializer);

            // Act
            sut.Process(_context);

            _context.Configuration.Label.Should().BeEquivalentTo(expectedLabel, options => options.WithStrictOrdering());
            _context.Configuration.Metadata.Should().BeEquivalentTo(expectedMeta, options => options.WithStrictOrdering());
        }

        [Fact]
        public void GetResult_Malformed_Json_At_Commit_Throws()
        {
            // Arrange
            using var fixture = new SimpleVersionRepositoryFixture(_serializer);
            _context.WorkingDirectory.Returns(fixture.RepositoryPath);

            fixture.MakeACommit();
            fixture.MakeACommit();
            fixture.MakeACommit();

            // Write the version file (with parsing errors)
            var file = Path.Combine(fixture.RepositoryPath, Constants.ConfigurationFileName);
            using (var writer = File.AppendText(file))
            {
                writer.WriteLine("This will not parse");
                writer.Flush();
            }

            fixture.Repository.Index.Add(Constants.ConfigurationFileName);
            fixture.Repository.Index.Write();
            fixture.MakeACommit();
            fixture.MakeACommit();
            fixture.MakeACommit();
            fixture.MakeACommit();

            var sut = new GitRepositoryVersionProcessor(_serializer);

            // Act
            Action action = () => sut.Process(_context);

            // Assert
            action.Should().Throw<InvalidOperationException>()
                .WithMessage($"Could not read '{Constants.ConfigurationFileName}', has it been committed?");
        }

        [Fact]
        public void GetResult_Malformed_Json_Committed_Counts_As_No_Change()
        {
            // Arrange
            using var fixture = new SimpleVersionRepositoryFixture(_serializer);
            _context.WorkingDirectory.Returns(fixture.RepositoryPath);

            fixture.MakeACommit();
            fixture.MakeACommit();
            fixture.MakeACommit();

            // Write the version file (with parsing errors)
            var file = Path.Combine(fixture.RepositoryPath, Constants.ConfigurationFileName);

            using (var writer = File.AppendText(file))
            {
                writer.WriteLine("This will not parse");
                writer.Flush();
            }

            fixture.Repository.Index.Add(Constants.ConfigurationFileName);
            fixture.Repository.Index.Write();
            fixture.MakeACommit(); // 5
            fixture.MakeACommit(); // 6
            fixture.MakeACommit(); // 7
            fixture.MakeACommit(); // 8

            var config = new RepositoryConfiguration { Version = "0.1.0" };
            fixture.SetConfig(config);

            var sut = new GitRepositoryVersionProcessor(_serializer);

            // Act
            sut.Process(_context);

            _context.Result.Height.Should().Be(9);
        }
    }
}
