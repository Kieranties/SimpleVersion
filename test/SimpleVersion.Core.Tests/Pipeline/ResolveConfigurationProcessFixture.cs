using FluentAssertions;
using GitTools.Testing;
using SimpleVersion.Model;
using SimpleVersion.Pipeline;
using System;
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
                    .WithMessage($"No commits found for '{Constants.VersionFileName}'");
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
                    .WithMessage($"No commits found for '{Constants.VersionFileName}'");
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
    }
}
