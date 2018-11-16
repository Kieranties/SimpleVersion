using FluentAssertions;
using Xunit;

namespace SimpleVersion.Abstractions.Tests
{
    public class VersionInfoFixture
    {
        [Fact]
        public void Ctor_SetsDefaults()
        {
            // Arrange / Act
            var sut = new VersionInfo();

            // Assert
            sut.Label.Should().BeEmpty();
            sut.MetaData.Should().BeEmpty();
            sut.Version.Should().BeEmpty();
        }
    }
}
