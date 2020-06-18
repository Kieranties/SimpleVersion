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
        private readonly IVersionContext _context;

        public PrTokenFixture()
        {
            _sut = new PrToken();
            _context = Substitute.For<IVersionContext>();
            _context.Result.Returns(new VersionResult());
        }

        [Fact]
        public void Ctor_SetsKey()
        {
            // Assert
            _sut.Key.Should().Be("pr");
        }

        [Fact]
        public void Process_NullContext_Throws()
        {
            // Arrange
            Action action = () => _sut.EvaluateWithOption(null, null, Substitute.For<ITokenEvaluator>());

            // Act / Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("context");
        }

        [Fact]
        public void Process_NullEvaluator_DoesNotThrow()
        {
            // Arrange
            Action action = () => _sut.EvaluateWithOption(null, _context, null);

            // Act / Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void Process_IsPr_ReturnsPrNumber()
        {
            // Arrange
            _context.Result.PullRequestNumber = 5;

            // Act
            var result = _sut.EvaluateWithOption(null, _context, null);

            // Assert
            result.Should().Be("5");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Process_IsNotPr_ReturnsEmpty(int number)
        {
            // Arrange
            _context.Result.PullRequestNumber = number;

            // Act
            var result = _sut.EvaluateWithOption(null, _context, null);

            // Assert
            result.Should().Be(string.Empty);
        }
    }
}
