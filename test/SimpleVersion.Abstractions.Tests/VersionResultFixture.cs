using Xunit;
using FluentAssertions;


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
            sut.Major.Should().Be(0);
            sut.Minor.Should().Be(0);
            sut.Patch.Should().Be(0);
            sut.Revision.Should().Be(0);
            sut.Height.Should().Be(0);
            sut.HeightPadded.Should().Be("0000");
            sut.Sha.Should().BeEmpty();
            sut.BranchName.Should().BeEmpty();
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
    }
}
