using FluentAssertions;
using SimpleVersion.Pipeline;
using SimpleVersion.Pipeline.Formatting;
using System;
using System.Collections.Generic;
using Xunit;

namespace SimpleVersion.Core.Tests.Pipeline.Formatting
{
    public class VersionFormatProcessFixture
    {
        private readonly VersionFormatProcess _sut;

        public VersionFormatProcessFixture()
        {
            _sut = new VersionFormatProcess();
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
            var context = new VersionContext
            {
                Configuration = {
                    Version = version
                }
            };            

            // Act
            Action action = () => _sut.Apply(context);

            // Asset
            action.Should().Throw<InvalidOperationException>()
                .WithMessage($"Version '{context.Configuration.Version}' is not in a valid format");
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
            // Arrange
            var context = new VersionContext
            {
                Configuration = {
                    Version = version
                }
            };

            // Act
            _sut.Apply(context);

            // Assert
            context.Result.Major.Should().Be(major);
            context.Result.Minor.Should().Be(minor);
            context.Result.Patch.Should().Be(patch);
            context.Result.Revision.Should().Be(revision);

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
            var context = new VersionContext
            {
                Configuration = {
                    Version = version
                },
                Result = {
                    Height = commits
                }
            };

            // Act
            _sut.Apply(context);

            // Assert
            context.Result.Major.Should().Be(major);
            context.Result.Minor.Should().Be(minor);
            context.Result.Patch.Should().Be(patch);
            context.Result.Revision.Should().Be(revision);
        }
    }
}
