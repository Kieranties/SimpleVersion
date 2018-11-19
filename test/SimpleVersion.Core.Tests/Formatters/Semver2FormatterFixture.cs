using FluentAssertions;
using SimpleVersion.Formatters;
using System;
using System.Collections.Generic;
using Xunit;

namespace SimpleVersion.Core.Tests.Formatters
{
    public class Semver2FormatterFixture
    {
        private readonly Semver2Format _sut;

        public Semver2FormatterFixture()
        {
            _sut = new Semver2Format();
        }

        public static IEnumerable<object[]> LabelParts()
        {
            yield return new object[] { Array.Empty<object>(), "1.2.0", 10, "1.2.0+10" };
            yield return new object[] { new[] { "one" }, "1.2.0", 10, "1.2.0-one.10" };
            yield return new object[] { new[] { "one", "two" }, "1.2.0", 106, "1.2.0-one.two.106" };
        }

        [Theory]
        [MemberData(nameof(LabelParts))]
        public void Apply_LabelParts_IsFormatted(string[] parts, string version, int height, string expected)
        {
            // Arrange
            var info = new VersionInfo { Version = version };
            info.Label.AddRange(parts);
            var result = new VersionResult { Height = height };

            // Act
            _sut.Apply(info, result);

            // Assert
            result.Formats.Should().ContainKey("Semver2");
            result.Formats["Semver2"].Should().Be(expected);
        }

        public static IEnumerable<object[]> MetaDataParts()
        {
            yield return new object[] { Array.Empty<object>(), "1.2.0", 10, "1.2.0+10" };
            yield return new object[] { new[] { "one" }, "1.2.0", 10, "1.2.0+10.one" };
            yield return new object[] { new[] { "one", "two" }, "1.2.0", 106, "1.2.0+106.one.two" };
        }

        [Theory]
        [MemberData(nameof(MetaDataParts))]
        public void Apply_MetaDataParts_IsFormatted(string[] parts, string version, int height, string expected)
        {
            // Arrange
            var info = new VersionInfo { Version = version };
            info.MetaData.AddRange(parts);
            var result = new VersionResult { Height = height };

            // Act
            _sut.Apply(info, result);

            // Assert
            result.Formats.Should().ContainKey("Semver2");
            result.Formats["Semver2"].Should().Be(expected);
        }
    }
}
