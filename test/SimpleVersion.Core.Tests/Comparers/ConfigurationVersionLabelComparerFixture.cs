// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Collections.Generic;
using FluentAssertions;
using SimpleVersion.Comparers;
using SimpleVersion.Model;
using Xunit;

namespace SimpleVersion.Core.Tests.Comparers
{
    public class ConfigurationVersionLabelComparerFixture
    {
        private readonly ConfigurationVersionLabelComparer _sut;

        public ConfigurationVersionLabelComparerFixture()
        {
            _sut = new ConfigurationVersionLabelComparer();
        }

        public static IEnumerable<object[]> Matching()
        {
            yield return new[] { new Settings(), new Settings() };
            yield return new[] { new Settings { Label = { "one" } }, new Settings { Label = { "one" } } };
            yield return new[] { new Settings { Version = "1.2.3", Label = { "one", "two" } }, new Settings { Version = "1.2.3", Label = { "one", "two" } } };
        }

        [Theory]
        [MemberData(nameof(Matching))]
        public void Equals_WithMatchingValues_ReturnsTrue(Settings x, Settings y)
        {
            // Arrange / Act
            var result = _sut.Equals(x, y);

            // Assert
            result.Should().BeTrue();
        }

        public static IEnumerable<object[]> DifferInLabel()
        {
            yield return new[] { new Settings(), new Settings { Label = { "one" } } };
            yield return new[] { new Settings { Label = { "one" } }, new Settings() };
            yield return new[] { new Settings { Label = { "one", "two" } }, new Settings { Label = { "one", "three" } } };
        }

        [Theory]
        [MemberData(nameof(DifferInLabel))]
        public void Equals_DifferInLabel_ReturnsFalse(Settings x, Settings y)
        {
            // Arrange / Act
            var result = _sut.Equals(x, y);

            // Assert
            result.Should().BeFalse();
        }

        public static IEnumerable<object[]> DifferInVersion()
        {
            yield return new[] { new Settings(), new Settings { Version = "1.2.3" } };
            yield return new[] { new Settings { Version = "1.2.3" }, new Settings() };
            yield return new[] { new Settings { Version = "1.2.3" }, new Settings { Version = "1.2.3.4" } };
        }

        [Theory]
        [MemberData(nameof(DifferInVersion))]
        public void Equals_DifferInVersion_ReturnsFalse(Settings x, Settings y)
        {
            // Arrange / Act
            var result = _sut.Equals(x, y);

            // Assert
            result.Should().BeFalse();
        }

        public static IEnumerable<object[]> NullValues()
        {
            yield return new[] { new Settings(), null };
            yield return new[] { null, new Settings() };
        }

        [Theory]
        [MemberData(nameof(NullValues))]
        public void Equals_NullInstance_ReturnsFalse(Settings x, Settings y)
        {
            // Arrange / Act
            var result = _sut.Equals(x, y);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void GetHashCode_Returns_Hashcode()
        {
            // Arrange
            var config = new Settings
            {
                Version = "1.2.3",
                Label = { "hi" }
            };
            var expected = (config.Version.GetHashCode(StringComparison.OrdinalIgnoreCase) * 17) + config.Label.GetHashCode();

            // Arrange / Act
            var result = _sut.GetHashCode(config);

            // Assert
            result.Should().Be(expected);
        }
    }
}
