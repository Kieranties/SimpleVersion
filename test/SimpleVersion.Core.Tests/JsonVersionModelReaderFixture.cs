using FluentAssertions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace SimpleVersion.Core.Tests
{
    public class JsonVersionModelReaderFixture
    {
        private readonly JsonVersionModelReader _sut;

        public JsonVersionModelReaderFixture()
        {
            _sut = new JsonVersionModelReader();
        }

        [Fact]
        public void Load_CurrentDirectoryHasNoFile_Throws()
        {
            // Arrange

            // Ensure there is no file in the current directory
            var possiblePath = Path.Combine(Directory.GetCurrentDirectory(), Constants.VersionFileName);
            if (File.Exists(possiblePath)) File.Delete(possiblePath);

            Action action = () => _sut.Load();

            // Act / Assert
            action.Should().Throw<FileNotFoundException>()
                .WithMessage("Could not find version file")
                .And.FileName.Should().Be(possiblePath);

        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t\t   ")]
        public void Load_InvalidPath_Throws(string path)
        {
            // Arrange
            Action action = () => _sut.Load(path);

            // Act / Assert
            action.Should().Throw<ArgumentException>()
                .WithMessage("Null or empty value\r\nParameter name: path")
                .And.ParamName.Should().Be("path");
        }
    }
}
