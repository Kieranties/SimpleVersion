// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System.Collections.Generic;
using FluentAssertions;
using SimpleVersion.Configuration;
using Xunit;

namespace SimpleVersion.Abstractions.Tests.Configuration
{
    public class VersionConfigurationFixture
    {
        [Fact]
        public void Ctor_SetsDefaults()
        {
            // Arrange / Act
            var sut = new VersionConfiguration();

            // Assert
            AssertUtils.AssertGetSetProperty(sut, nameof(sut.Version), x => x.Should().BeEmpty(), "test");
            AssertUtils.AssertGetSetProperty(sut, nameof(sut.OffSet), x => x.Should().Be(0), 50);
            AssertUtils.AssertGetSetProperty(sut, nameof(sut.Label), x => x.Should().BeEmpty(), new List<string> { "test" });
            AssertUtils.AssertGetSetProperty(sut, nameof(sut.Metadata), x => x.Should().BeEmpty(), new List<string> { "test" });
        }

        [Fact(Skip = "Needs redesign")]
        public void Equals_GivenNull_ReturnsFalse()
        {
            // Arrange
            var sut = new VersionConfiguration();

            // Act
            var result = sut.Equals(null);

            // Assert
            result.Should().BeFalse();
        }

        public static IEnumerable<object[]> DifferInLabel()
        {
            yield return new[] { new VersionConfiguration(), new VersionConfiguration { Label = { "one" } } };
            yield return new[] { new VersionConfiguration { Label = { "one" } }, new VersionConfiguration() };
            yield return new[] { new VersionConfiguration { Label = { "one", "two" } }, new VersionConfiguration { Label = { "one", "three" } } };
        }

        [Theory(Skip = "Needs redesign")]
        [MemberData(nameof(DifferInLabel))]
        public void Equals_DifferInLabel_ReturnsTrue(VersionConfiguration sut, VersionConfiguration other)
        {
            // Arrange / Act
            var result = sut.Equals(other);

            // Assert
            result.Should().BeFalse();
        }
    }
}
