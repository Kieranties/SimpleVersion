// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Collections.Generic;
using FluentAssertions;
using SimpleVersion.Comparers;
using SimpleVersion.Configuration;
using Xunit;

namespace SimpleVersion.Core.Tests.Comparers
{
    public class VersionConfigurationLabelComparerFixture
    {
        private readonly VersionConfigurationLabelComparer _sut;

        public VersionConfigurationLabelComparerFixture()
        {
            _sut = new VersionConfigurationLabelComparer();
        }

        public static IEnumerable<object[]> Matching()
        {
            var sameReference = new VersionConfiguration();
            yield return new[] { sameReference, sameReference };
            yield return new[] { new VersionConfiguration(), new VersionConfiguration() };
            yield return new[] { new VersionConfiguration { Label = { "one" } }, new VersionConfiguration { Label = { "one" } } };
            yield return new[] { new VersionConfiguration { Version = "1.2.3", Label = { "one", "two" } }, new VersionConfiguration { Version = "1.2.3", Label = { "one", "two" } } };
        }

        [Theory]
        [MemberData(nameof(Matching))]
        public void Equals_WithMatchingValues_ReturnsTrue(VersionConfiguration x, VersionConfiguration y)
        {
            // Arrange / Act
            var result = _sut.Equals(x, y);

            // Assert
            result.Should().BeTrue();
        }

        public static IEnumerable<object[]> DifferInLabel()
        {
            yield return new[] { new VersionConfiguration(), new VersionConfiguration { Label = { "one" } } };
            yield return new[] { new VersionConfiguration { Label = { "one" } }, new VersionConfiguration() };
            yield return new[] { new VersionConfiguration { Label = { "one", "two" } }, new VersionConfiguration { Label = { "one", "three" } } };
            yield return new[] { new VersionConfiguration { Label = { "one", "two" } }, new VersionConfiguration { Label = { "two", "one" } } };
            yield return new[] { new VersionConfiguration { Label = null }, new VersionConfiguration { Label = { "one" } } };
            yield return new[] { new VersionConfiguration { Label = { "one" } }, new VersionConfiguration { Label = null } };
        }

        [Theory]
        [MemberData(nameof(DifferInLabel))]
        public void Equals_DifferInLabel_ReturnsFalse(VersionConfiguration x, VersionConfiguration y)
        {
            // Arrange / Act
            var result = _sut.Equals(x, y);

            // Assert
            result.Should().BeFalse();
        }

        public static IEnumerable<object[]> DifferInVersion()
        {
            yield return new[] { new VersionConfiguration(), new VersionConfiguration { Version = "1.2.3" } };
            yield return new[] { new VersionConfiguration { Version = "1.2.3" }, new VersionConfiguration() };
            yield return new[] { new VersionConfiguration { Version = "1.2.3" }, new VersionConfiguration { Version = "1.2.3.4" } };
        }

        [Theory]
        [MemberData(nameof(DifferInVersion))]
        public void Equals_DifferInVersion_ReturnsFalse(VersionConfiguration x, VersionConfiguration y)
        {
            // Arrange / Act
            var result = _sut.Equals(x, y);

            // Assert
            result.Should().BeFalse();
        }

        public static IEnumerable<object[]> NullValues()
        {
            yield return new[] { new VersionConfiguration(), null };
            yield return new[] { null, new VersionConfiguration() };
        }

        [Theory]
        [MemberData(nameof(NullValues))]
        public void Equals_NullInstance_ReturnsFalse(VersionConfiguration x, VersionConfiguration y)
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
            var config = new VersionConfiguration
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
