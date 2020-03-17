// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using SimpleVersion.Rules;
using Xunit;

namespace SimpleVersion.Core.Tests.Rules
{
    public class TokenRuleExtensionsFixture
    {
        [Fact]
        public void ApplyTokenRules_NullValue_ReturnsNull()
        {
            // Arrange / Act
            var result = TokenRuleExtensions.ApplyTokenRules<object>(null, null, null);

            // Assert
            result.Should().BeNull();
        }

        public static IEnumerable<object[]> EmptyRules()
        {
            yield return new[] { (IEnumerable<ITokenRule<int>>)null };
            yield return new[] { Enumerable.Empty<ITokenRule<int>>() };
        }

        [Theory]
        [MemberData(nameof(EmptyRules))]
        public void ApplyTokenRules_EmptyRules_ResturnsValue(IEnumerable<ITokenRule<int>> rules)
        {
            // Arrange
            var collection = new[] { 1, 2, 3, 4, 5 };

            // Act
            var result = TokenRuleExtensions.ApplyTokenRules(collection, null, rules);

            // Assert
            result.Should().BeSameAs(collection);
        }

        [Fact]
        public void ApplyTokenRules_WithRules_ReturnsProcessedRules()
        {
            // Arrange
            var collection = new[] { 1, 2, 3, 4, 5 };
            var rule1 = Substitute.For<ITokenRule<int>>();
            rule1.Apply(default, default).ReturnsForAnyArgs(call => call.Arg<IEnumerable<int>>().Concat(new[] { 1 }));
            var rule2 = Substitute.For<ITokenRule<int>>();
            rule2.Apply(default, default).ReturnsForAnyArgs(call => call.Arg<IEnumerable<int>>().Concat(new[] { 2 }));
            var rule3 = Substitute.For<ITokenRule<int>>();
            rule3.Apply(default, default).ReturnsForAnyArgs(call => call.Arg<IEnumerable<int>>().Concat(new[] { 3 }));

            // Act
            var result = TokenRuleExtensions.ApplyTokenRules(collection, null, new[] { rule1, rule2, rule3 });

            // Assert
            result.Should().BeEquivalentTo(new[] { 1, 2, 3, 4, 5, 1, 2, 3 });
        }

        [Fact]
        public void ResolveTokenRules_NullValue_ReturnsNull()
        {
            // Arrange / Act
            var result = TokenRuleExtensions.ResolveTokenRules<object>(null, null, null);

            // Assert
            result.Should().BeNull();
        }

        [Theory]
        [MemberData(nameof(EmptyRules))]
        public void ResolveTokenRules_EmptyRules_ResturnsValue(IEnumerable<ITokenRule<int>> rules)
        {
            // Arrange
            var value = 1;

            // Act
            var result = TokenRuleExtensions.ResolveTokenRules(1, null, rules);

            // Assert
            result.Should().Be(value);
        }

        [Fact]
        public void ResolveTokenRules_WithRules_ReturnsProcessedRules()
        {
            // Arrange
            var value = 1;
            var rule1 = Substitute.For<ITokenRule<int>>();
            rule1.Resolve(default, default).ReturnsForAnyArgs(1);
            var rule2 = Substitute.For<ITokenRule<int>>();
            rule2.Resolve(default, default).ReturnsForAnyArgs(2);
            var rule3 = Substitute.For<ITokenRule<int>>();
            rule3.Resolve(default, default).ReturnsForAnyArgs(3);

            // Act
            var result = TokenRuleExtensions.ResolveTokenRules(value, null, new[] { rule1, rule2, rule3 });

            // Assert
            result.Should().Be(3);
        }
    }
}
