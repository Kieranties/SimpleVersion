using FluentAssertions;
using System;
using System.IO;
using Xunit;

namespace SimpleVersion.Core.Tests
{
    public class JsonVersionInfoWriterFixture
    {
        private readonly JsonVersionInfoWriter _sut;

        public JsonVersionInfoWriterFixture()
        {
            _sut = new JsonVersionInfoWriter();
        }

        [Fact]
        public void ToText_NullModel_Throws()
        {
            // Arrange / Act
            Action action = () => _sut.ToText(null);

            // Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("info");
        }

        [Fact]
        public void ToText_WithModel_Returns()
        {
            // Arrange
            var model = new VersionInfo
            {
                Label =
                {
                    "First",
                    "Second"
                },
                MetaData =
                {
                    "Meta1",
                    "Meta2"
                },
                Version = "1.2.3.4"
            };

            // Act
            var result = _sut.ToText(model);

            // Assert
            result.Should().Be("{\r\n  \"Version\": \"1.2.3.4\",\r\n  \"Label\": [\r\n    \"First\",\r\n    \"Second\"\r\n  ],\r\n  \"MetaData\": [\r\n    \"Meta1\",\r\n    \"Meta2\"\r\n  ]\r\n}");
        }

        [Fact]
        public void ToFile_NullModel_Throws()
        {
            // Arrange / Act
            Action action = () => _sut.ToFile(null, Directory.GetCurrentDirectory());

            // Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("info");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t\t   ")]
        public void ToFile_InvalidPath_Throws(string path)
        {
            // Arrange / Act
            Action action = () => _sut.ToFile(new VersionInfo(), path);

            // Assert
            action.Should().Throw<ArgumentException>()
                .WithMessage("Null or empty value\r\nParameter name: path")
                .And.ParamName.Should().Be("path");
        }

        [Fact]
        public void ToFile_NonExistantPath_WritesFile()
        {
            // Arrange
            var expectedPath = Path.Combine(Directory.GetCurrentDirectory(), Guid.NewGuid().ToString());

            // Act
            _sut.ToFile(new VersionInfo(), expectedPath);

            // Assert
            var resultPath = Path.Combine(expectedPath, Constants.VersionFileName);
            File.Exists(resultPath).Should().BeTrue();

            var resultContent = File.ReadAllText(resultPath);
            resultContent.Should().Be("{\r\n  \"Version\": \"\",\r\n  \"Label\": [],\r\n  \"MetaData\": []\r\n}");
        }
    }
}
