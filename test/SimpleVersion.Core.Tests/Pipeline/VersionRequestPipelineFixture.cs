// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using NSubstitute;
using SimpleVersion.Environment;
using SimpleVersion.Pipeline;
using Xunit;

namespace SimpleVersion.Core.Tests.Pipeline
{
    public class VersionRequestPipelineFixture
    {
        private readonly IVersionEnvironment _environment;
        private readonly IVersionRepository _repository;

        public VersionRequestPipelineFixture()
        {
            _environment = Substitute.For<IVersionEnvironment>();
            _repository = Substitute.For<IVersionRepository>();
        }

        [Fact]
        public void Ctor_NullEnvironment_Throws()
        {
            // Arrange
            Action action = () => new VersionRequestPipeline(null, _repository, Enumerable.Empty<IVersionRequestPipelineProcessor>());

            // Act / Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("environment");
        }

        [Fact]
        public void Ctor_NullRepository_Throws()
        {
            // Arrange
            Action action = () => new VersionRequestPipeline(_environment, null, Enumerable.Empty<IVersionRequestPipelineProcessor>());

            // Act / Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("repository");
        }

        [Fact]
        public void Ctor_NullProcessors_Throws()
        {
            // Arrange
            Action action = () => new VersionRequestPipeline(_environment, _repository, null);

            // Act / Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("processors");
        }

        [Fact]
        public void Process_EmptyProcessors_ReturnsResult()
        {
            // Arrange
            var sut = new VersionRequestPipeline(_environment, _repository, Enumerable.Empty<IVersionRequestPipelineProcessor>());

            // Act
            var result = sut.Process();

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void Process_WithProcessors_CallsInOrder()
        {
            // Arrange
            var processors = new[]
            {
                Substitute.For<IVersionRequestPipelineProcessor>(),
                Substitute.For<IVersionRequestPipelineProcessor>(),
                Substitute.For<IVersionRequestPipelineProcessor>()
            };
            var calledProcessors = new List<int>();
            for (var x = 0; x < calledProcessors.Count; x++)
            {
                processors[x].When(p => p.Process(Arg.Any<VersionContext>()))
                                   .Do(_ => calledProcessors.Add(x));
            }

            var sut = new VersionRequestPipeline(_environment, _repository, processors);

            // Act
            sut.Process();

            calledProcessors.Should().ContainInOrder(0, 1, 2);
        }
    }
}
