// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using FluentAssertions;
using GitTools.Testing;
using NSubstitute;
using SimpleVersion.Abstractions;
using SimpleVersion.Pipeline;
using Xunit;

namespace SimpleVersion.Core.Tests.Pipeline
{
    public class AzureDevopsContextProcessorFixture
    {
        private readonly IEnvironment _env;
        private readonly AzureDevopsContextProcessor _sut;

        public AzureDevopsContextProcessorFixture()
        {
            _env = Substitute.For<IEnvironment>();
            _sut = new AzureDevopsContextProcessor(_env);
        }

        [Fact]
        public void Apply_NotAzure_DoesNotApply()
        {
            // Arrange
            _env.GetVariable("TF_BUILD").Returns("False");
            var expectedBranchName = "BranchName";
            var expectedCanonicalBranchName = "CanonicalBranchName";
            using (var fixture = new EmptyRepositoryFixture())
            {
                var context = new VersionContext(fixture.RepositoryPath)
                {
                    Result =
                {
                    BranchName = expectedBranchName,
                    CanonicalBranchName = expectedCanonicalBranchName
                }
                };

                // Act
                _sut.Apply(context);

                // Assert
                context.Result.BranchName.Should().Be(expectedBranchName);
                context.Result.CanonicalBranchName.Should().Be(expectedCanonicalBranchName);
            }
        }

        [Theory]
        [InlineData("refs/heads/master", "master")]
        [InlineData("refs/heads/release/1.0/master", "release/1.0/master")]
        [InlineData("refs/pull/10/merge", "pull/10/merge")]
        public void Apply_IsAzure_DoesApply(string canonical, string name)
        {
            // Arrange
            _env.GetVariable("TF_BUILD").Returns("True");
            _env.GetVariable("BUILD_SOURCEBRANCH").Returns(canonical);
            using (var fixture = new EmptyRepositoryFixture())
            {
                var context = new VersionContext(fixture.RepositoryPath)
                {
                    Result =
                    {
                        BranchName = "Default Name",
                        CanonicalBranchName = "Default Canonical Name"
                    }
                };

                // Act
                _sut.Apply(context);

                // Assert
                context.Result.BranchName.Should().Be(name);
                context.Result.CanonicalBranchName.Should().Be(canonical);
            }
        }
    }
}
