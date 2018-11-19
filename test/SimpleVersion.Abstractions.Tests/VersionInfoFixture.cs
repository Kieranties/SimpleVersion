using FluentAssertions;
using System.Collections.Generic;
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

        [Fact]
        public void Equals_GivenNull_ReturnsFalse()
        {
            // Arrange
            var sut = new VersionInfo();

            // Act
            var result = sut.Equals(null);

            // Assert
            result.Should().BeFalse();
        }

        public static string Thing => "this";

        public static IEnumerable<object[]> DifferInLabel()
        {
            yield return new[] { new VersionInfo(), new VersionInfo { Label = { "one" } } };
            yield return new[] { new VersionInfo { Label = { "one" } }, new VersionInfo() };
            yield return new[] { new VersionInfo { Label = { "one", "two" } }, new VersionInfo { Label = { "one", "three" } } };
        }

        [Theory]
        [MemberData(nameof(DifferInLabel))]
        public void Equals_DifferInLabel_ReturnsTrue(VersionInfo sut, VersionInfo other)
        {
            // Arrange / Act
            var result = sut.Equals(other);

            // Assert
            result.Should().BeFalse();
        }
    }
}
