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
        private readonly ShaTokenRequest _request;
        private readonly IVersionContext _context;
        private readonly ITokenEvaluator _evaluator;

        public ShaTokenFixture()
        {
            _sut = new ShaToken();
            _request = new ShaTokenRequest();
            _context = Substitute.For<IVersionContext>();
            _context.Result.Returns(new VersionResult());
            _evaluator = Substitute.For<ITokenEvaluator>();
        }

        [Fact]
        public void Evaluate_NullRequest_Throws()
        {
            // Arrange
            Action action = () => _sut.Evaluate(null, _context, _evaluator);

            // Act / Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("request");
        }

        [Fact]
        public void Evaluate_NullContext_Throws()
        {
            // Arrange
            Action action = () => _sut.Evaluate(_request, null, _evaluator);

            // Act / Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("context");
        }

        [Theory]
        [InlineData(1, "b")]
        [InlineData(7, "b49710e")]
        [InlineData(40, "b49710eeebbf5dbf8e60d35b7340732aab18531d")]
        public void Process_OptionValid_ReturnsShaSubstring(int option, string expected)
        {
            // Arrange
            _context.Result.Sha = expected;
            _request.Length = option;

            // Act
            var result = _sut.Evaluate(_request, _context, _evaluator);

            // Assert
            result.Should().Be("c" + expected);
        }

        [Fact]
        public void Process_OptionGreaterThanLength_Returns()
        {
            // Arrange
            var sha = "b49710eeebbf5dbf8e60d35b7340732aab18531d";
            _request.Length = sha.Length + 10;
            _context.Result.Sha = sha;

            // Act
            var result = _sut.Evaluate(_request, _context, _evaluator);

            // Assert
            result.Should().Be("c" + sha);
        }
    }
}
