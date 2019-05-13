// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using FluentAssertions;
using SimpleVersion.Pipeline;
using SimpleVersion.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SimpleVersion.Core.Tests.Rules
{
    public class HeightTokenRuleFixture
    {
        [Fact]
        public void Instance_Padded_IsFalse()
        {
            // Arrange
            var sut = HeightTokenRule.Instance;

            // Act
            var result = sut.Padded;

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Token_Is_Asterisk()
        {
            // Arrange / Act
            var sut = new HeightTokenRule();

            // Assert
            sut.Token.Should().Be("*");
        }

        [Fact]
        public void Padded_ByDefault_IsFalse()
        {
            // Arrange / Act
            var sut = new HeightTokenRule();

            // Assert
            sut.Padded.Should().BeFalse();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Padded_WhenSet_SetsCorrectly(bool usePadding)
        {
            // Arrange / Act
            var sut = new HeightTokenRule(usePadding);

            // Assert
            sut.Padded.Should().Be(usePadding);
        }

        [Theory]
        [InlineData(true, 10, "myString-*", "myString-0010")]
        [InlineData(false, 15, "myString-*", "myString-15")]
        [InlineData(true, 10, "myString-*.*", "myString-0010.0010")]
        [InlineData(false, 15, "myString-*-*", "myString-15-15")]
        [InlineData(true, 10, "myString", "myString")]
        [InlineData(false, 15, "myString", "myString")]
        public void Resolve_ReplacesToken_IfNeeded(bool usePadding, int height, string input, string expected)
        {
            // Arrange
            var sut = new HeightTokenRule(usePadding);
            var context = new VersionContext("test path")
            {
                Result =
                {
                    Height = height
                }
            };

            // Act
            var result = sut.Resolve(context, input);

            // Assert
            result.Should().Be(expected);
        }

        public static IEnumerable<object[]> ApplyData()
        {
            // Version in string, Do not add to label
            yield return new object[] { "1.*", new[] { "this" }, new[] { "this" } };

            // No release label, Do not add to label
            yield return new object[] { "1.0", Array.Empty<string>(), Array.Empty<string>() };

            // Not in version and release, append to end
            yield return new object[] { "1.0", new[] { "this" }, new[] { "this", "*" } };
        }

        [Theory]
        [MemberData(nameof(ApplyData))]
        public void Apply_Appends_IfRequired(string version, IEnumerable<string> input, IEnumerable<string> expected)
        {
            // Arrange
            var sut = new HeightTokenRule();
            var context = new VersionContext("test path")
            {
                Configuration =
                {
                    Version = version
                }
            };

            // Act
            var result = sut.Apply(context, input);

            // Assert
            result.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
        }
    }
}
