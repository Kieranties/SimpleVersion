// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System.Security;
using FluentAssertions;
using Xunit;

namespace SimpleVersion.Abstractions.Tests
{
    public class VersionResultFixture
    {
        [Fact]
        public void Ctor_SetsDefaults()
        {
            // Arrange / Act
            var sut = new VersionResult();

            // Assert
            sut.HeightPadded.Should().Be("0000");
            sut.Sha7.Should().BeNull();
            sut.IsPullRequest.Should().BeFalse();
            sut.PullRequestNumber.Should().Be(0);
            sut.Formats.Should().BeEmpty();
            AssertUtils.AssertGetSetProperty(sut, nameof(sut.Version), x => x.Should().BeNull(), "1.2.3.4");
            AssertUtils.AssertGetSetProperty(sut, nameof(sut.Major), x => x.Should().Be(0), 10);
            AssertUtils.AssertGetSetProperty(sut, nameof(sut.Minor), x => x.Should().Be(0), 10);
            AssertUtils.AssertGetSetProperty(sut, nameof(sut.Patch), x => x.Should().Be(0), 10);
            AssertUtils.AssertGetSetProperty(sut, nameof(sut.Revision), x => x.Should().Be(0), 10);
            AssertUtils.AssertGetSetProperty(sut, nameof(sut.Height), x => x.Should().Be(0), 10);
            AssertUtils.AssertGetSetProperty(sut, nameof(sut.Sha), x => x.Should().BeNull(), "1234");
            AssertUtils.AssertGetSetProperty(sut, nameof(sut.BranchName), x => x.Should().BeNull(), "master");
            AssertUtils.AssertGetSetProperty(sut, nameof(sut.CanonicalBranchName), x => x.Should().BeNull(), "refs/heads/master");
            AssertUtils.AssertGetSetProperty(sut, nameof(sut.RepositoryPath), x => x.Should().BeNull(), "test");
            AssertUtils.AssertGetSetProperty(sut, nameof(sut.IsRelease), x => x.Should().BeFalse(), true);
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

        [Theory]
        [InlineData(null, null)]
        [InlineData("1234", "1234")]
        [InlineData("1234567890", "1234567")]
        public void Sha7_IsSubstring_OfSha(string sha, string sha7)
        {
            // Arrange / Act
            var sut = new VersionResult
            {
                Sha = sha
            };

            // Assert
            sut.Sha7.Should().Be(sha7);
        }
    }
}
