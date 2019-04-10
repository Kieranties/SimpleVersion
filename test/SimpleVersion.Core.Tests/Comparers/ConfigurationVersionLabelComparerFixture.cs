// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using FluentAssertions;
using SimpleVersion.Comparers;
using SimpleVersion.Model;
using System;
using System.Collections.Generic;
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
            yield return new[] { new Configuration(), new Configuration() };
            yield return new[] { new Configuration { Label = { "one" } }, new Configuration { Label = { "one" } } };
            yield return new[] { new Configuration { Version = "1.2.3", Label = { "one", "two" } }, new Configuration { Version = "1.2.3", Label = { "one", "two" } } };
        }

        [Theory]
        [MemberData(nameof(Matching))]
        public void Equals_WithMatchingValues_ReturnsTrue(Configuration x, Configuration y)
        {
            // Arrange / Act
            var result = _sut.Equals(x, y);

            // Assert
            result.Should().BeTrue();
        }

        public static IEnumerable<object[]> DifferInLabel()
        {
            yield return new[] { new Configuration(), new Configuration { Label = { "one" } } };
            yield return new[] { new Configuration { Label = { "one" } }, new Configuration() };
            yield return new[] { new Configuration { Label = { "one", "two" } }, new Configuration { Label = { "one", "three" } } };
        }

        [Theory]
        [MemberData(nameof(DifferInLabel))]
        public void Equals_DifferInLabel_ReturnsFalse(Configuration x, Configuration y)
        {
            // Arrange / Act
            var result = _sut.Equals(x, y);

            // Assert
            result.Should().BeFalse();
        }

        public static IEnumerable<object[]> DifferInVersion()
        {
            yield return new[] { new Configuration(), new Configuration { Version = "1.2.3" } };
            yield return new[] { new Configuration { Version = "1.2.3" }, new Configuration() };
            yield return new[] { new Configuration { Version = "1.2.3" }, new Configuration { Version = "1.2.3.4" } };
        }

        [Theory]
        [MemberData(nameof(DifferInVersion))]
        public void Equals_DifferInVersion_ReturnsFalse(Configuration x, Configuration y)
        {
            // Arrange / Act
            var result = _sut.Equals(x, y);

            // Assert
            result.Should().BeFalse();
        }

        public static IEnumerable<object[]> NullValues()
        {
            yield return new[] { new Configuration(), null };
            yield return new[] { null, new Configuration() };
        }

        [Theory]
        [MemberData(nameof(NullValues))]
        public void Equals_NullInstance_ReturnsFalse(Configuration x, Configuration y)
        {
            // Arrange / Act
            var result = _sut.Equals(x, y);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void GetHashCode_NullInstance_Throws()
        {
            // Arrange / Act
            Action action = () => _sut.GetHashCode(null);

            // Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("configuration");
        }

        [Fact]
        public void GetHashCode_NullVersion_Throws()
        {
            // Arrange 
            var config = new Configuration { Version = null };

            // Act
            Action action = () => _sut.GetHashCode(config);

            // Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("Version");
        }

        [Fact]
        public void GetHashCode_Returns_Hashcode()
        {
            // Arrange
            var config = new Configuration
            {
                Version = "1.2.3",
                Label = { "hi" }
            };
            var expected = (config.Version.GetHashCode() * 17) + config.Label.GetHashCode();

            // Arrange / Act
            var result = _sut.GetHashCode(config);

            // Assert
            result.Should().Be(expected);
        }
    }
}
