// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Collections.Generic;
using FluentAssertions;
using SimpleVersion.Pipeline;
using SimpleVersion.Rules;
using Xunit;
using static SimpleVersion.Core.Tests.Utils;

namespace SimpleVersion.Core.Tests.Rules
{
    public class BranchNameTokenRuleFixture
    {
        [Fact]
        public void Instance_SetsDefaults()
        {
            // Arrange / Act
            var sut = BranchNameTokenRule.Instance;

            // Assert
            sut.Pattern.Should().NotBeNull();
            sut.Token.Should().Be("{branchname}");
        }

        public static IEnumerable<object[]> ApplyData()
        {
            yield return new object[] { null, null };
            yield return new object[] { null, Array.Empty<string>() };
        }

        [Theory]
        [MemberData(nameof(ApplyData))]
        public void Apply_Returns_Input(VersionContext context, IEnumerable<string> input)
        {
            // Arrange
            var sut = new BranchNameTokenRule();

            // Act
            var result = sut.Apply(context, input);

            // Assert
            result.Should().BeSameAs(input);
        }

        [Theory]
        [InlineData("refs/heads/master", "{branchName}", "refsheadsmaster")] // Ignore spelling: refsheadsmaster
        [InlineData("refs/heads/master", "{BRANCHNAME}", "refsheadsmaster")]
        [InlineData("refs/heads/release/1.0", "{BRANCHNAME}", "refsheadsrelease10")]
        [InlineData("refs/heads/release-1.0", "{BRANCHNAME}", "refsheadsrelease10")]
        public void Resolve_Replaces_CanonicalBranchName(string branchName, string input, string expected)
        {
            // Arrange
            var sut = new BranchNameTokenRule();

            var context = new MockVersionContext
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
        [InlineData("master", "{branchName}", "[mr]", "aste")] // Ignore spelling: mr,aste
        public void Resolve_CustomPattern_Replaces_BranchName(string branchName, string input, string pattern, string expected)
        {
            // Arrange
            var sut = new BranchNameTokenRule(pattern);
            var context = new MockVersionContext
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
