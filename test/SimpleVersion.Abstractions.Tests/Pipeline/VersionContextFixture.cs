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
        private readonly VersionConfiguration _config;
        private readonly VersionResult _result;

        public VersionContextFixture()
        {
            _environment = Substitute.For<IVersionEnvironment>();
            _config = new VersionConfiguration();
            _result = new VersionResult();
        }

        [Fact]
        public void Ctor_NullEnvironment_Throws()
        {
            // Arrange
            Action action = () => new VersionContext(null, _config, _result);

            // Act / Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("environment");
        }

        [Fact]
        public void Ctor_NullConfiguration_Throws()
        {
            // Arrange
            Action action = () => new VersionContext(_environment, null, _result);

            // Act / Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("configuration");
        }

        [Fact]
        public void Ctor_NullResult_Throws()
        {
            // Arrange
            Action action = () => new VersionContext(_environment, _config, null);

            // Act / Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("result");
        }

        [Fact]
        public void Ctor_ValidParameters_SetsEnvironment()
        {
            // Arrange / Act
            var sut = new VersionContext(_environment, _config, _result);

            // Assert
            sut.Environment.Should().BeSameAs(_environment);
            sut.Configuration.Should().BeSameAs(_config);
            sut.Result.Should().BeSameAs(_result);
        }
    }
}
