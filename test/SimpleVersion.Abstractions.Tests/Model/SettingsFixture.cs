// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using FluentAssertions;
using SimpleVersion.Model;
using System.Collections.Generic;
using Xunit;

namespace SimpleVersion.Abstractions.Tests.Model
{
    public class SettingsFixture
    {
        [Fact]
        public void Ctor_SetsDefaults()
        {
            // Arrange / Act
            var sut = new Settings();

            // Assert
            sut.Label.Should().BeEmpty();
            sut.Metadata.Should().BeEmpty();
            sut.Version.Should().BeEmpty();
            sut.OffSet.Should().Be(0);
            sut.Branches.Should().NotBeNull();
        }

        [Fact]
        public void Equals_GivenNull_ReturnsFalse()
        {
            // Arrange
            var sut = new Settings();

            // Act
            var result = sut.Equals(null);

            // Assert
            result.Should().BeFalse();
        }

        public static IEnumerable<object[]> DifferInLabel()
        {
            yield return new[] { new Settings(), new Settings { Label = { "one" } } };
            yield return new[] { new Settings { Label = { "one" } }, new Settings() };
            yield return new[] { new Settings { Label = { "one", "two" } }, new Settings { Label = { "one", "three" } } };
        }

        [Theory]
        [MemberData(nameof(DifferInLabel))]
        public void Equals_DifferInLabel_ReturnsTrue(Settings sut, Settings other)
        {
            // Arrange / Act
            var result = sut.Equals(other);

            // Assert
            result.Should().BeFalse();
        }
    }
}
