// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using FluentAssertions;
using SimpleVersion.Model;
using System.Collections.Generic;
using Xunit;

namespace SimpleVersion.Abstractions.Tests.Model
{
    public class ConfigurationFixture
    {
        [Fact]
        public void Ctor_SetsDefaults()
        {
            // Arrange / Act
            var sut = new Configuration();

            // Assert
            sut.Label.Should().BeEmpty();
            sut.MetaData.Should().BeEmpty();
            sut.Version.Should().BeEmpty();
            sut.OffSet.Should().Be(0);
            sut.Branches.Should().NotBeNull();
        }

        [Fact]
        public void Equals_GivenNull_ReturnsFalse()
        {
            // Arrange
            var sut = new Configuration();

            // Act
            var result = sut.Equals(null);

            // Assert
            result.Should().BeFalse();
        }

        public static string Thing => "this";

        public static IEnumerable<object[]> DifferInLabel()
        {
            yield return new[] { new Configuration(), new Configuration { Label = { "one" } } };
            yield return new[] { new Configuration { Label = { "one" } }, new Configuration() };
            yield return new[] { new Configuration { Label = { "one", "two" } }, new Configuration { Label = { "one", "three" } } };
        }

        [Theory]
        [MemberData(nameof(DifferInLabel))]
        public void Equals_DifferInLabel_ReturnsTrue(Configuration sut, Configuration other)
        {
            // Arrange / Act
            var result = sut.Equals(other);

            // Assert
            result.Should().BeFalse();
        }
    }
}
