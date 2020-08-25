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
    public class LabelTokenFixture
    {
        private readonly LabelToken _sut;
        private readonly LabelTokenRequest _request;
        private readonly IVersionContext _context;
        private readonly ITokenEvaluator _evaluator;

        public LabelTokenFixture()
        {
            _sut = new LabelToken();
            _request = new LabelTokenRequest();
            _context = Substitute.For<IVersionContext>();
            _context.Result.Returns(new VersionResult());
            _context.Result.IsRelease = true;
            _evaluator = Substitute.For<ITokenEvaluator>();
            _evaluator.Parse(Arg.Any<string>(), Arg.Any<IVersionContext>())
                .Returns(call => call.Arg<string>());
        }

        [Fact]
        public void Evaluate_NullRequest_Throws()
        {
            // Arrange
            Action action = () => _sut.Evaluate(null, _context, _evaluator);

            // Assert
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

        [Fact]
        public void Evaluate_DefaultOption_UsesDefault()
        {
            // Arrange
            var config = new VersionConfiguration
            {
                Label = { "alpha", "beta", "gamma" }
            };
            _context.Configuration.Returns(config);

            // Act
            var result = _sut.Evaluate(_request, _context, _evaluator);

            // Assert
            result.Should().Be("alpha.beta.gamma");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t\t  ")]
        public void Evaluate_WhitespaceOption_ReturnsJoined(string option)
        {
            // Arrange
            var config = new VersionConfiguration
            {
                Label = { "alpha", "beta", "gamma" }
            };
            _context.Configuration.Returns(config);
            _request.Separator = option;

            var expected = string.Join(option, config.Label);

            // Act
            var result = _sut.Evaluate(_request, _context, _evaluator);

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(".thi")]
        [InlineData("-")]
        [InlineData("test")]
        public void Evaluate_StringOption_ReturnsJoined(string option)
        {
            // Arrange
            var config = new VersionConfiguration
            {
                Label = { "alpha", "beta", "gamma" }
            };
            _context.Configuration.Returns(config);
            _request.Separator = option;

            var expected = string.Join(option, config.Label);

            // Act
            var result = _sut.Evaluate(_request, _context, _evaluator);

            // Assert
            result.Should().Be(expected);
        }
    }
}
