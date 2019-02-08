﻿using FluentAssertions;
using SimpleVersion.Pipeline;
using SimpleVersion.Rules;
using System;
using System.Collections.Generic;
using Xunit;

namespace SimpleVersion.Core.Tests.Rules
{
    public class ShortShaRuleFixture
    {
        private ShortShaRule _sut;

        public ShortShaRuleFixture()
        {
            _sut = new ShortShaRule();
        }

        [Fact]
        public void Token_Is_Correct()
        {
            // Act / Assert
            _sut.Token.Should().Be("{shortsha}");
        }

        [Theory]
        [InlineData("{shortSha}", "c4ca82d2")]
        [InlineData("{SHORTSHA}", "c4ca82d2")]
        [InlineData("this-{shortsha}", "this-c4ca82d2")]
        [InlineData("this", "this")]
        public void Resolve_ReplacesToken_IfNeeded(string input, string expected)
        {
            // Arrange
            var context = new VersionContext
            {
                Result = Utils.GetVersionResult(10)
            };

            // Act
            var result = _sut.Resolve(context, input);

            // Assert
            result.Should().Be(expected);
        }

        public static IEnumerable<object[]> ApplyData()
        {
            // release branch does not include sha
            yield return new object[] { true, new[] { "this" }, new[] { "this" } };
            // non-release sha appends
            yield return new object[] { false, new[] { "this" }, new[] { "this", "{shortsha}" } };
            // non-release sha does not append if already there
            yield return new object[] { false, new[] { "{shortsha}", "this" }, new[] { "{shortsha}", "this" } };
            // empty array appends
            yield return new object[] { false, Array.Empty<string>(), new[] { "{shortsha}" } };
        }

        [Theory]
        [MemberData(nameof(ApplyData))]
        public void Apply_AppendsToken_IfNeeded(bool isRelease, IEnumerable<string> input, IEnumerable<string> expected)
        {
            // Arrange
            var context = new VersionContext
            {
                Configuration = Utils.GetConfiguration("1.2.3"),
                Result = Utils.GetVersionResult(10, isRelease)
            };

            // Act
            var result = _sut.Apply(context, input);

            // Assert
            result.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
        }
    }
}
