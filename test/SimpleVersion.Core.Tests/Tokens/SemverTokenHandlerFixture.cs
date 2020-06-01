// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using FluentAssertions;
using NSubstitute;
using SimpleVersion.Pipeline;
using SimpleVersion.Tokens;
using System;
using Xunit;

namespace SimpleVersion.Core.Tests.Tokens
{
    public class SemverTokenHandlerFixture
    {
        private readonly SemverTokenHandler _sut;
        private readonly IVersionContext _context;
        private readonly ITokenEvaluator _evaluator;

        public SemverTokenHandlerFixture()
        {
            _sut = new SemverTokenHandler();
            _context = Substitute.For<IVersionContext>();
            _evaluator = Substitute.For<ITokenEvaluator>();
            _evaluator.Process("{version}", _context)
                .Returns("version");
            _evaluator.Process(Arg.Is<string>(x => x.Contains("label")), _context)
                .Returns("label");
        }

        [Fact]
        public void Process_NullContext_Throws()
        {
            // Arrange
            Action action = () => _sut.Process(null, null, _evaluator);

            // Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("context");
        }

        [Fact]
        public void Process_NullEvaluator_Throws()
        {
            // Arrange
            Action action = () => _sut.Process(null, _context, null);

            // Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("evaluator");
        }

        [Fact]
        public void Process_NullOption_UsesDefault()
        {
            // Act
            _sut.Process(null, _context, _evaluator);

            // Assert
            _evaluator.Received(1).Process("{label:.}", _context);
        }

        [Theory]
        [InlineData("1", "-")]
        [InlineData("2", ".")]
        public void Process_AllowedOption_UsesExpectedDelimiter(string optionValue, string delimiter)
        {
            // Act
            _sut.Process(optionValue, _context, _evaluator);

            // Assert
            _evaluator.Received(1).Process($"{{label:{delimiter}}}", _context);
        }

        [Theory]
        [InlineData("1", "-")]
        [InlineData("2", ".")]
        public void Process_HasLabel_ReturnsLabel(string optionValue, string delimiter)
        {
            // Arrange
            var label = "somelabel";
            var version = "1.2.3";
            _evaluator.Process("{version}", _context)
                .Returns(version);
            _evaluator.Process($"{{label:{delimiter}}}", _context)
                .Returns(label);

            var expected = $"{version}-{label}";

            // Act
            var result = _sut.Process(optionValue, _context, _evaluator);

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("1", "-")]
        [InlineData("2", ".")]
        public void Process_NoLabel_ReturnsOnlyVersion(string optionValue, string delimiter)
        {
            // Arrange
            string label = null;
            var version = "1.2.3";
            _evaluator.Process("{version}", _context)
                .Returns(version);
            _evaluator.Process($"{{label:{delimiter}}}", _context)
                .Returns(label);

            // Act
            var result = _sut.Process(optionValue, _context, _evaluator);

            // Assert
            result.Should().Be(version);
        }
    }
}
