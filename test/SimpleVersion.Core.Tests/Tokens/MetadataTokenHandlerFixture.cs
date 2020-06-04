// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using FluentAssertions;
using NSubstitute;
using SimpleVersion.Configuration;
using SimpleVersion.Pipeline;
using SimpleVersion.Tokens;
using Xunit;

namespace SimpleVersion.Core.Tests.Tokens
{
    public class MetadataTokenHandlerFixture
    {
        private readonly MetadataTokenHandler _sut;
        private readonly IVersionContext _context;
        private readonly ITokenEvaluator _evaluator;

        public MetadataTokenHandlerFixture()
        {
            _sut = new MetadataTokenHandler();
            _context = Substitute.For<IVersionContext>();
            _context.Result.Returns(new VersionResult());
            _evaluator = Substitute.For<ITokenEvaluator>();
            _evaluator.Process(Arg.Any<string>(), Arg.Any<IVersionContext>())
                .Returns(call => call.Arg<string>());
        }

        [Fact]
        public void Ctor_SetsKey()
        {
            // Act / Assert
            _sut.Key.Should().Be("metadata");
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
            // Arrange
            var config = new VersionConfiguration
            {
                Metadata = { "alpha", "beta", "gamma" }
            };
            _context.Configuration.Returns(config);

            // Act
            var result = _sut.Process(null, _context, _evaluator);

            // Assert
            result.Should().Be("alpha.beta.gamma");
        }

        [Theory]
        [InlineData("")]
        [InlineData("\t\t  ")]
        public void Process_WhitespaceOption_ReturnsJoined(string option)
        {
            // Arrange
            var config = new VersionConfiguration
            {
                Metadata = { "alpha", "beta", "gamma" }
            };
            _context.Configuration.Returns(config);

            var expected = string.Join(option, config.Metadata);

            // Act
            var result = _sut.Process(option, _context, _evaluator);

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(".thi")]
        [InlineData("-")]
        [InlineData("test")]
        public void Process_StringOption_ReturnsJoined(string option)
        {
            // Arrange
            var config = new VersionConfiguration
            {
                Metadata = { "alpha", "beta", "gamma" }
            };
            _context.Configuration.Returns(config);

            var expected = string.Join(option, config.Metadata);

            // Act
            var result = _sut.Process(option, _context, _evaluator);

            // Assert
            result.Should().Be(expected);
        }
    }
}
