//using Xunit;
//using FluentAssertions;
//using System;
//using System.IO;

//namespace SimpleVersion.Core.Tests
//{
//    public class VersionModelFixture: IDisposable
//    {
//        private readonly string _initDirectory;

//        public VersionModelFixture()
//        {
//            _initDirectory = Directory.GetCurrentDirectory();
//        }

//        [Fact]
//        public void Load_DefaultPath_MissingJson_Throws()
//        {
//            // Arrange
//            var current = Directory.GetCurrentDirectory();
//            var expectedPath = Path.Combine(current, Constants.VersionFileName);

//            // Act
//            Action action = () => VersionModel.Load();

//            // Assert
//            action.Should().Throw<FileNotFoundException>()
//                .WithMessage("Could not find SimpleVersion configuration")
//                .And.FileName.Should().Be(expectedPath);
//        }

//        [Fact]
//        public void Load_ExplicitPath_MissingJson_Throws()
//        {
//            // Arrange
//            var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
//            var expectedPath = Path.Combine(path, Constants.VersionFileName);

//            // Act
//            Action action = () => VersionModel.Load(path);

//            // Assert
//            action.Should().Throw<FileNotFoundException>()
//                .WithMessage("Could not find SimpleVersion configuration")
//                .And.FileName.Should().Be(expectedPath);
//        }

//        [Fact]
//        public void Load_DefaultPath_WithJson_Populates()
//        {
//            // Arrange
//            var current = Directory.GetCurrentDirectory();
//            Directory.SetCurrentDirectory(Path.Combine(current, "Assets"));

//            // Act
//            var sut = VersionModel.Load();

//            // Assert
//            sut.Version.Should().Be("1.2.3.4");
//            sut.Label.Should().HaveCount(1)
//                .And.ContainSingle("alpha1");
//            sut.MetaData.Should().HaveCount(1)
//                .And.ContainSingle("test");
//        }

//        [Fact]
//        public void Load_ExplicitPath_WithJson_Populates()
//        {
//            // Act
//            var sut = VersionModel.Load(Path.Combine(".", "Assets"));

//            // Assert
//            sut.Version.Should().Be("1.2.3.4");
//            sut.Label.Should().HaveCount(1)
//                .And.ContainSingle("alpha1");
//            sut.MetaData.Should().HaveCount(1)
//                .And.ContainSingle("test");
//        }

//        [Fact]
//        public void Ctor_Defaults()
//        {
//            // Arrange / Act
//            var sut = new VersionModel();

//            // Assert
//            sut.Version.Should().BeEmpty();
//            sut.Label.Should().BeEmpty();
//            sut.MetaData.Should().BeEmpty();
//        }

//        public void Dispose()
//        {
//            Directory.SetCurrentDirectory(_initDirectory);
//        }
//    }
//}
