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
            _evaluator.Process<VersionToken>(_context).Returns("version");
            _evaluator.Process<LabelToken>(Arg.Any<string>(), _context).Returns("label");
            _context.Result.IsRelease = true;
        }

        [Fact]
        public void Process_NullContext_Throws()
        {
            // Arrange
            Action action = () => _sut.EvaluateWithOption(SemverToken.Options.Default, null, _evaluator);

            // Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("context");
        }

        [Fact]
        public void Process_NullEvaluator_Throws()
        {
            // Arrange
            Action action = () => _sut.EvaluateWithOption(SemverToken.Options.Default, _context, null);

            // Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("evaluator");
        }

        [Fact]
        public void Process_NullOption_Throws()
        {
            // Arrange
            Action action = () => _sut.EvaluateWithOption(null, _context, _evaluator);

            // Act / Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("optionValue");
        }

        [Theory]
        [InlineData("")]
        [InlineData("\t\t   ")]
        [InlineData("12")]
        public void Process_InvalidOption_Throws(string optionValue)
        {
            // Arrange
            Action action = () => _sut.EvaluateWithOption(optionValue, _context, _evaluator);

            // Act / Assert
            action.Should().Throw<InvalidOperationException>()
                .WithMessage($"'{optionValue}' is not a valid semver version");
        }

        [Theory]
        [InlineData("1", "-")]
        [InlineData("2", ".")]
        public void Process_AllowedOption_UsesExpectedDelimiter(string optionValue, string delimiter)
        {
            // Arrange
            _context.Configuration = new Configuration.VersionConfiguration();

            // Act
            _sut.EvaluateWithOption(optionValue, _context, _evaluator);

            // Assert
            _evaluator.Received(1).Process<LabelToken>(delimiter, _context);
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
                .WithMessage($"'{optionValue}' is not a valid semver version");
        }

        [Theory]
        [InlineData("1", "-")]
        [InlineData("2", ".")]
        public void Process_HasLabel_ReturnsLabel(string optionValue, string delimiter)
        {
            // Arrange
            var version = "1.2.3";
            var label = "somelabel";
            var height = "10";
            _context.Configuration = new Configuration.VersionConfiguration
            {
                Label = { label },
                Version = version
            };

            _evaluator.Process<VersionToken>(_context).Returns(version);
            _evaluator.Process<HeightToken>(Arg.Any<string>(), _context).Returns(height);
            _evaluator.Process<LabelToken>(delimiter, _context).Returns(label);
            _evaluator.Process<MetadataToken>(_context).Returns((string)null);

            var expected = $"{version}-{label}{delimiter}{height}";

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
            _evaluator.Process<VersionToken>(_context).Returns(version);
            _evaluator.Process<LabelToken>(delimiter, _context).Returns(label);

            // Act
            var result = _sut.EvaluateWithOption(optionValue, _context, _evaluator);

            // Assert
            result.Should().Be(version);
        }

        [Fact]
        public void Process_Semver1_DoesNotProcessMetadata()
        {
            // Arrange
            _evaluator.Process<VersionToken>(_context).Returns((string)null);
            _evaluator.Process<HeightToken>(Arg.Any<string>(), _context).Returns((string)null);
            _evaluator.Process<LabelToken>(Arg.Any<string>(), _context).Returns((string)null);

            // Act
            _sut.EvaluateWithOption("1", _context, _evaluator);

            // Assert
            _evaluator.DidNotReceive().Process<MetadataToken>(_context);
        }

        [Fact]
        public void Process_HasLabelAndMetadata_ReturnsVersionLabelAndMetadata()
        {
            // Arrange
            var version = "1.2.3";
            var label = "somelabel";
            var meta = "some.meta";
            var height = "10";
            _context.Configuration = new Configuration.VersionConfiguration
            {
                Label = { label },
                Metadata = { meta },
                Version = version
            };

            _evaluator.Process<VersionToken>(_context).Returns(version);
            _evaluator.Process<HeightToken>(HeightToken.Options.Default, _context).Returns(height);
            _evaluator.Process<LabelToken>(LabelToken.Options.Default, _context).Returns(label);
            _evaluator.Process<MetadataToken>(_context).Returns(meta);

            var expected = $"{version}-{label}.{height}+{meta}";

            // Act
            var result = _sut.EvaluateWithOption(SemverToken.Options.Semver2, _context, _evaluator);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void Process_NoLabelIncludesMetadata_ReturnsVersionAndMetadata()
        {
            // Arrange
            var version = "1.2.3";
            var meta = "some.meta";
            _context.Configuration = new Configuration.VersionConfiguration
            {
                Metadata = { meta },
                Version = version
            };

            _evaluator.Process<VersionToken>(_context).Returns(version);
            _evaluator.Process<LabelToken>(LabelToken.Options.Default, _context).Returns((string)null);
            _evaluator.Process<MetadataToken>(_context).Returns(meta);

            // Act
            var result = _sut.EvaluateWithOption(SemverToken.Options.Semver2, _context, _evaluator);

            // Assert
            result.Should().Be($"{version}+{meta}");
        }
    }
}
