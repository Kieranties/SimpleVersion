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
        public void Ctor_SetsKey()
        {
            // Act / Assert
            _sut.Key.Should().Be("*");
        }

        [Fact]
        public void Process_NullContext_Throws()
        {
            // Arrange
            Action action = () => _sut.EvaluateWithOption(null, null, _evaluator);

            // Act / Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("context");
        }

        [Theory]
        [InlineData(null, 1, "1")]
        [InlineData("", 10, "10")]
        [InlineData("\t\t   ", 500, "500")]
        [InlineData("0", 500, "500")]
        [InlineData("1", 500, "500")]
        [InlineData("2", 500, "500")]
        [InlineData("3", 500, "500")]
        [InlineData("4", 500, "0500")]
        public void Process_OptionValue_ReturnsExpected(string optionValue, int height, string expected)
        {
            // Arrange
            var version = new VersionResult
            {
                Height = height
            };
            _context.Result.Returns(version);

            // Act
            var result = _sut.EvaluateWithOption(optionValue, _context, null);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void Process_InvalidOption_Throws()
        {
            // Arrange
            Action action = () => _sut.EvaluateWithOption("thing", _context, _evaluator);

            // Act / Assert
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("Invalid option for height token: 'thing'");
        }
    }
}
