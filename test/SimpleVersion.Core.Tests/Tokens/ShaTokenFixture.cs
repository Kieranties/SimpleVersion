// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using FluentAssertions;
using NSubstitute;
using SimpleVersion.Pipeline;
using SimpleVersion.Tokens;
using Xunit;

namespace SimpleVersion.Core.Tests.Tokens
{
    public class ShaTokenFixture
    {
        private readonly ShaToken _sut;
        private readonly IVersionContext _context;
        private readonly ITokenEvaluator _evaluator;

        public ShaTokenFixture()
        {
            _sut = new ShaToken();
            _context = Substitute.For<IVersionContext>();
            _context.Result.Returns(new VersionResult());
            _evaluator = Substitute.For<ITokenEvaluator>();
        }

        [Fact]
        public void Ctor_SetsProperties()
        {
            // Assert
            _sut.Key.Should().Be("sha");
        }

        [Theory]
        [InlineData("")]
        [InlineData("\t\t  ")]
        [InlineData("this is a string")]
        [InlineData("10/20")]
        [InlineData("0")]
        [InlineData("-1")]
        public void Process_InvalidOption_Throws(string option)
        {
            // Arrange / Act
            Action action = () => _sut.EvaluateWithOption(option, _context, _evaluator);

            // Assert
            action.Should().Throw<InvalidOperationException>()
                .WithMessage($"Invalid sha option '{option}'.  Expected an integer greater than 0, 'full', or 'short'.");
        }

        [Theory]
        [InlineData("1", "b")]
        [InlineData("7", "b49710e")]
        [InlineData(ShaToken.Options.Short, "b49710e")]
        [InlineData("0010", "b49710eeeb")]
        [InlineData("40", "b49710eeebbf5dbf8e60d35b7340732aab18531d")]
        [InlineData(ShaToken.Options.Full, "b49710eeebbf5dbf8e60d35b7340732aab18531d")]
        [InlineData(ShaToken.Options.Default, "b49710eeebbf5dbf8e60d35b7340732aab18531d")]
        public void Process_OptionValid_ReturnsShaSubstring(string option, string expected)
        {
            // Arrange
            _context.Result.Sha = expected;

            // Act
            var result = _sut.EvaluateWithOption(option, _context, _evaluator);

            // Assert
            result.Should().Be("c" + expected);
        }

        [Fact]
        public void Process_NullContext_Throws()
        {
            // Arrange
            Action action = () => _sut.EvaluateWithOption(ShaToken.Options.Default, null, _evaluator);

            // Act / Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("context");
        }

        [Fact]
        public void Process_OptionGreaterThanLength_Returns()
        {
            // Arrange
            var sha = "b49710eeebbf5dbf8e60d35b7340732aab18531d";
            var option = sha.Length + 10;
            _context.Result.Sha = sha;

            // Act
            var result = _sut.EvaluateWithOption(option.ToString(), _context, _evaluator);

            // Assert
            result.Should().Be("c" + sha);
        }
    }
}
