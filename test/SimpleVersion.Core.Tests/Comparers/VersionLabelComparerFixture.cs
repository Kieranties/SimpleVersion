using FluentAssertions;
using SimpleVersion.Comparers;
using System;
using System.Collections.Generic;
using Xunit;

namespace SimpleVersion.Core.Tests.Comparers
{
    public class VersionLabelComparerFixture
    {
        private readonly VersionLabelComparer _sut;

        public VersionLabelComparerFixture()
        {
            _sut = new VersionLabelComparer();
        }

        public static IEnumerable<object[]> NullValues()
        {
            yield return new object[] { null, new List<string>(), "1.2.3", new List<string>(), false };
            yield return new object[] { "1.2.3", null, "1.2.3", new List<string>(), false };
            yield return new object[] { "1.2.3", new List<string>(), null, new List<string>(), false };
            yield return new object[] { "1.2.3", new List<string>(), "1.2.3", null, false };
            yield return new object[] { "1.2.3", null, "1.2.3", null, true };
            yield return new object[] { null, new List<string>(), null, new List<string>(), true };
        }

        [Theory]
        [MemberData(nameof(NullValues))]
        public void Equals_WithValues_Compares(string xVersion, List<string> xLabel, string yVersion , List<string> yLabel, bool expected)
        {
            // Arrange / Act
            var result = _sut.Equals((xVersion, xLabel), (yVersion, yLabel));

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void GetHashCode_NullVersion_Throws()
        {
            // Arrange / Act
            Action action = () => _sut.GetHashCode((null, new List<string>()));

            // Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("Version");
        }

        [Fact]
        public void GetHashCode_NullLabel_Throws()
        {
            // Arrange / Act
            Action action = () => _sut.GetHashCode(("1.2.3", null));

            // Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("Label");
        }

        [Fact]
        public void GetHashCode_Returns_Hashcode()
        {
            var version = "1.2.3";
            var label = new List<string> { "hi" };
            var expected = version.GetHashCode() * 17 + label.GetHashCode();

            // Arrange / Act
            var result = _sut.GetHashCode((version, label));

            // Assert
            result.Should().Be(expected);
        }
    }
}
