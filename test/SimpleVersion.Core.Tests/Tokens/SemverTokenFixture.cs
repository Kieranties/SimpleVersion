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
        private readonly SemverTokenRequest _request;
        private readonly IVersionContext _context;
        private readonly ITokenEvaluator _evaluator;

        public SemverTokenFixture()
        {
            _sut = new SemverToken();
            _request = new SemverTokenRequest();
            _context = Substitute.For<IVersionContext>();
            _context.Result.Returns(new VersionResult());
            _evaluator = Substitute.For<ITokenEvaluator>();
            _evaluator.Process(Arg.Any<VersionTokenRequest>(), _context).Returns("version");
            _evaluator.Process(Arg.Any<LabelTokenRequest>(), _context).Returns("label");
            _context.Result.IsRelease = true;
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

            // Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("context");
        }

        [Fact]
        public void Evaluate_NullEvaluator_Throws()
        {
            // Arrange
            Action action = () => _sut.Evaluate(_request, _context, null);

            // Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("evaluator");
        }

        [Theory]
        [InlineData(1, "-")]
        [InlineData(2, ".")]
        public void Evaluate_AllowedOption_UsesExpectedDelimiter(int option, string delimiter)
        {
            // Arrange
            _context.Configuration = new Configuration.VersionConfiguration();
            _request.Version = option;

            // Act
            _sut.Evaluate(_request, _context, _evaluator);

            // Assert
            _evaluator.Received(1).Process(Arg.Is<LabelTokenRequest>(t => t.Separator == delimiter), _context);
        }

        [Theory]
        [InlineData(1, "-")]
        [InlineData(2, ".")]
        public void Evaluate_HasLabel_ReturnsLabel(int option, string delimiter)
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
            _request.Version = option;

            _evaluator.Process(Arg.Any<VersionTokenRequest>(), _context).Returns(version);
            _evaluator.Process(Arg.Any<HeightTokenRequest>(), _context).Returns(height);
            _evaluator.Process(Arg.Any<LabelTokenRequest>(), _context).Returns(label);
            _evaluator.Process(Arg.Any<MetadataTokenRequest>(), _context).Returns((string)null);

            var expected = $"{version}-{label}{delimiter}{height}";

            // Act
            var result = _sut.Evaluate(_request, _context, _evaluator);

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void Evaluate_NoLabel_ReturnsOnlyVersion(int option)
        {
            // Arrange
            string label = null;
            var version = "1.2.3";
            _request.Version = option;

            _evaluator.Process(Arg.Any<VersionTokenRequest>(), _context).Returns(version);
            _evaluator.Process(Arg.Any<LabelTokenRequest>(), _context).Returns(label);

            // Act
            var result = _sut.Evaluate(_request, _context, _evaluator);

            // Assert
            result.Should().Be(version);
        }

        [Fact]
        public void Evaluate_Semver1_DoesNotProcessMetadata()
        {
            // Arrange
            _evaluator.Process(Arg.Any<VersionTokenRequest>(), _context).Returns((string)null);
            _evaluator.Process(Arg.Any<HeightTokenRequest>(), _context).Returns((string)null);
            _evaluator.Process(Arg.Any<LabelTokenRequest>(), _context).Returns((string)null);
            _request.Version = 1;

            // Act
            _sut.Evaluate(_request, _context, _evaluator);

            // Assert
            _evaluator.DidNotReceive().Process(Arg.Any<MetadataTokenRequest>(), _context);
        }

        [Fact]
        public void Evaluate_HasLabelAndMetadata_ReturnsVersionLabelAndMetadata()
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
            _request.Version = 2;

            _evaluator.Process(Arg.Any<VersionTokenRequest>(), _context).Returns(version);
            _evaluator.Process(Arg.Any<HeightTokenRequest>(), _context).Returns(height);
            _evaluator.Process(Arg.Any<LabelTokenRequest>(), _context).Returns(label);
            _evaluator.Process(Arg.Any<MetadataTokenRequest>(), _context).Returns(meta);


            var expected = $"{version}-{label}.{height}+{meta}";

            // Act
            var result = _sut.Evaluate(_request, _context, _evaluator);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void Evaluate_NoLabelIncludesMetadata_ReturnsVersionAndMetadata()
        {
            // Arrange
            var version = "1.2.3";
            var meta = "some.meta";
            _context.Configuration = new Configuration.VersionConfiguration
            {
                Metadata = { meta },
                Version = version
            };
            _request.Version = 2;

            _evaluator.Process(Arg.Any<VersionTokenRequest>(), _context).Returns(version);
            _evaluator.Process(Arg.Any<LabelTokenRequest>(), _context).Returns((string)null);
            _evaluator.Process(Arg.Any<MetadataTokenRequest>(), _context).Returns(meta);

            // Act
            var result = _sut.Evaluate(_request, _context, _evaluator);

            // Assert
            result.Should().Be($"{version}+{meta}");
        }
    }
}
