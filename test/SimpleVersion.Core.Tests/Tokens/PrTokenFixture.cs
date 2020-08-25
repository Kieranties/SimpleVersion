// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using FluentAssertions;
using NSubstitute;
using SimpleVersion.Pipeline;
using SimpleVersion.Tokens;
using Xunit;

namespace SimpleVersion.Core.Tests.Tokens
{
    public class PrTokenFixture
    {
        private readonly PrToken _sut;
        private readonly PrTokenRequest _request;
        private readonly IVersionContext _context;
        private readonly ITokenEvaluator _evaluator;

        public PrTokenFixture()
        {
            _sut = new PrToken();
            _request = new PrTokenRequest();
            _context = Substitute.For<IVersionContext>();
            _evaluator = Substitute.For<ITokenEvaluator>();
            _context.Result.Returns(new VersionResult());
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

        [Fact]
        public void Evaluate_NullEvaluator_DoesNotThrow()
        {
            // Arrange
            Action action = () => _sut.Evaluate(_request, _context, null);

            // Act / Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void Evaluate_IsPr_ReturnsPrNumber()
        {
            // Arrange
            _context.Result.PullRequestNumber = 5;

            // Act
            var result = _sut.Evaluate(_request, _context, null);

            // Assert
            result.Should().Be("5");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Evaluate_IsNotPr_ReturnsEmpty(int number)
        {
            // Arrange
            _context.Result.PullRequestNumber = number;

            // Act
            var result = _sut.Evaluate(_request, _context, null);

            // Assert
            result.Should().Be(string.Empty);
        }
    }
}
