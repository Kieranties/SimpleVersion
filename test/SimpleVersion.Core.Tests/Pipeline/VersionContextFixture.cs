// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using FluentAssertions;
using GitTools.Testing;
using SimpleVersion.Model;
using SimpleVersion.Pipeline;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace SimpleVersion.Core.Tests.Pipeline
{
    public class VersionContextFixture
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   \t\t")]
        public void Ctor_EmptyPath_Throws(string path)
        {
            // Arrange / Act
            Action action = () => new VersionContext(path);

            // Assert
            action.Should().Throw<ArgumentException>()
                .WithMessage("Path must be provided.\r\nParameter name: path");
        }

        public static IEnumerable<object[]> InvalidPaths()
        {
            yield return new[] { Environment.GetLogicalDrives()[0] };
            yield return new[] { Path.Combine(Directory.GetCurrentDirectory(), "does not exist") };
        }

        [Theory]
        [MemberData(nameof(InvalidPaths))]
        public void Ctor_InvalidContextPath_Throws(string path)
        {
            // Arrange / Act
            Action action = () => new VersionContext(path);

            // Assert
            action.Should().Throw<DirectoryNotFoundException>()
                .And.Message.Should().Be($"Could not find git repository at '{path}' or any parent directory.");
        }

        [Fact]
        public void Ctor_WithRootPath_Populates()
        {
            using (var fixture = new EmptyRepositoryFixture())
            {
                // Act
                var sut = new VersionContext(fixture.RepositoryPath);

                // Assert
                AssertContext(sut, fixture);
            }
        }

        [Fact]
        public void GetResult_DescendantPath_ReturnsRootPath()
        {
            using (var fixture = new EmptyRepositoryFixture())
            {
                // Arrange
                var dir = Directory.CreateDirectory(Path.Combine(fixture.RepositoryPath, "alpha", "beta"));

                // Act
                var sut = new VersionContext(dir.FullName);

                // Assert
                AssertContext(sut, fixture);
            }
        }

        private void AssertContext(VersionContext sut, RepositoryFixtureBase fixture)
        {
            sut.Configuration.Should().NotBeNull();
            sut.Repository.Should().NotBeNull();
            sut.Result.BranchName.Should().Be(fixture.Repository.Head.FriendlyName);
            sut.Result.CanonicalBranchName.Should().Be(fixture.Repository.Head.CanonicalName);
            sut.Result.Sha.Should().Be(fixture.Repository.Head.Tip.Sha);
        }
    }
}
