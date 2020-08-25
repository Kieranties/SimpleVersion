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
        private readonly BranchNameTokenRequest _request;
        private readonly IVersionContext _context;
        private readonly ITokenEvaluator _evaluator;

        public BranchNameTokenFixture()
        {
            _sut = new BranchNameToken();
            _request = new BranchNameTokenRequest();
            _context = Substitute.For<IVersionContext>();
            _context.Result.Returns(new VersionResult());
            _evaluator = Substitute.For<ITokenEvaluator>();
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

            // Act / Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("context");
        }

        [Fact]
        public void Evaluate_NullEvaluator_DoesNotThrow()
        {
            // Arrange
            _context.Result.CanonicalBranchName = "test";
            Action action = () => _sut.Evaluate(_request, _context, null);

            // Act / Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void Evaluate_NoBranchNameSet_Throws()
        {
            // Arrange
            _context.Result.CanonicalBranchName = null;
            _context.Result.BranchName = null;
            Action action = () => _sut.Evaluate(_request, _context, null);

            // Act / Assert
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("Branch name has not been set.");
        }

        [Fact]
        public void Evaluate_InvalidOption_Throws()
        {
            // Arrange
            _request.BranchName = (BranchNameOption)50;

            Action action = () => _sut.Evaluate(_request, _context, _evaluator);

            // Act / Assert
            action.Should().Throw<InvalidOperationException>()
                .WithMessage($"Invalid option '{_request.BranchName}'");
        }

        [Theory]
        [InlineData("refs/heads/master", "refsheadsmaster")]
        [InlineData("refs/heads/release/1.0", "refsheadsrelease10")]
        [InlineData("refs/heads/release-1.0", "refsheadsrelease10")]
        public void Evaluate_Canonical_ReturnsFormattedBranchName(string branch, string expected)
        {
            // Arrange
            _context.Result.CanonicalBranchName = branch;
            _request.BranchName = BranchNameOption.Canonical;

            // Act
            var result = _sut.Evaluate(_request, _context, _evaluator);

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("refs/heads/master", "master")]
        [InlineData("refs/heads/release/1.0", "10")]
        [InlineData("refs/heads/release-1.0", "release10")]
        public void Evaluate_Suffix_ReturnsFormattedSuffix(string branch, string expected)
        {
            // Arrange
            _context.Result.CanonicalBranchName = branch;
            _request.BranchName = BranchNameOption.Suffix;

            // Act
            var result = _sut.Evaluate(_request, _context, _evaluator);

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("master", "master")]
        [InlineData("release/1.0", "release10")]
        [InlineData("release-1.0", "release10")]
        public void Evaluate_Short_ReturnsFormattedBranchName(string branch, string expected)
        {
            // Arrange
            _context.Result.BranchName = branch;
            _request.BranchName = BranchNameOption.Short;

            // Act
            var result = _sut.Evaluate(_request, _context, _evaluator);

            // Assert
            result.Should().Be(expected);
        }
    }
}
