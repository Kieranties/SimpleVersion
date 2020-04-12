// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Collections.Generic;
using FluentAssertions;
using GitTools.Testing;
using NSubstitute;
using SimpleVersion.Pipeline;
using SimpleVersion.Rules;
using Xunit;

namespace SimpleVersion.Core.Tests.Rules
{
    public class BranchNameSuffixTokenRuleFixture
    {
        [Fact]
        public void Instance_SetsDefaults()
        {
            // Arrange / Act
            var sut = BranchNameSuffixTokenRule.Instance;

            // Assert
            sut.Pattern.Should().NotBeNull();
            sut.Token.Should().Be("{branchnamesuffix}");
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
            var sut = new BranchNameSuffixTokenRule();

            // Act
            var result = sut.Apply(context, input);

            // Assert
            result.Should().BeSameAs(input);
        }

        [Theory]
        [InlineData("refs/heads/master", "{branchNameSuffix}", "master")]
        [InlineData("refs/heads/master", "{BRANCHNAMESUFFIX}", "master")]
        [InlineData("refs/heads/release/1.0", "{BRANCHNAMESuffix}", "10")]
        [InlineData("refs/heads/release-1.0", "{BRANCHNAMEsuffix}", "release10")]
        public void Resolve_Replaces_CanonicalBranchName(string branchName, string input, string expected)
        {
            // Arrange
            var sut = new BranchNameSuffixTokenRule();
            using (var fixture = new EmptyRepositoryFixture())
            {
                fixture.MakeACommit();
                var context = Substitute.For<IVersionContext>();
                var contextResult = new VersionResult
                {
                    CanonicalBranchName = branchName
                };
                context.Result.Returns(contextResult);

                // Act
                var result = sut.Resolve(context, input);

                // Assert
                result.Should().Be(expected);
            }
        }

        [Theory]
        [InlineData("master", "{branchNameSuffix}", "[mr]", "aste")] // cSpell:ignore aste // Ignore Spelling: aste
        public void Resolve_CustomPattern_Replaces_BranchName(string branchName, string input, string pattern, string expected)
        {
            // Arrange
            var sut = new BranchNameSuffixTokenRule(pattern);
            using (var fixture = new EmptyRepositoryFixture())
            {
                fixture.MakeACommit();
                var context = Substitute.For<IVersionContext>();
                var contextResult = new VersionResult
                {
                    CanonicalBranchName = branchName
                };
                context.Result.Returns(contextResult);

                // Act
                var result = sut.Resolve(context, input);

                // Assert
                result.Should().Be(expected);
            }
        }
    }
}
