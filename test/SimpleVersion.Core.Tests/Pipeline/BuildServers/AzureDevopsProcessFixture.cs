using SimpleVersion.Pipeline.BuildServers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using NSubstitute;
using SimpleVersion.Pipeline;
using FluentAssertions;

namespace SimpleVersion.Core.Tests.Pipeline.BuildServers
{
    public class AzureDevopsProcessFixture
    {
        private readonly IEnvironment _env;
        private AzureDevopsProcess _sut;

        public AzureDevopsProcessFixture()
        {
            _env = Substitute.For<IEnvironment>();
            _sut = new AzureDevopsProcess(_env);
        }

        [Fact]
        public void Apply_NotAzure_DoesNotApply()
        {
            // Arrange
            _env.GetVariable("TF_BUILD").Returns("False");
            var expectedBranchName = "BranchName";
            var expectedCanonicalBranchName = "CanonicalBranchName";
            var context = new VersionContext
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

        [Theory]
        [InlineData("refs/heads/master", "master")]
        [InlineData("refs/heads/release/1.0/master", "release/1.0/master")]
        [InlineData("refs/pull/10/merge", "pull/10/merge")]
        public void Apply_IsAzure_DoesApply(string canonical, string name)
        {
            // Arrange
            _env.GetVariable("TF_BUILD").Returns("True");
            _env.GetVariable("BUILD_SOURCEBRANCH").Returns(canonical);
            var context = new VersionContext
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
