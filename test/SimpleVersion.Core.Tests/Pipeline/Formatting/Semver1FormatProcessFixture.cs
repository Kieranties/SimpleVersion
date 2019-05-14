// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Collections.Generic;
using FluentAssertions;
using GitTools.Testing;
using SimpleVersion.Pipeline;
using SimpleVersion.Pipeline.Formatting;
using Xunit;

namespace SimpleVersion.Core.Tests.Pipeline.Formatting
{
    public class Semver1FormatProcessFixture
    {
        private readonly Semver1FormatProcess _sut;

        public Semver1FormatProcessFixture()
        {
            _sut = new Semver1FormatProcess();
        }

        public static IEnumerable<object[]> LabelParts()
        {
            yield return new object[] { Array.Empty<object>(), "1.2.0", 10, "1.2.0" };
            yield return new object[] { new[] { "one" }, "1.2.0", 10, "1.2.0-one-0010" };
            yield return new object[] { new[] { "one", "two" }, "1.2.0", 106, "1.2.0-one-two-0106" };
            yield return new object[] { new[] { "*", "one", "two" }, "1.2.0", 106, "1.2.0-0106-one-two" };
            yield return new object[] { new[] { "one", "*", "two" }, "1.2.0", 106, "1.2.0-one-0106-two" };
            yield return new object[] { new[] { "one", "two*", "three" }, "1.2.0", 106, "1.2.0-one-two0106-three" };
            yield return new object[] { new[] { "one", "*two*", "three" }, "1.2.0", 106, "1.2.0-one-0106two0106-three" };
            yield return new object[] { new[] { "one", "*t*o*", "three" }, "1.2.0", 106, "1.2.0-one-0106t0106o0106-three" };
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
            using (var fixture = new EmptyRepositoryFixture())
            {
                var context = new VersionContext(fixture.Repository)
                {
                    Configuration = Utils.GetConfiguration(version, label: parts),
                    Result = Utils.GetVersionResult(height, false)
                };
                context.Result.Version = context.Configuration.Version;

                var shaSub = context.Result.Sha.Substring(0, 7);
                var fullExpected = $"{expectedPart}-c{shaSub}";

                // Act
                _sut.Apply(context);

                // Assert
                context.Result.Formats.Should().ContainKey("Semver1");
                context.Result.Formats["Semver1"].Should().Be(fullExpected);
            }
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
            using (var fixture = new EmptyRepositoryFixture())
            {
                var context = new VersionContext(fixture.Repository)
                {
                    Configuration = Utils.GetConfiguration(version, label: parts),
                    Result = Utils.GetVersionResult(height, true)
                };
                context.Result.Version = context.Configuration.Version;

                // Act
                _sut.Apply(context);

                // Assert
                context.Result.Formats.Should().ContainKey("Semver1");
                context.Result.Formats["Semver1"].Should().Be(expected);
            }
        }
    }
}
