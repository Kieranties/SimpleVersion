// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using FluentAssertions;
using GitTools.Testing;
using SimpleVersion.Model;
using SimpleVersion.Pipeline;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace SimpleVersion.Core.Tests.Pipeline
{
    public class ConfigurationContextProcessorFixture
    {
        private readonly ConfigurationContextProcessor _sut;

        public ConfigurationContextProcessorFixture()
        {
            _sut = new ConfigurationContextProcessor();
        }

        [Fact]
        public void Apply_NullContext_Throws()
        {
            // Arrange / Act
            Action action = () => _sut.Apply(null);

            // Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("context");
        }

        [Fact]
        public void Apply_NoCommits_ShouldThrow()
        {
            using (var fixture = new EmptyRepositoryFixture())
            {
                // Arrange
                var context = new VersionContext(fixture.Repository);

                // Act
                Action action = () => _sut.Apply(context);

                // Assert
                action.Should().Throw<InvalidOperationException>()
                    .WithMessage($"Could not read '{Constants.VersionFileName}', has it been committed?");
            }
        }

        [Fact]
        public void Apply_NoCommitsForFile_ShouldThrow()
        {
            using (var fixture = new EmptyRepositoryFixture())
            {
                // Arrange
                fixture.MakeACommit();
                fixture.MakeACommit();
                fixture.MakeACommit();

                var context = new VersionContext(fixture.Repository);

                // Act
                Action action = () => _sut.Apply(context);

                // Assert
                action.Should().Throw<InvalidOperationException>()
                    .WithMessage($"Could not read '{Constants.VersionFileName}', has it been committed?");
            }
        }

        [Fact]
        public void Apply_First_Commit_Height_Is_One()
        {
            using (var fixture = new SimpleVersionRepositoryFixture())
            {
                // Arrange
                var context = new VersionContext(fixture.Repository);

                // Act
                _sut.Apply(context);

                // Assert
                context.Result.Height.Should().Be(1);
            }
        }

        [Fact]
        public void Apply_Single_Branch_Increments_Each_Commit()
        {
            using (var fixture = new SimpleVersionRepositoryFixture())
            {
                // Arrange
                fixture.MakeACommit();
                fixture.MakeACommit();
                fixture.MakeACommit();
                fixture.MakeACommit();

                var context = new VersionContext(fixture.Repository);

                // Act
                _sut.Apply(context);

                // Assert
                context.Result.Height.Should().Be(5);
            }
        }

        [Fact]
        public void Apply_Modified_No_Version_Or_Label_Changes_Does_Not_Reset()
        {
            using (var fixture = new SimpleVersionRepositoryFixture())
            {
                // Arrange
                var config = fixture.GetConfig();

                fixture.MakeACommit();
                fixture.MakeACommit();
                fixture.MakeACommit();
                fixture.MakeACommit();

                config.Metadata.Add("example");
                fixture.SetConfig(config);

                var context = new VersionContext(fixture.Repository);

                // Act
                _sut.Apply(context);

                // Assert
                context.Result.Height.Should().Be(6);
            }
        }

        [Fact]
        public void Apply_Feature_Branch_No_Change_Increments_Merge_Once()
        {
            using (var fixture = new SimpleVersionRepositoryFixture())
            {
                // Arrange
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

                var context = new VersionContext(fixture.Repository);

                // Act
                _sut.Apply(context);

                context.Result.Height.Should().Be(6);
            }
        }

        [Fact]
        public void Apply_Feature_Branch_Changes_Version_Resets_On_Merge()
        {
            using (var fixture = new SimpleVersionRepositoryFixture())
            {
                // Arrange
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

                var context = new VersionContext(fixture.Repository);

                // Act
                _sut.Apply(context);

                // Assert
                context.Result.Height.Should().Be(1);
            }
        }

        [Fact]
        public void Apply_Feature_Branch_Changes_Version_Resets_On_Each_Merge()
        {
            using (var fixture = new SimpleVersionRepositoryFixture())
            {
                // Arrange
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

                var context = new VersionContext(fixture.Repository);

                // Act
                _sut.Apply(context);

                // Assert
                context.Result.Height.Should().Be(1);
            }
        }

        [Fact]
        public void Apply_Feature_Branch_Sets_BranchName()
        {
            using (var fixture = new SimpleVersionRepositoryFixture())
            {
                // Arrange
                fixture.MakeACommit();
                fixture.MakeACommit();
                fixture.MakeACommit();
                fixture.MakeACommit();

                fixture.BranchTo("feature/other");
                fixture.MakeACommit();
                fixture.MakeACommit();
                fixture.MakeACommit();

                var context = new VersionContext(fixture.Repository);

                // Act
                _sut.Apply(context);

                context.Result.BranchName.Should().Be("feature/other");
                context.Result.CanonicalBranchName.Should().Be("refs/heads/feature/other");
            }
        }

        [Fact]
        public void Apply_BranchOverride_AppliesOverride()
        {
            // Arrange
            var expectedLabel = new List<string> { "{branchName}" };
            var expectedMeta = new List<string> { "meta" };

            var config = new Configuration
            {
                Version = "0.1.0",
                Branches =
                {
                    Overrides =
                    {
                        new BranchConfiguration
                        {
                            Match = "feature/other",
                            Label = expectedLabel,
                            Metadata = expectedMeta
                        }
                    }
                }
            };

            using (var fixture = new SimpleVersionRepositoryFixture(config))
            {
                fixture.MakeACommit();
                fixture.MakeACommit();
                fixture.MakeACommit();
                fixture.MakeACommit();

                fixture.BranchTo("feature/other");
                fixture.MakeACommit();
                fixture.MakeACommit();
                fixture.MakeACommit();

                var context = new VersionContext(fixture.Repository);

                // Act
                _sut.Apply(context);

                context.Configuration.Label.Should().BeEquivalentTo(expectedLabel, options => options.WithStrictOrdering());
                context.Configuration.Metadata.Should().BeEquivalentTo(expectedMeta, options => options.WithStrictOrdering());
            }
        }

        [Fact]
        public void Apply_AdvancedBranchOverride_AppliesOverride()
        {
            // Arrange
            var expectedLabel = new List<string> { "preL1", "preL2", "L1", "insertedL", "L2", "postL1", "postL2" };
            var expectedMeta = new List<string> { "preM1", "preM2", "M1", "insertedM", "M2", "postM1", "postM2" };

            var config = new Configuration
            {
                Version = "0.1.0",
                Label = { "L1", "L2" },
                Metadata = { "M1", "M2" },
                Branches =
                {
                    Overrides =
                    {
                        new BranchConfiguration
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

            using (var fixture = new SimpleVersionRepositoryFixture(config))
            {
                fixture.MakeACommit();
                fixture.MakeACommit();
                fixture.MakeACommit();
                fixture.MakeACommit();

                fixture.BranchTo("feature/other");
                fixture.MakeACommit();
                fixture.MakeACommit();
                fixture.MakeACommit();

                var context = new VersionContext(fixture.Repository);

                // Act
                _sut.Apply(context);

                context.Configuration.Label.Should().BeEquivalentTo(expectedLabel, options => options.WithStrictOrdering());
                context.Configuration.Metadata.Should().BeEquivalentTo(expectedMeta, options => options.WithStrictOrdering());
            }
        }

        [Fact]
        public void Apply_Malformed_Json_At_Commit_Throws()
        {
            using (var fixture = new SimpleVersionRepositoryFixture())
            {
                // Arrange
                fixture.MakeACommit();
                fixture.MakeACommit();
                fixture.MakeACommit();

                // Write the version file (with parsing errors)
                var file = Path.Combine(fixture.RepositoryPath, Constants.VersionFileName);
                using (var writer = File.AppendText(file))
                {
                    writer.WriteLine("This will not parse");
                    writer.Flush();
                }

                fixture.Repository.Index.Add(Constants.VersionFileName);
                fixture.Repository.Index.Write();
                fixture.MakeACommit();
                fixture.MakeACommit();
                fixture.MakeACommit();
                fixture.MakeACommit();

                var context = new VersionContext(fixture.Repository);

                // Act
                Action action = () => _sut.Apply(context);

                // Assert
                action.Should().Throw<InvalidOperationException>()
                    .WithMessage($"Could not read '{Constants.VersionFileName}', has it been committed?");
            }
        }

        [Fact]
        public void Apply_Malformed_Json_Committed_Counts_As_No_Change()
        {
            using (var fixture = new SimpleVersionRepositoryFixture())
            {
                // Arrange
                fixture.MakeACommit();
                fixture.MakeACommit();
                fixture.MakeACommit();

                // Write the version file (with parsing errors)
                var file = Path.Combine(fixture.RepositoryPath, Constants.VersionFileName);

                using (var writer = File.AppendText(file))
                {
                    writer.WriteLine("This will not parse");
                    writer.Flush();
                }

                fixture.Repository.Index.Add(Constants.VersionFileName);
                fixture.Repository.Index.Write();
                fixture.MakeACommit(); // 5
                fixture.MakeACommit(); // 6
                fixture.MakeACommit(); // 7
                fixture.MakeACommit(); // 8

                var config = new Configuration { Version = "0.1.0" };
                fixture.SetConfig(config);

                var context = new VersionContext(fixture.Repository);

                // Act
                _sut.Apply(context);

                context.Result.Height.Should().Be(9);
            }
        }
    }
}
