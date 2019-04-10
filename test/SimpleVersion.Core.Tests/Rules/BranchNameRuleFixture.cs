// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using FluentAssertions;
using SimpleVersion.Pipeline;
using SimpleVersion.Rules;
using System;
using System.Collections.Generic;
using Xunit;

namespace SimpleVersion.Core.Tests.Rules
{
    public class BranchNameRuleFixture
    {
        [Fact]
        public void Instance_SetsDefaults()
        {
            // Arrange / Act
            var sut = BranchNameRule.Instance;

            // Assert
            sut.Pattern.Should().NotBeNull();
            sut.Token.Should().Be("{branchname}");
        }

        public static IEnumerable<object[]> ApplyData()
        {
            yield return new object[] { null, null };
            yield return new object[] { null, Array.Empty<string>() };
            yield return new object[] { new VersionContext(), new[] { "this" } };
        }

        [Theory]
        [MemberData(nameof(ApplyData))]
        public void Apply_Returns_Input(VersionContext context, IEnumerable<string> input)
        {
            // Arrange
            var sut = new BranchNameRule();

            // Act
            var result = sut.Apply(context, input);

            // Assert
            result.Should().BeSameAs(input);
        }

        [Theory]
        [InlineData("refs/heads/master", "{branchName}", "refsheadsmaster")]
        [InlineData("refs/heads/master", "{BRANCHNAME}", "refsheadsmaster")]
        [InlineData("refs/heads/release/1.0", "{BRANCHNAME}", "refsheadsrelease10")]
        [InlineData("refs/heads/release-1.0", "{BRANCHNAME}", "refsheadsrelease10")]
        public void Resolve_Replaces_CanonicalBranchName(string branchName, string input, string expected)
        {
            // Arrange
            var sut = new BranchNameRule();
            var context = new VersionContext
            {
                Result =
                {
                    CanonicalBranchName = branchName
                }
            };

            // Act
            var result = sut.Resolve(context, input);

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("master", "{branchName}", "[mr]", "aste")]
        public void Resolve_CustomPattern_Replaces_BranchName(string branchName, string input, string pattern, string expected)
        {
            // Arrange
            var sut = new BranchNameRule(pattern);
            var context = new VersionContext
            {
                Result =
                {
                    CanonicalBranchName = branchName
                }
            };

            // Act
            var result = sut.Resolve(context, input);

            // Assert
            result.Should().Be(expected);
        }
    }
}
