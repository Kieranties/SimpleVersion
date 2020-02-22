// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using FluentAssertions;
using GitTools.Testing;
using SimpleVersion.Pipeline;
using SimpleVersion.Rules;
using Xunit;

namespace SimpleVersion.Core.Tests.Rules
{
    public class PullRequestIdTokenRuleFixture
    {
        private readonly PullRequestIdTokenRule _sut;

        public PullRequestIdTokenRuleFixture()
        {
            _sut = PullRequestIdTokenRule.Instance;
        }

        [Fact]
        public void Token_Is_Correct()
        {
            // Act / Assert
            _sut.Token.Should().Be("{pr}");
        }

        [Theory]
        [InlineData("{pr}", 5, "5")]
        [InlineData("{PR}", 10, "10")]
        [InlineData("this-{pr}", 15, "this-15")]
        [InlineData("this", 20, "this")]
        public void Resolve_ReplacesToken_IfNeeded(string input, int id, string expected)
        {
            // Arrange
            using (var fixture = new EmptyRepositoryFixture())
            {
                fixture.MakeACommit();
                var contextResult = Utils.GetVersionResult(10);
                contextResult.CanonicalBranchName = $"refs/pull/{id}/merge";

                var context = new VersionContext(fixture.Repository)
                {
                    Result = contextResult
                };

                // Act
                var result = _sut.Resolve(context, input);

                // Assert
                result.Should().Be(expected);
            }
        }

        [Fact]
        public void Apply_ReturnsInput()
        {
            // Arrange
            using (var fixture = new EmptyRepositoryFixture())
            {
                fixture.MakeACommit();
                var context = new VersionContext(fixture.Repository)
                {
                    Configuration = Utils.GetConfiguration("1.2.3"),
                    Result = Utils.GetVersionResult(10)
                };

                var input = new[] { "this", "will", "not", "change", "{pr}" };

                // Act
                var result = _sut.Apply(context, input);

                // Assert
                result.Should().BeEquivalentTo(input, options => options.WithStrictOrdering());
            }
        }
    }
}
