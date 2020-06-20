// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using FluentAssertions;
using NSubstitute;
using SimpleVersion.Pipeline;
using SimpleVersion.Tokens;
using Xunit;

namespace SimpleVersion.Core.Tests.Tokens
{
    public class VersionTokenFixture
    {
        private readonly VersionToken _sut;
        private readonly IVersionContext _context;
        private readonly ITokenEvaluator _evaluator;

        public VersionTokenFixture()
        {
            _sut = new VersionToken();
            _context = Substitute.For<IVersionContext>();
            _context.Result.Returns(new VersionResult());
            _evaluator = Substitute.For<ITokenEvaluator>();

        }

        [Fact]
        public void Ctor_SetsKey()
        {
            // Act / Assert
            _sut.Key.Should().Be("version");
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
        [InlineData(VersionToken.Options.Default, 1, 1, 1, 0, "1.1.1")]
        [InlineData(VersionToken.Options.Default, 1, 0, 0, 0, "1.0.0")]
        [InlineData(VersionToken.Options.Default, 1, 0, 1, 0, "1.0.1")]
        [InlineData(VersionToken.Options.Default, 1, 1, 1, 1, "1.1.1.1")]
        [InlineData(VersionToken.Options.Default, 1, 0, 0, 2, "1.0.0.2")]
        [InlineData(VersionToken.Options.Default, 1, 0, 1, 3, "1.0.1.3")]
        [InlineData("MmpR", 1, 0, 1, 0, "1.0.1.0")]
        [InlineData("MmpR", 1, 0, 1, 9, "1.0.1.9")]
        [InlineData("M", 9, 0, 1, 8, "9")]
        [InlineData("m", 0, 9, 1, 8, "9")]
        [InlineData("p", 0, 0, 9, 8, "9")]
        [InlineData("r", 0, 0, 1, 9, "9")]
        [InlineData("r", 0, 0, 1, 0, "")]
        [InlineData("R", 0, 0, 1, 9, "9")]
        [InlineData("R", 1, 1, 1, 0, "0")]
        [InlineData("MMMM", 1, 0, 0, 0, "1.1.1.1")]
        public void Process_OptionValue_ReturnsExpected(string optionValue, int major, int minor, int patch, int revision, string expected)
        {
            // Arrange
            var version = new VersionResult
            {
                Major = major,
                Minor = minor,
                Patch = patch,
                Revision = revision
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
            Action action = () => _sut.EvaluateWithOption("1.2.3.4", _context, _evaluator);

            // Act / Assert
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("Invalid character '1' in version option [1.2.3.4].");
        }
    }
}
