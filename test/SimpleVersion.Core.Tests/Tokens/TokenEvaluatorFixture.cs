// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using SimpleVersion.Pipeline;
using SimpleVersion.Tokens;
using Xunit;

namespace SimpleVersion.Core.Tests.Tokens
{
    public class TokenEvaluatorFixture
    {
        [Fact]
        public void Ctor_NullTokens_Throws()
        {
            // Arrange
            Action action = () => new TokenEvaluator(null);

            // Act / Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("handlers");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t\t  ")]
        public void Process_EmptyTokenString_ReturnsValue(string tokenString)
        {
            // Arrange
            var sut = new TokenEvaluator(Enumerable.Empty<ITokenHandler>());

            // Act
            var result = sut.Process(tokenString, null);

            // Assert
            result.Should().Be(tokenString);
        }

        [Theory]
        [InlineData("{}")]
        [InlineData("{1.2}")]
        [InlineData("{#2304820::348%%")]
        public void Process_InvalidFormat_ReturnsValue(string tokenString)
        {
            // Arrange
            var sut = new TokenEvaluator(Enumerable.Empty<ITokenHandler>());

            // Act
            var result = sut.Process(tokenString, null);

            // Assert
            result.Should().Be(tokenString);
        }

        [Fact]
        public void Process_NoHandler_Throws()
        {
            // Arrange
            var tokenString = "{example}";
            var sut = new TokenEvaluator(Enumerable.Empty<ITokenHandler>());

            // Act
            Action action = () => sut.Process(tokenString, null);

            // Assert
            action.Should().Throw<Exception>()
                .WithMessage($"Could not find token handler for request: {tokenString}");
        }

        [Theory]
        [InlineData("{example}", "example", "")]
        [InlineData("{example:7}", "example", "7")]
        [InlineData("{example:hh:mm:dd}", "example", "hh:mm:dd")]
        public void Process_MatchedHandler_CallsHandler(string tokenString, string key, string expectedOption)
        {
            // Arrange
            var context = Substitute.For<IVersionContext>();
            var tokenHandler = Substitute.For<ITokenHandler>();
            tokenHandler.Key.Returns(key);
            tokenHandler.Process(expectedOption, Arg.Any<IVersionContext>(), Arg.Any<ITokenEvaluator>()).Returns("result");

            var sut = new TokenEvaluator(new[] { tokenHandler });

            // Act
            var result = sut.Process(tokenString, context);

            // Assert
            tokenHandler.Received(1).Process(expectedOption, context, sut);
            result.Should().Be("result");
        }

        [Theory]
        [InlineData("*", "*", "")]
        [InlineData("{*:7}", "*", "7")]
        public void Process_HeightTokenSpecialCase_CallsHandler(string tokenString, string key, string expectedOption)
        {
            // Arrange
            var context = Substitute.For<IVersionContext>();
            var tokenHandler = Substitute.For<ITokenHandler>();
            tokenHandler.Key.Returns(key);
            tokenHandler.Process(expectedOption, Arg.Any<IVersionContext>(), Arg.Any<ITokenEvaluator>()).Returns("result");

            var sut = new TokenEvaluator(new[] { tokenHandler });

            // Act
            var result = sut.Process(tokenString, context);

            // Assert
            tokenHandler.Received(1).Process(expectedOption, context, sut);
            result.Should().Be("result");
        }
    }
}
