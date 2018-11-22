using FluentAssertions;
using SimpleVersion.Formatters;
using System;
using System.Collections.Generic;
using Xunit;

namespace SimpleVersion.Core.Tests.Formatters
{
    public class VersionFormatFixture
    {
        private readonly VersionFormat _sut;

        public VersionFormatFixture()
        {
            _sut = new VersionFormat();
        }

        public static IEnumerable<object[]> InvalidVersions()
        {
            yield return new[] { "1" };
            yield return new[] { "1.2.=" };
            yield return new[] { "1.l.4" };
            yield return new[] { "*.l.5" };
        }

        [Theory]
        [MemberData(nameof(InvalidVersions))]
        public void Apply_InvalidVersion_Throws(string version)
        {
            // Arrange
            var info = new VersionInfo { Version = version };
            var result = new VersionResult();

            // Act
            Action action = () => _sut.Apply(info, result);

            // Asset
            action.Should().Throw<InvalidOperationException>()
                .WithMessage($"Version '{info.Version}' is not in a valid format");
        }

        public static IEnumerable<object[]> ValidVersions()
        {
            yield return new object[] { "1.0", 1, 0, 0, 0 };
            yield return new object[] { "1.0.0", 1, 0, 0, 0 };
            yield return new object[] { "1.2.0", 1, 2, 0, 0 };
            yield return new object[] { "1.2.3", 1, 2, 3, 0 };
            yield return new object[] { "14.22.32.234", 14, 22, 32, 234 };
        }

        [Theory]
        [MemberData(nameof(ValidVersions))]
        public void Apply_ValidVersions_SetsMembers(string version, int major, int minor, int patch, int revision)
        {
            // Arrange
            var info = new VersionInfo { Version = version };
            var result = new VersionResult();

            // Act
            _sut.Apply(info, result);

            // Assert
            result.Major.Should().Be(major);
            result.Minor.Should().Be(minor);
            result.Patch.Should().Be(patch);
            result.Revision.Should().Be(revision);

        }

        [Theory]
        [InlineData("1.0", 10, 1, 0, 0, 0)]
        [InlineData("1.0.0", 10, 1, 0, 0, 0)]
        [InlineData("1.0.0.0", 10, 1, 0, 0, 0)]
        [InlineData("1.*", 10, 1, 10, 0, 0)]
        [InlineData("1.0.*", 10, 1, 0, 10, 0)]
        [InlineData("1.0.0.*", 10, 1, 0, 0, 10)]
        [InlineData("1.*.0", 10, 1, 10, 0, 0)]
        [InlineData("1.0.*.0", 10, 1, 0, 10, 0)]
        public void Height_In_Version(
            string version,
            int commits,
            int major,
            int minor,
            int patch,
            int revision)
        {
            // Arrange
            var info = new VersionInfo { Version = version };
            var result = new VersionResult { Height = commits };

            // Act
            _sut.Apply(info, result);

            // Assert
            result.Major.Should().Be(major);
            result.Minor.Should().Be(minor);
            result.Patch.Should().Be(patch);
            result.Revision.Should().Be(revision);
        }
    }
}
