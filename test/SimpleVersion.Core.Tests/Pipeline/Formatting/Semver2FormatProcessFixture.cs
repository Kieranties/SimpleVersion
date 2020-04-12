// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Collections.Generic;
using FluentAssertions;
using GitTools.Testing;
using NSubstitute;
using SimpleVersion.Pipeline;
using SimpleVersion.Pipeline.Formatting;
using Xunit;

namespace SimpleVersion.Core.Tests.Pipeline.Formatting
{
    public class Semver2FormatProcessFixture
    {
        private readonly Semver2FormatProcessor _sut;

        public Semver2FormatProcessFixture()
        {
            _sut = new Semver2FormatProcessor();
        }

        public static IEnumerable<object[]> LabelParts()
        {
            yield return new object[] { Array.Empty<object>(), "1.2.0", 10, "1.2.0" };
            yield return new object[] { new[] { "one" }, "1.2.0", 10, "1.2.0-one.10" };
            yield return new object[] { new[] { "one", "two" }, "1.2.0", 106, "1.2.0-one.two.106" };
            yield return new object[] { new[] { "*", "one", "two" }, "1.2.0", 106, "1.2.0-106.one.two" };
            yield return new object[] { new[] { "one", "*", "two" }, "1.2.0", 106, "1.2.0-one.106.two" };
            yield return new object[] { new[] { "one", "two*", "three" }, "1.2.0", 106, "1.2.0-one.two106.three" };
            yield return new object[] { new[] { "one", "*two*", "three" }, "1.2.0", 106, "1.2.0-one.106two106.three" };
            yield return new object[] { new[] { "one", "*t*o*", "three" }, "1.2.0", 106, "1.2.0-one.106t106o106.three" };
        }

        [Theory]
        [MemberData(nameof(LabelParts))]
        public void Process_LabelParts_NonRelease_Is_Formatted(
            string[] parts,
            string version,
            int height,
            string expectedPart)
        {
            // Arrange
            using (var fixture = new EmptyRepositoryFixture())
            {
                fixture.MakeACommit();
                var context = Substitute.For<IVersionContext>();
                context.Configuration.Returns(Utils.GetRepositoryConfiguration(version, label: parts));
                context.Result.Returns(Utils.GetVersionResult(height, false));
                context.Result.Version = context.Configuration.Version;

                var divider = parts.Length > 0 ? '.' : '-';
                var fullExpected = $"{expectedPart}{divider}c{context.Result.Sha7}";

                // Act
                _sut.Process(context);

                // Assert
                context.Result.Formats.Should().ContainKey("Semver2");
                context.Result.Formats["Semver2"].Should().Be(fullExpected);
            }
        }

        [Theory]
        [MemberData(nameof(LabelParts))]
        public void Process_LabelParts_Release_Is_Formatted(
            string[] parts,
            string version,
            int height,
            string expected)
        {
            // Arrange
            using (var fixture = new EmptyRepositoryFixture())
            {
                fixture.MakeACommit();
                var context = Substitute.For<IVersionContext>();
                context.Configuration.Returns(Utils.GetRepositoryConfiguration(version, parts));
                context.Result.Returns(Utils.GetVersionResult(height));
                context.Result.Version = context.Configuration.Version;

                // Act
                _sut.Process(context);

                // Assert
                context.Result.Formats.Should().ContainKey("Semver2");
                context.Result.Formats["Semver2"].Should().Be(expected);
            }
        }

        public static IEnumerable<object[]> MetadataParts()
        {
            yield return new object[] { Array.Empty<object>(), "1.2.0", 10 };
            yield return new object[] { new[] { "one" }, "1.2.0", 10 };
            yield return new object[] { new[] { "one", "two" }, "1.2.0", 106 };
        }

        [Theory]
        [MemberData(nameof(MetadataParts))]
        public void Process_MetadataParts_NonRelease_Is_Formatted(
            string[] parts,
            string version,
            int height)
        {
            // Arrange
            using (var fixture = new EmptyRepositoryFixture())
            {
                fixture.MakeACommit();
                var context = Substitute.For<IVersionContext>();
                context.Configuration.Returns(Utils.GetRepositoryConfiguration(version, meta: parts));
                context.Result.Returns(Utils.GetVersionResult(height, false));
                context.Result.Version = context.Configuration.Version;

                string expected;
                if (parts.Length > 0)
                {
                    expected = $"{version}-c{context.Result.Sha7}+{string.Join(".", parts)}";
                }
                else
                {
                    expected = $"{version}-c{context.Result.Sha7}";
                }

                // Act
                _sut.Process(context);

                // Assert
                context.Result.Formats.Should().ContainKey("Semver2");
                context.Result.Formats["Semver2"].Should().Be(expected);
            }
        }

        [Theory]
        [MemberData(nameof(MetadataParts))]
        public void Process_Parts_Release_Is_Formatted(
            string[] parts,
            string version,
            int height)
        {
            // Arrange
            using (var fixture = new EmptyRepositoryFixture())
            {
                fixture.MakeACommit();
                var context = Substitute.For<IVersionContext>();
                context.Configuration.Returns(Utils.GetRepositoryConfiguration(version, meta: parts));
                context.Result.Returns(Utils.GetVersionResult(height));
                context.Result.Version = context.Configuration.Version;

                string expected;
                if (parts.Length > 0)
                {
                    expected = $"{version}+{string.Join(".", parts)}";
                }
                else
                {
                    expected = $"{version}";
                }

                // Act
                _sut.Process(context);

                // Assert
                context.Result.Formats.Should().ContainKey("Semver2");
                context.Result.Formats["Semver2"].Should().Be(expected);
            }
        }
    }
}
