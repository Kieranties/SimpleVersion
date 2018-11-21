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
            return new List<object[]>
            {
                new object[] { Array.Empty<object>(), "1.2.0", 10 },
                new object[] { new[] { "one" }, "1.2.0", 10 },
                new object[] { new[] { "one", "two" }, "1.2.0", 106 }
            };
        }

        [Theory]
        [MemberData(nameof(LabelParts))]
        public void Apply_LabelParts_NonRelease_Is_Formatted(
            string[] parts,
            string version,
            int height)
        {
            // Arrange
            var info = Utils.GetVersionInfo(version, parts);
            var result = Utils.GetVersionResult(height, false);
            string expected;
            if(parts.Length > 0)
                expected = $"{version}-{string.Join(".", parts)}.{height}.4ca82d2";
            else
                expected = $"{version}-4ca82d2+{height}";

            // Act
            _sut.Apply(info, result);

            // Assert
            result.Formats.Should().ContainKey("Semver2");
            result.Formats["Semver2"].Should().Be(expected);
        }


        [Theory]
        [MemberData(nameof(LabelParts))]
        public void Apply_LabelParts_Release_Is_Formatted(
            string[] parts,
            string version,
            int height)
        {
            // Arrange
            var info = Utils.GetVersionInfo(version, parts);
            var result = Utils.GetVersionResult(height);
            string expected;
            if (parts.Length > 0)
                expected = $"{version}-{string.Join(".", parts)}.{height}";
            else
                expected = $"{version}+{height}";

            // Act
            _sut.Apply(info, result);

            // Assert
            result.Formats.Should().ContainKey("Semver2");
            result.Formats["Semver2"].Should().Be(expected);
        }

        public static IEnumerable<object[]> MetaDataParts()
        {
            yield return new object[] { Array.Empty<object>(), "1.2.0", 10 };
            yield return new object[] { new[] { "one" }, "1.2.0", 10};
            yield return new object[] { new[] { "one", "two" }, "1.2.0", 106 };
        }

        [Theory]
        [MemberData(nameof(MetaDataParts))]
        public void Apply_MetaDataParts_NonRelease_Is_Formatted(
            string[] parts,
            string version,
            int height)
        {
            // Arrange
            var info = Utils.GetVersionInfo(version, meta: parts);
            var result = Utils.GetVersionResult(height, false);
            string expected;
            if (parts.Length > 0)
                expected = $"{version}-4ca82d2+{height}.{string.Join(".", parts)}";
            else
                expected = $"{version}-4ca82d2+{height}";

            // Act
            _sut.Apply(info, result);

            // Assert
            result.Formats.Should().ContainKey("Semver2");
            result.Formats["Semver2"].Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(MetaDataParts))]
        public void Apply_MetaDataParts_Release_Is_Formatted(
            string[] parts, 
            string version, 
            int height)
        {

            // Arrange
            var info = Utils.GetVersionInfo(version, meta: parts);
            var result = Utils.GetVersionResult(height);
            string expected;
            if (parts.Length > 0)
                expected = $"{version}+{height}.{string.Join(".", parts)}";
            else
                expected = $"{version}+{height}";

            // Act
            _sut.Apply(info, result);

            // Assert
            result.Formats.Should().ContainKey("Semver2");
            result.Formats["Semver2"].Should().Be(expected);
        }
    }
}
