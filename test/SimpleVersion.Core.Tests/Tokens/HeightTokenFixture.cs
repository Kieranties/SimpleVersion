// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using FluentAssertions;
using NSubstitute;
using SimpleVersion.Pipeline;
using SimpleVersion.Tokens;
using Xunit;

namespace SimpleVersion.Core.Tests.Tokens
{
    public class HeightTokenFixture
    {
        private readonly HeightToken _sut;
        private readonly IVersionContext _context;
        private readonly ITokenEvaluator _evaluator;

        public HeightTokenFixture()
        {
            _sut = new HeightToken();
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
            Action action = () => _sut.Evaluate(new HeightTokenRequest(), null, _evaluator);

            // Act / Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("context");
        }

        [Theory]
        [InlineData(0, 500, "500")]
        [InlineData(1, 500, "500")]
        [InlineData(2, 500, "500")]
        [InlineData(3, 500, "500")]
        [InlineData(4, 500, "0500")]
        public void Evaluate_OptionValue_ReturnsExpected(int padding, int height, string expected)
        {
            // Arrange
            var version = new VersionResult
            {
                Height = height
            };
            _context.Result.Returns(version);
            var request = new HeightTokenRequest { Padding = padding };

            // Act
            var result = _sut.Evaluate(request, _context, null);

            // Assert
            result.Should().Be(expected);
        }
    }
}
