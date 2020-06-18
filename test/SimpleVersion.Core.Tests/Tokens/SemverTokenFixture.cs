// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using FluentAssertions;
using NSubstitute;
using SimpleVersion.Pipeline;
using SimpleVersion.Tokens;
using Xunit;

namespace SimpleVersion.Core.Tests.Tokens
{
    public class SemverTokenFixture
    {
        private readonly SemverToken _sut;
        private readonly IVersionContext _context;
        private readonly ITokenEvaluator _evaluator;

        public SemverTokenFixture()
        {
            _sut = new SemverToken();
            _context = Substitute.For<IVersionContext>();
            _context.Result.Returns(new VersionResult());
            _evaluator = Substitute.For<ITokenEvaluator>();
            _evaluator.Process("{version}", _context).Returns("version");
            _evaluator.Process(Arg.Is<string>(x => x.Contains("label")), _context).Returns("label");
            _context.Result.IsRelease = true;
        }

        [Fact]
        public void Process_NullContext_Throws()
        {
            // Arrange
            Action action = () => _sut.EvaluateWithOption(null, null, _evaluator);

            // Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("context");
        }

        [Fact]
        public void Process_NullEvaluator_Throws()
        {
            // Arrange
            Action action = () => _sut.EvaluateWithOption(null, _context, null);

            // Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("evaluator");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t\t   ")]
        public void Process_NullOrEmptyOption_UsesDefault(string optionValue)
        {
            // Act
            _sut.EvaluateWithOption(optionValue, _context, _evaluator);

            // Assert
            _evaluator.Received(1).Process("{label:.}", _context);
        }

        [Theory]
        [InlineData("1", "-")]
        [InlineData("2", ".")]
        public void Process_AllowedOption_UsesExpectedDelimiter(string optionValue, string delimiter)
        {
            // Act
            _sut.EvaluateWithOption(optionValue, _context, _evaluator);

            // Assert
            _evaluator.Received(1).Process($"{{label:{delimiter}}}", _context);
        }

        [Theory]
        [InlineData("test")]
        [InlineData("1+1")]
        public void Process_NonIntegerOption_Throws(string optionValue)
        {
            // Arrange
            Action action = () => _sut.EvaluateWithOption(optionValue, _context, _evaluator);

            // Act / Assert
            action.Should().Throw<InvalidOperationException>()
                .WithMessage($"Could not parse value semver option {optionValue}");
        }

        [Fact]
        public void Process_InvalidOption_Throws()
        {
            // Arrange
            Action action = () => _sut.EvaluateWithOption("12", _context, _evaluator);

            // Act / Assert
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("'12' is not a valid semver version");
        }

        [Theory]
        [InlineData("1", "-")]
        [InlineData("2", ".")]
        public void Process_HasLabel_ReturnsLabel(string optionValue, string delimiter)
        {
            // Arrange
            var label = "somelabel";
            var version = "1.2.3";
            _evaluator.Process("{version}", _context).Returns(version);
            _evaluator.Process($"{{label:{delimiter}}}", _context).Returns(label);

            var expected = $"{version}-{label}";

            // Act
            var result = _sut.EvaluateWithOption(optionValue, _context, _evaluator);

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
            _evaluator.Process("{version}", _context).Returns(version);
            _evaluator.Process($"{{label:{delimiter}}}", _context).Returns(label);

            // Act
            var result = _sut.EvaluateWithOption(optionValue, _context, _evaluator);

            // Assert
            result.Should().Be(version);
        }

        [Fact]
        public void Process_Semver1_DoesNotProcessMetadata()
        {
            // Act
            _sut.EvaluateWithOption("1", _context, _evaluator);

            // Assert
            _evaluator.DidNotReceive().Process(Arg.Is<string>(x => x.Contains("metadata")), _context);
        }

        [Fact]
        public void Process_HasLabelAndMetadata_ReturnsVersionLabelAndMetadata()
        {
            // Arrange
            var label = "somelabel";
            var metadata = "meta.data";
            var version = "1.2.3";
            _evaluator.Process("{version}", _context).Returns(version);
            _evaluator.Process("{label:.}", _context).Returns(label);
            _evaluator.Process("{metadata:.}", _context).Returns(metadata);

            var expected = $"{version}-{label}+{metadata}";

            // Act
            var result = _sut.EvaluateWithOption("2", _context, _evaluator);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void Process_NoLabelIncludesMetadata_ReturnsVersionAndMetadata()
        {
            // Arrange
            string label = null;
            var metadata = "meta.data";
            var version = "1.2.3";
            _evaluator.Process("{version}", _context).Returns(version);
            _evaluator.Process("{label:.}", _context).Returns(label);
            _evaluator.Process("{metadata:.}", _context).Returns(metadata);

            // Act
            var result = _sut.EvaluateWithOption("2", _context, _evaluator);

            // Assert
            result.Should().Be("1.2.3+meta.data");
        }
    }
}
