using System;
using System.Collections.Generic;
using Xunit;
using FluentAssertions;
using SimpleVersion.Formatters;

namespace SimpleVersion.Core.Tests.Formatters
{
    public class Semver1FormatFixture
    {
        private readonly Semver1Format _sut;

        public Semver1FormatFixture()
        {
            _sut = new Semver1Format();
        }

        public static IEnumerable<object[]> LabelParts()
        {
            yield return new object[] { Array.Empty<object>(), "1.2.0", 10, "1.2.0" };
            yield return new object[] { new[] { "one" }, "1.2.0", 10, "1.2.0-one-0010" };
            yield return new object[] { new[] { "one", "two" } , "1.2.0", 106, "1.2.0-one-two-0106" };
            yield return new object[] { new[] { "*", "one", "two" } , "1.2.0", 106, "1.2.0-0106-one-two" };
            yield return new object[] { new[] { "one", "*", "two" } , "1.2.0", 106, "1.2.0-one-0106-two" };
        }

        [Theory]
        [MemberData(nameof(LabelParts))]
        public void Apply_LabelParts_NonRelease_Is_Formatted(
            string[] parts,
            string version,
            int height,
            string expectedPart)
        {
            // Arrange
            var info = Utils.GetVersionInfo(version, label: parts);
            var result = Utils.GetVersionResult(height, false);
            result.Version = info.Version;

            var fullExpected = expectedPart;

            if (parts.Length > 0)
            {
                var shaSub = result.Sha.Substring(0, 7);
                fullExpected = $"{expectedPart}-{shaSub}";
            }

            // Act
            _sut.Apply(info, result);

            // Assert
            result.Formats.Should().ContainKey("Semver1");
            result.Formats["Semver1"].Should().Be(fullExpected);
        }


        [Theory]
        [MemberData(nameof(LabelParts))]
        public void Apply_LabelParts_Release_Is_Formatted(
            string[] parts,
            string version,
            int height,
            string expected)
        {
            // Arrange
            var info = Utils.GetVersionInfo(version, label: parts);
            var result = Utils.GetVersionResult(height, true);
            result.Version = info.Version;

            // Act
            _sut.Apply(info, result);

            // Assert
            result.Formats.Should().ContainKey("Semver1");
            result.Formats["Semver1"].Should().Be(expected);
        }
    }
}
