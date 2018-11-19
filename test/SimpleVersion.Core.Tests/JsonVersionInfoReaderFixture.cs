using FluentAssertions;
using System;
using System.IO;
using Xunit;

namespace SimpleVersion.Core.Tests
{
    public class JsonVersionInfoReaderFixture: IDisposable
    {
        private readonly JsonVersionInfoReader _sut;
        private string _initDirectory;

        public JsonVersionInfoReaderFixture()
        {
            _sut = new JsonVersionInfoReader();
            _initDirectory = Directory.GetCurrentDirectory();
        }

        [Fact]
        public void Load_CurrentDirectoryHasNoFile_Throws()
        {
            // Arrange

            // Ensure there is no file in the current directory
            var possiblePath = Path.Combine(Directory.GetCurrentDirectory(), Constants.VersionFileName);
            if (File.Exists(possiblePath)) File.Delete(possiblePath);

            // Act
            Action action = () => _sut.Load();

            // Assert
            action.Should().Throw<FileNotFoundException>()
                .WithMessage("Could not find version file")
                .And.FileName.Should().Be(possiblePath);

        }

        [Fact]
        public void Load_CurrentDirectoryHasFile_ReturnsVersion()
        {
            // Arrange
            Directory.SetCurrentDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Assets"));

            // Act
            var result = _sut.Load();

            // Assert
            result.Label.Should().HaveCount(1)
                .And.ContainSingle(l => l == "alpha1");
            result.MetaData.Should().HaveCount(2)
                .And.ContainInOrder("test", "branch");
            result.Version.Should().Be("1.2.3.4");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t\t   ")]
        public void Load_InvalidPath_Throws(string path)
        {
            // Arrange / Act
            Action action = () => _sut.Load(path);

            // Assert
            action.Should().Throw<ArgumentException>()
                .WithMessage("Null or empty value\r\nParameter name: path")
                .And.ParamName.Should().Be("path");
        }

        [Fact]
        public void Load_NonExistantPath_Throws()
        {
            // Arrange
            var expectedPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            // Act
            Action action = () => _sut.Load(expectedPath);

            // Assert
            action.Should().Throw<FileNotFoundException>()
                .WithMessage("Could not find version file")
                .And.FileName.Should().Be(Path.Combine(expectedPath, Constants.VersionFileName));
        }

        [Fact]
        public void Load_NonExistantFile_Throws()
        {
            // Arrange
            var expectedPath = Path.GetTempPath();

            // Act
            Action action = () => _sut.Load(expectedPath);

            // Assert
            action.Should().Throw<FileNotFoundException>()
                .WithMessage("Could not find version file")
                .And.FileName.Should().Be(Path.Combine(expectedPath, Constants.VersionFileName));
        }

        [Fact]
        public void Load_CustomPath_ReturnsVersion()
        {
            // Arrange
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Assets");

            // Act
            var result = _sut.Load(path);

            // Assert
            result.Label.Should().HaveCount(1)
                .And.ContainSingle(l => l == "alpha1");
            result.MetaData.Should().HaveCount(2)
                .And.ContainInOrder("test", "branch");
            result.Version.Should().Be("1.2.3.4");

        }
        public void Dispose() => Directory.SetCurrentDirectory(_initDirectory);
    }
}
