// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using SimpleVersion.Configuration;
using SimpleVersion.Environment;
using SimpleVersion.Pipeline;
using Xunit;

namespace SimpleVersion.Abstractions.Tests.Pipeline
{
    public class VersionContextFixture
    {
        private readonly IVersionEnvironment _environment;
        private readonly IVersionRepository _repository;

        public VersionContextFixture()
        {
            _environment = Substitute.For<IVersionEnvironment>();
            _repository = Substitute.For<IVersionRepository>();
        }

        [Fact]
        public void Ctor_NullEnvironment_Throws()
        {
            // Arrange
            Action action = () => new VersionContext(null, _repository);

            // Act / Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("environment");
        }

        [Fact]
        public void Ctor_NullRepository_Throws()
        {
            // Arrange
            Action action = () => new VersionContext(_environment, null);

            // Act / Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("repository");
        }

        [Fact]
        public void Ctor_ValidParameters_SetsEnvironment()
        {
            // Arrange / Act
            var sut = new VersionContext(_environment, _repository);

            // Assert
            sut.Environment.Should().BeSameAs(_environment);
        }

        [Fact]
        public void Ctor_ValidParameters_SetsConfiguration()
        {
            // Arrange
            var versionConfig = new VersionConfiguration();
            _repository.GetConfiguration(null).Returns(versionConfig);

            // Act
            var sut = new VersionContext(_environment, _repository);

            // Assert
            sut.Configuration.Should().BeSameAs(versionConfig);
        }

        [Fact]
        public void Ctor_ValidParameters_SetsResult()
        {
            // Arrange / Acr
            var sut = new VersionContext(_environment, _repository);

            // Assert
            sut.Result.Should().NotBeNull();
        }

        [Fact]
        public void Ctor_EnvironmentHasCanonicalBranchName_GetsCanonicalBranchNameConfiguration()
        {
            // Arrange
            var canonicalBranchName = "EnvCanonicalBranchName";
            _environment.When(e => e.Process(Arg.Any<VersionContext>()))
                         .Do(call => call.Arg<VersionContext>().Result.CanonicalBranchName = canonicalBranchName);

            var versionConfig = new VersionConfiguration();
            _repository.GetConfiguration(canonicalBranchName).Returns(versionConfig);

            // Act
            var sut = new VersionContext(_environment, _repository);

            // Assert
            sut.Configuration.Should().BeSameAs(versionConfig);
        }

        [Fact]
        public void Ctor_ValidParameters_CallsProcessorsInOrder()
        {
            // Arrange
            var calledProcessors = new List<string>();
            _repository.When(r => r.Process(Arg.Any<VersionContext>()))
                       .Do(_ => calledProcessors.Add("repo"));
            _environment.When(e => e.Process(Arg.Any<VersionContext>()))
                       .Do(_ => calledProcessors.Add("env"));

            // Act
            var sut = new VersionContext(_environment, _repository);

            // Assert
            calledProcessors.Should().ContainInOrder("env", "repo");
        }
    }
}
