using FluentAssertions;
using GitTools.Testing;
using SimpleVersion.Pipeline;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SimpleVersion.Core.Tests.Pipeline
{
    public class ResolveRepositoryPathProcessFixture
    {
        private readonly ResolveRepositoryPathProcess _sut;

        public ResolveRepositoryPathProcessFixture()
        {
            _sut = new ResolveRepositoryPathProcess();
        }

        [Fact]
        public void Apply_NullContext_Throws()
        {
            // Arrange / Act
            Action action = () => _sut.Apply(null);

            // Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("context");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t\t   ")]
        public void Apply_EmptyOrNullContextPath_Throws(string path)
        {
            // Arrange / Act
            Action action = () => _sut.Apply(new VersionContext { Path = path });

            // Assert
            action.Should().Throw<Exception>();
        }


        public static IEnumerable<object[]> InvalidPaths()
        {
            yield return new[] { Environment.GetLogicalDrives()[0] };
            yield return new[] { Path.Combine(Directory.GetCurrentDirectory(), "does not exist") };
        }

        [Theory]
        [MemberData(nameof(InvalidPaths))]
        public void Apply_InvalidContextPath_Throws(string path)
        {
            // Arrange / Act
            Action action = () => _sut.Apply(new VersionContext { Path = path });

            // Assert
            action.Should().Throw<DirectoryNotFoundException>()
                .And.Message.Should().Be($"Could not find git repository at '{path}' or any parent directory");

        }

        [Fact]
        public void Apply_WithRootPath_ReturnsRootPath()
        {
            using(var fixture = new EmptyRepositoryFixture())
            {
                // Arrange
                var context = new VersionContext { Path = fixture.RepositoryPath };

                // Act
                _sut.Apply(context);

                // Assert
                context.RepositoryPath.Should().Be(fixture.RepositoryPath);
            }
        }

        [Fact]
        public void Apply_DescendantPath_ReturnsRootPath()
        {
            using (var fixture = new EmptyRepositoryFixture())
            {
                // Arrange
                var dir = Directory.CreateDirectory(Path.Combine(fixture.RepositoryPath, "alpha", "beta"));
                var context = new VersionContext { Path = dir.FullName };
               
                // Act
                _sut.Apply(context);

                // Assert
                context.RepositoryPath.Should().Be(fixture.RepositoryPath);
            }
        }

    }
}
