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
    public class ResolveConfigurationProcessFixture
    {
        private readonly ResolveConfigurationProcess _sut;

        public ResolveConfigurationProcessFixture()
        {
            _sut = new ResolveConfigurationProcess();
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

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  \t\t")]
        public void Apply_InvalidRepositoryPath_Throws(string path)
        {
            // Arrange
            var context = new VersionContext { RepositoryPath = path };

            // Act
            Action action = () => _sut.Apply(context);

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Apply_NoCommits_ShouldThrow()
        {
            using (var fixture = new EmptyRepositoryFixture())
            {
                // Arrange
                var context = new VersionContext { RepositoryPath = fixture.RepositoryPath };

                // Act
                Action action = () => _sut.Apply(context);

                // Assert
                action.Should().Throw<InvalidOperationException>()
                    .WithMessage($"Could not read '{Constants.VersionFileName}', has it been committed?");
            }
        }

        [Fact]
        public void Apply_CommitsForFile_ShouldThrow()
        {
            using (var fixture = new EmptyRepositoryFixture())
            {
                // Arrange
                var context = new VersionContext { RepositoryPath = fixture.RepositoryPath };

                fixture.MakeACommit();
                fixture.MakeACommit();
                fixture.MakeACommit();

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
            using (var fixture = new EmptyRepositoryFixture())
            {
                // Arrange
                var context = new VersionContext { RepositoryPath = fixture.RepositoryPath };

                // write the version file
                var config = new Configuration { Version = "0.1.0" };
                Utils.WriteConfiguration(config, fixture);

                // Act
                _sut.Apply(context);

                // Assert
                context.Result.Height.Should().Be(1);
            }
        }

        [Fact]
        public void Apply_Single_Branch_Increments_Each_Commit()
        {
            using (var fixture = new EmptyRepositoryFixture())
            {
                // Arrange
                var context = new VersionContext { RepositoryPath = fixture.RepositoryPath };

                // write the version file
                var config = new Configuration { Version = "0.1.0" };
                Utils.WriteConfiguration(config, fixture); // 1

                fixture.MakeACommit(); // 2
                fixture.MakeACommit(); // 3
                fixture.MakeACommit(); // 4
                fixture.MakeACommit(); // 5

                // Act
                _sut.Apply(context);

                // Assert
                context.Result.Height.Should().Be(5);
            }
        }

        [Fact]
        public void Apply_Modfied_No_Version_Or_Label_Changes_Does_Not_Reset()
        {
            using (var fixture = new EmptyRepositoryFixture())
            {
                // Arrange
                var context = new VersionContext { RepositoryPath = fixture.RepositoryPath };

                // write the version file
                var config = new Configuration { Version = "0.1.0" };
                Utils.WriteConfiguration(config, fixture); // 1

                fixture.MakeACommit(); // 2
                fixture.MakeACommit(); // 3
                fixture.MakeACommit(); // 4
                fixture.MakeACommit(); // 5

                config.MetaData.Add("example");
                Utils.WriteConfiguration(config, fixture); // 6

                // Act
                _sut.Apply(context);

                // Assert
                context.Result.Height.Should().Be(6);
            }
        }

        [Fact]
        public void Apply_Feature_Branch_No_Change_Increments_Merge_Once()
        {
            using (var fixture = new EmptyRepositoryFixture())
            {
                // Arrange
                var context = new VersionContext { RepositoryPath = fixture.RepositoryPath };

                // write the version file
                var config = new Configuration { Version = "0.1.0" };
                Utils.WriteConfiguration(config, fixture); // 1

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

                // Act
                _sut.Apply(context);

                context.Result.Height.Should().Be(6);
            }
        }

        [Fact]
        public void Apply_Feature_Branch_Changes_Version_Resets_On_Merge()
        {
            using (var fixture = new EmptyRepositoryFixture())
            {
                // Arrange
                var context = new VersionContext { RepositoryPath = fixture.RepositoryPath };

                // write the version file
                var config = new Configuration { Version = "0.1.0" };
                Utils.WriteConfiguration(config, fixture); // 1

                fixture.MakeACommit(); // 2
                fixture.MakeACommit(); // 3
                fixture.MakeACommit(); // 4
                fixture.MakeACommit(); // 5

                fixture.BranchTo("feature/other");
                fixture.MakeACommit(); // feature 1
                config.Version = "0.1.1";
                Utils.WriteConfiguration(config, fixture); // 1
                fixture.MakeACommit(); // feature 2
                fixture.MakeACommit(); // feature 3

                fixture.Checkout("master");
                fixture.MergeNoFF("feature/other"); // 1

                // Act
                _sut.Apply(context);

                // Assert
                context.Result.Height.Should().Be(1);
            }
        }

        [Fact]
        public void Apply_Feature_Branch_Changes_Version_Resets_On_Each_Merge()
        {
            using (var fixture = new EmptyRepositoryFixture())
            {
                // Arrange
                var context = new VersionContext { RepositoryPath = fixture.RepositoryPath };

                // write the version file
                var config = new Configuration { Version = "0.1.0" };
                Utils.WriteConfiguration(config, fixture); // 1

                fixture.MakeACommit(); // 2
                fixture.MakeACommit(); // 3
                fixture.MakeACommit(); // 4
                fixture.MakeACommit(); // 5

                fixture.BranchTo("feature/other");
                fixture.MakeACommit(); // feature 1
                config.Version = "0.1.1";
                Utils.WriteConfiguration(config, fixture); // 1
                fixture.MakeACommit(); // feature 2
                fixture.MakeACommit(); // feature 3

                fixture.Checkout("master");
                fixture.MergeNoFF("feature/other"); // 1

                fixture.Checkout("feature/other");
                fixture.MakeACommit(); // feature 1
                config.Version = "0.1.2";
                Utils.WriteConfiguration(config, fixture); // 1
                fixture.MakeACommit(); // feature 2
                fixture.MakeACommit(); // feature 3

                fixture.Checkout("master");
                fixture.MergeNoFF("feature/other"); // 1

                // Act
                _sut.Apply(context);

                // Assert
                context.Result.Height.Should().Be(1);
            }
        }

        [Fact]
        public void Apply_Feature_Branch_Sets_BranchName()
        {
            using (var fixture = new EmptyRepositoryFixture())
            {
                // Arrange
                var context = new VersionContext { RepositoryPath = fixture.RepositoryPath };

                // write the version file
                var config = new Configuration { Version = "0.1.0" };
                Utils.WriteConfiguration(config, fixture); // 1

                fixture.MakeACommit(); // 2
                fixture.MakeACommit(); // 3
                fixture.MakeACommit(); // 4
                fixture.MakeACommit(); // 5

                fixture.BranchTo("feature/other");
                fixture.MakeACommit(); // feature 1
                fixture.MakeACommit(); // feature 2
                fixture.MakeACommit(); // feature 3

                // Act
                _sut.Apply(context);

                context.Result.BranchName.Should().Be("feature/other");
                context.Result.CanonicalBranchName.Should().Be("refs/heads/feature/other");
            }
        }

        [Fact]
        public void Apply_BranchOverride_AppliesOverride()
        {
            using (var fixture = new EmptyRepositoryFixture())
            {
                // Arrange
                var context = new VersionContext { RepositoryPath = fixture.RepositoryPath };

                var expectedLabel = new List<string> { "{branchName}" };
                var expectedMeta = new List<string> { "meta" };

                // write the version file
                var config = new Configuration
                {
                    Version = "0.1.0",
                    Branches = {
                        Overrides = {
                            new BranchConfiguration
                            {
                                Match = "feature/other",
                                Label = expectedLabel,
                                MetaData = expectedMeta
                            }
                        }
                    }
                };
                Utils.WriteConfiguration(config, fixture); // 1

                fixture.MakeACommit(); // 2
                fixture.MakeACommit(); // 3
                fixture.MakeACommit(); // 4
                fixture.MakeACommit(); // 5

                fixture.BranchTo("feature/other");
                fixture.MakeACommit(); // feature 1
                fixture.MakeACommit(); // feature 2
                fixture.MakeACommit(); // feature 3

                // Act
                _sut.Apply(context);

                context.Configuration.Label.Should().BeEquivalentTo(expectedLabel, options => options.WithStrictOrdering());
                context.Configuration.MetaData.Should().BeEquivalentTo(expectedMeta, options => options.WithStrictOrdering());
            }
        }

        [Fact]
        public void Apply_Malformed_Json_At_Commit_Throws()
        {
            using (var fixture = new EmptyRepositoryFixture())
            {
                // Arrange
                var context = new VersionContext { RepositoryPath = fixture.RepositoryPath };

                // write the version file (Well formaed)
                var config = new Configuration { Version = "0.1.0" };
                Utils.WriteConfiguration(config, fixture); // 1

                fixture.MakeACommit(); // 2
                fixture.MakeACommit(); // 3
                fixture.MakeACommit(); // 4


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
            using (var fixture = new EmptyRepositoryFixture())
            {
                // Arrange
                var context = new VersionContext { RepositoryPath = fixture.RepositoryPath };

                // write the version file (Well formaed)
                var config = new Configuration { Version = "0.1.0" };
                Utils.WriteConfiguration(config, fixture); // 1

                fixture.MakeACommit(); // 2
                fixture.MakeACommit(); // 3
                fixture.MakeACommit(); // 4


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

                config = new Configuration { Version = "0.1.0" };
                Utils.WriteConfiguration(config, fixture); // 9

                // Act
                _sut.Apply(context);

                context.Result.Height.Should().Be(9);
            }
        }
    }
}
