// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using FluentAssertions;
using NSubstitute;
using SimpleVersion.Pipeline;
using SimpleVersion.Tokens;
using Xunit;

namespace SimpleVersion.Core.Tests.Tokens
{
    public class ShaTokenHandlerFixture
    {
        private readonly ShaTokenHandler _sut;
        private readonly IVersionContext _context;
        private readonly ITokenEvaluator _evaluator;

        public ShaTokenHandlerFixture()
        {
            _sut = new ShaTokenHandler();
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
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t\t  ")]
        public void Process_NoOption_ReturnsSha(string option)
        {
            // Arrange
            var expected = "b49710eeebbf5dbf8e60d35b7340732aab18531d";
            _context.Result.Sha = expected;

            // Act
            var result = _sut.Process(option, _context, _evaluator);

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("1", "b")]
        [InlineData("7", "b49710e")]
        [InlineData("0010", "b49710eeeb")]
        [InlineData("40", "b49710eeebbf5dbf8e60d35b7340732aab18531d")]
        public void Process_OptionValid_ReturnsShaSubstring(string option, string expected)
        {
            // Arrange
            _context.Result.Sha = expected;

            // Act
            var result = _sut.Process(option, _context, _evaluator);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void Process_NullContext_Throws()
        {
            // Arrange
            Action action = () => _sut.Process(null, null, _evaluator);

            // Act / Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("context");
        }

        [Theory]
        [InlineData("0")]
        [InlineData("-1")]
        public void Process_OptionLessThan1_Throws(string option)
        {
            // Arrange
            var sha = "b49710eeebbf5dbf8e60d35b7340732aab18531d";
            _context.Result.Sha = sha;
            Action action = () => _sut.Process(option, _context, _evaluator);

            // Act / Assert
            action.Should().Throw<InvalidOperationException>()
                .WithMessage($"Invalid sha substring length {option}.  Expected an integer greater than 0.");
        }

        [Fact]
        public void Process_OptionGreaterThanLength_Returns()
        {
            // Arrange
            var expected = "b49710eeebbf5dbf8e60d35b7340732aab18531d";
            var option = expected.Length + 10;
            _context.Result.Sha = expected;

            // Act
            var result = _sut.Process(option.ToString(), _context, _evaluator);

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("this is a string")]
        [InlineData("10/20")]
        public void Process_OptionInvalid_Throws(string option)
        {
            // Arrange
            var sha = "b49710eeebbf5dbf8e60d35b7340732aab18531d";
            _context.Result.Sha = sha;
            Action action = () => _sut.Process(option, _context, _evaluator);

            // Act / Assert
            action.Should().Throw<InvalidOperationException>()
                .WithMessage($"Invalid sha substring length {option}.  Expected an integer greater than 0.");
        }
    }
}
