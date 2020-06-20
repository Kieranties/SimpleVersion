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
                .And.ParamName.Should().Be("tokens");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t\t  ")]
        public void Process_EmptyTokenString_ReturnsValue(string tokenString)
        {
            // Arrange
            var sut = new TokenEvaluator(Enumerable.Empty<IToken>());

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
            var sut = new TokenEvaluator(Enumerable.Empty<IToken>());

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
            var sut = new TokenEvaluator(Enumerable.Empty<IToken>());

            // Act
            Action action = () => sut.Process(tokenString, null);

            // Assert
            action.Should().Throw<Exception>()
                .WithMessage($"Could not find token handler for request: {tokenString}");
        }

        [Theory]
        [InlineData("{example}", "example", null)]
        [InlineData("{example:7}", "example", "7")]
        [InlineData("{example:hh:mm:dd}", "example", "hh:mm:dd")]
        public void Process_MatchedHandler_CallsHandler(string tokenString, string key, string expectedOption)
        {
            // Arrange
            var context = Substitute.For<IVersionContext>();
            var token = Substitute.For<IToken>();
            token.Key.Returns(key);
            if (string.IsNullOrEmpty(expectedOption))
            {
                token.Evaluate(Arg.Any<IVersionContext>(), Arg.Any<ITokenEvaluator>()).Returns("result");
            }
            else
            {
                token.EvaluateWithOption(expectedOption, Arg.Any<IVersionContext>(), Arg.Any<ITokenEvaluator>()).Returns("result");
            }

            var sut = new TokenEvaluator(new[] { token });

            // Act
            var result = sut.Process(tokenString, context);

            // Assert
            result.Should().Be("result");
        }

        [Theory]
        [InlineData("*", "*", null)]
        [InlineData("{*:7}", "*", "7")]
        public void Process_HeightTokenSpecialCase_CallsHandler(string tokenString, string key, string expectedOption)
        {
            // Arrange
            var context = Substitute.For<IVersionContext>();
            var token = Substitute.For<IToken>();
            token.Key.Returns(key);
            if (string.IsNullOrEmpty(expectedOption))
            {
                token.Evaluate(Arg.Any<IVersionContext>(), Arg.Any<ITokenEvaluator>()).Returns("result");
            }
            else
            {
                token.EvaluateWithOption(expectedOption, Arg.Any<IVersionContext>(), Arg.Any<ITokenEvaluator>()).Returns("result");
            }

            var sut = new TokenEvaluator(new[] { token });

            // Act
            var result = sut.Process(tokenString, context);

            // Assert
            result.Should().Be("result");
        }

        [Fact]
        public void Process_MultipleTokens_CallsHandlers()
        {
            // Arrange
            var context = Substitute.For<IVersionContext>();
            var handlers = Enumerable.Range(1, 3)
                .Select(n =>
                {
                    var handler = Substitute.For<IToken>();
                    handler.Key.Returns($"t{n}");
                    handler.Evaluate(Arg.Any<IVersionContext>(), Arg.Any<ITokenEvaluator>()).Returns(n.ToString());
                    return handler;
                });

            var sut = new TokenEvaluator(handlers);

            // Act
            var result = sut.Process("{t1}-{t2}-{t3}{t3}", context);

            // Assert
            result.Should().Be("1-2-33");
        }
    }
}
