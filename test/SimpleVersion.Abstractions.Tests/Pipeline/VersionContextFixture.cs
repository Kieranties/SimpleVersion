// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.IO;
using FluentAssertions;
using SimpleVersion.Pipeline;
using Xunit;

namespace SimpleVersion.Abstractions.Tests.Pipeline
{
    public class VersionContextFixture
    {
        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData("\t\t  ")]
        public void Ctor_InvalidValue_Throws(string value)
        {
            // Arrange
            Action action = () => new VersionContext(value);

            // Act / Assert
            action.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage($"'{value}' is not a valid working directory. (Parameter 'workingDirectory')")
                .And.ParamName.Should().Be("workingDirectory");
        }

        [Theory]
        [InlineData("/this/is/not/a/path")]
        public void Ctor_InvalidPath_Throws(string value)
        {
            // Arrange
            Action action = () => new VersionContext(value);

            // Act / Assert
            action.Should().Throw<DirectoryNotFoundException>()
                .WithMessage($"Could not find directory '{value}'");
        }

        [Fact]
        public void Ctor_ValidParameters_SetsEnvironment()
        {
            var path = System.Environment.CurrentDirectory;

            // Arrange / Act
            var sut = new VersionContext(path);

            // Assert
            sut.Environment.Should().BeNull();
            sut.Configuration.Should().BeNull();
            sut.Result.Should().NotBeNull();
            sut.WorkingDirectory.Should().Be(path);
        }
    }
}
