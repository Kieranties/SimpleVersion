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
    public class VersionCalculatorFixture
    {
        private readonly VersionCalculator _sut;

        public VersionCalculatorFixture()
        {
            _sut = new VersionCalculator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   \t\t")]
        public void GetResult_EmptyPath_Throws(string path)
        {
            // Arrange / Act
            Action action = () => _sut.GetResult(path);

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
        public void GetResult_InvalidContextPath_Throws(string path)
        {
            // Arrange / Act
            Action action = () => _sut.GetResult(path);

            // Assert
            action.Should().Throw<DirectoryNotFoundException>()
                .And.Message.Should().Be($"Could not find git repository at '{path}' or any parent directory.");
        }

        //[Fact]
        //public void GetResult_WithRootPath_ReturnsRootPath()
        //{
        //    using (var fixture = new EmptyRepositoryFixture())
        //    {
        //        // Act
        //        var result = _sut.GetResult(fixture.RepositoryPath);

        //        // Assert
        //        result.RepositoryPath.Should().Be(fixture.RepositoryPath);
        //    }
        //}

        //[Fact]
        //public void GetResult_DescendantPath_ReturnsRootPath()
        //{
        //    using (var fixture = new EmptyRepositoryFixture())
        //    {
        //        // Arrange
        //        var dir = Directory.CreateDirectory(Path.Combine(fixture.RepositoryPath, "alpha", "beta"));

        //        // Act
        //        var result = _sut.GetResult(dir.FullName);

        //        // Assert
        //        result.RepositoryPath.Should().Be(fixture.RepositoryPath);
        //    }
        //}
    }
}
