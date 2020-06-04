// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using FluentAssertions;
using NSubstitute;
using SimpleVersion.Configuration;
using SimpleVersion.Pipeline;
using SimpleVersion.Pipeline.Formatting;
using SimpleVersion.Tokens;
using Xunit;

namespace SimpleVersion.Core.Tests.Pipeline
{
    public class VersionVersionProcessorFixture
    {
        private readonly ITokenEvaluator _evaluator;
        private readonly IVersionContext _context;

        public VersionVersionProcessorFixture()
        {
            _evaluator = Substitute.For<ITokenEvaluator>();
            _evaluator.Process(Arg.Any<string>(), Arg.Any<IVersionContext>())
                .Returns(call => call.Arg<string>());
            _context = Substitute.For<IVersionContext>();
            _context.Result.Returns(new VersionResult());
            _context.Configuration.Returns(new VersionConfiguration());
        }

        [Fact]
        public void Ctor_NullEvaluator_Throws()
        {
            // Arrange
            Action action = () => new VersionVersionProcessor(null);

            // Act / Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("evaluator");
        }

        [Fact]
        public void Process_NullContext_Throws()
        {
            // Arrange
            var sut = new VersionVersionProcessor(_evaluator);
            Action action = () => sut.Process(null);

            // Act / Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("context");
        }

        [Theory]
        [InlineData("1")]
        [InlineData("1.2.=")]
        [InlineData("1.l.4")]
        [InlineData("*.l.5")]
        public void Process_InvalidVersion_Throws(string version)
        {
            // Arrange
            var sut = new VersionVersionProcessor(_evaluator);
            _context.Configuration.Version = version;

            Action action = () => sut.Process(_context);

            // Act / Assert
            action.Should().Throw<InvalidOperationException>()
                .WithMessage($"Version '{version}' is not in a valid format.");
        }

        [Theory]
        [InlineData("1.0", "1.0.0", 1, 0, 0, 0)]
        [InlineData("1.0.0", "1.0.0", 1, 0, 0, 0)]
        [InlineData("1.2.0", "1.2.0", 1, 2, 0, 0)]
        [InlineData("1.2.3", "1.2.3", 1, 2, 3, 0)]
        [InlineData("14.22.32.234", "14.22.32.234", 14, 22, 32, 234)]
        public void Process_ValidVersion_Returns(
            string version,
            string expectedVersion,
            int major,
            int minor,
            int patch,
            int revision)
        {
            // Arrange
            var sut = new VersionVersionProcessor(_evaluator);
            _context.Configuration.Version = version;

            // Act
            sut.Process(_context);

            // Assert
            //_context.Result.Version.Should().Be(expectedVersion);
            _context.Result.Major.Should().Be(major);
            _context.Result.Minor.Should().Be(minor);
            _context.Result.Patch.Should().Be(patch);
            _context.Result.Revision.Should().Be(revision);
        }
    }
}
