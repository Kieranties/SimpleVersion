using Xunit;
using FluentAssertions;
using SimpleVersion.Model;

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
            sut.Version.Should().BeEmpty();
            sut.Major.Should().Be(0);
            sut.Minor.Should().Be(0);
            sut.Patch.Should().Be(0);
            sut.Revision.Should().Be(0);
            sut.Height.Should().Be(0);
            sut.HeightPadded.Should().Be("0000");
            sut.Sha.Should().BeEmpty();
            sut.BranchName.Should().BeEmpty();
            sut.CanonicalBranchName.Should().BeEmpty();
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
        public void Formats_Has_CaseInsensitiveKeys()
        {
            // Arrange
            var key = "This is the KEY";
            var value = "This is the value";
            var sut = new VersionResult
            {
                Formats = {
                    [key] = value
                }
            };

            // Act
            var upperResult = sut.Formats[key.ToUpper()];
            var lowerResult = sut.Formats[key.ToLower()];

            // Assert
            upperResult.Should().Be(value);
            lowerResult.Should().Be(value);
        }
    }
}
