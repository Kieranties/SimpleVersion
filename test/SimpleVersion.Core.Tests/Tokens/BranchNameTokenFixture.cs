// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using FluentAssertions;
using NSubstitute;
using SimpleVersion.Pipeline;
using SimpleVersion.Tokens;
using Xunit;

namespace SimpleVersion.Core.Tests.Tokens
{
    public class BranchNameTokenFixture
    {
        private readonly BranchNameToken _sut;
        private readonly IVersionContext _context;
        private readonly ITokenEvaluator _evaluator;

        public BranchNameTokenFixture()
        {
            _sut = new BranchNameToken();
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
            Action action = () => _sut.EvaluateWithOption(BranchNameToken.Options.Default, null, _evaluator);

            // Act / Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("context");
        }

        [Fact]
        public void Process_NullEvaluator_DoesNotThrow()
        {
            // Arrange
            _context.Result.CanonicalBranchName = "test";
            Action action = () => _sut.EvaluateWithOption(BranchNameToken.Options.Default, _context, null);

            // Act / Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void Process_NoBranchNameSet_Throws()
        {
            // Arrange
            _context.Result.CanonicalBranchName = null;
            _context.Result.BranchName = null;
            Action action = () => _sut.EvaluateWithOption(BranchNameToken.Options.Default, _context, null);

            // Act / Assert
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("Branch name has not been set.");
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData("\t\t  ")]
        [InlineData("random string")]
        public void Process_InvalidOption_Throws(string option)
        {
            // Arrange
            Action action = () => _sut.EvaluateWithOption(option, _context, _evaluator);

            // Act / Assert
            action.Should().Throw<InvalidOperationException>()
                .WithMessage($"Invalid option '{option}' for token 'branchname'");
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
            var result = _sut.EvaluateWithOption(option, _context, _evaluator);

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
            var result = _sut.EvaluateWithOption(option, _context, _evaluator);

            // Assert
            result.Should().Be(expected);
        }
    }
}
