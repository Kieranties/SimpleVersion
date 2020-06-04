// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using FluentAssertions;
using NSubstitute;
using SimpleVersion.Pipeline;
using SimpleVersion.Tokens;
using Xunit;

namespace SimpleVersion.Core.Tests.Tokens
{
    public class BranchNameTokenHandlerFixture
    {
        private readonly BranchNameTokenHandler _sut;
        private readonly IVersionContext _context;
        private readonly ITokenEvaluator _evaluator;

        public BranchNameTokenHandlerFixture()
        {
            _sut = new BranchNameTokenHandler();
            _context = Substitute.For<IVersionContext>();
            _context.Result.Returns(new VersionResult());
            _evaluator = Substitute.For<ITokenEvaluator>();
        }

        [Fact]
        public void Ctor_SetsKey()
        {
            // Act / Assert
            _sut.Key.Should().Be("branchname");
        }

        [Fact]
        public void Process_NullContext_Throws()
        {
            // Arrange
            Action action = () => _sut.Process(null, null, _evaluator);

            // Act / Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("context");
        }

        [Fact]
        public void Process_NullEvaluator_DoesNotThrow()
        {
            // Arrange
            _context.Result.CanonicalBranchName = "test";
            Action action = () => _sut.Process(null, _context, null);

            // Act / Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void Process_NoBranchNameSet_Throws()
        {
            // Arrange
            _context.Result.CanonicalBranchName = null;
            _context.Result.BranchName = null;
            Action action = () => _sut.Process(null, _context, null);

            // Act / Assert
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("Branch name has not been set.");
        }

        [Theory]
        [InlineData("", "refs/heads/master", "refsheadsmaster")] // Ignore spelling: refsheadsmaster
        [InlineData("  ", "refs/heads/master", "refsheadsmaster")]
        [InlineData("\t\t  ", "refs/heads/release/1.0", "refsheadsrelease10")]
        [InlineData("random string", "refs/heads/release-1.0", "refsheadsrelease10")]
        public void Process_InvalidOption_ReturnsFormattedCanonical(string option, string branch, string expected)
        {
            // Arrange
            _context.Result.CanonicalBranchName = branch;

            // Act
            var result = _sut.Process(option, _context, _evaluator);

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("suffix", "refs/heads/master", "master")]
        [InlineData("SUFFIX", "refs/heads/master", "master")]
        [InlineData("Suffix", "refs/heads/release/1.0", "10")]
        [InlineData("SuFfIx", "refs/heads/release-1.0", "release10")]
        public void Process_Suffix_ReturnsFormattedSuffix(string option, string branch, string expected)
        {
            // Arrange
            _context.Result.CanonicalBranchName = branch;

            // Act
            var result = _sut.Process(option, _context, _evaluator);

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("short", "master", "master")]
        [InlineData("SHORT", "master", "master")]
        [InlineData("Short", "release/1.0", "release10")]
        [InlineData("ShOrT", "release-1.0", "release10")]
        public void Process_Short_ReturnsFormattedBranchName(string option, string branch, string expected)
        {
            // Arrange
            _context.Result.BranchName = branch;

            // Act
            var result = _sut.Process(option, _context, _evaluator);

            // Assert
            result.Should().Be(expected);
        }
    }
}
