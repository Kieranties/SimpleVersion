// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using FluentAssertions;
using SimpleVersion.Model;
using Xunit;

namespace SimpleVersion.Abstractions.Tests.Model
{
    public class VersionResultFixture
    {
        [Fact]
        public void Ctor_SetsDefaults()
        {
            // Arrange / Act
            var sut = new VersionResult();

            // Assert
            sut.Version.Should().BeNull();
            sut.Major.Should().Be(0);
            sut.Minor.Should().Be(0);
            sut.Patch.Should().Be(0);
            sut.Revision.Should().Be(0);
            sut.Height.Should().Be(0);
            sut.HeightPadded.Should().Be("0000");
            sut.Sha.Should().BeNull();
            sut.Sha7.Should().BeNull();
            sut.BranchName.Should().BeNull();
            sut.CanonicalBranchName.Should().BeNull();
            sut.IsPullRequest.Should().BeFalse();
            sut.PullRequestNumber.Should().Be(0);
            sut.RepositoryPath.Should().BeNull();
            sut.Formats.Should().BeEmpty();
        }

        [Fact]
        public void HeightPadded_Pads_Height()
        {
            // Arrange / Act
            var sut = new VersionResult { Height = 40 };

            // Assert
            sut.HeightPadded.Should().Be("0040");
        }

        [Fact]
        public void PR_Branch_Sets_PR_Properties()
        {
            // Arrange / Act
            var sut = new VersionResult { CanonicalBranchName = "refs/pull/124876/merge" };

            // Assert
            sut.IsPullRequest.Should().BeTrue();
            sut.PullRequestNumber.Should().Be(124876);
        }

        [Fact]
        public void Non_PR_Branch_Does_Not_Set_PR_Properties()
        {
            // Arrange / Act
            var sut = new VersionResult { CanonicalBranchName = "refs/heads/master" };

            // Assert
            sut.IsPullRequest.Should().BeFalse();
            sut.PullRequestNumber.Should().Be(0);
        }

        [Fact]
        public void Formats_Has_CaseInsensitiveKeys()
        {
            // Arrange
            var key = "This is the KEY";
            var value = "This is the value";
            var sut = new VersionResult
            {
                Formats =
                {
                    [key] = value
                }
            };

            // Act
            var upperResult = sut.Formats[key.ToUpper(System.Globalization.CultureInfo.CurrentCulture)];
            var lowerResult = sut.Formats[key.ToLower(System.Globalization.CultureInfo.CurrentCulture)];

            // Assert
            upperResult.Should().Be(value);
            lowerResult.Should().Be(value);
        }
    }
}
