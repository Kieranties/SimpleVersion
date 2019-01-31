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
        public void Equals_WithNullValue_Compares(string xVersion, List<string> xLabel, string yVersion , List<string> yLabel, bool expected)
        {
            // Arrange / Act
            var result = _sut.Equals((xVersion, xLabel), (yVersion, yLabel));

            // Assert
            result.Should().Be(expected);
        }
    }
}
