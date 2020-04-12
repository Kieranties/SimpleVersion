using System;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SimpleVersion.Environment;
using Xunit;

namespace SimpleVersion.Core.Tests.Extensions
{
    public class ServiceCollectionExtensionsFixture
    {
        [Fact]
        public void AddSimpleVersion_NullServices_Throws()
        {
            // Arrange
            Action action = () => ServiceCollectionExtensions.AddSimpleVersion(null);

            // Act / Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("services");
        }

        [Fact]
        public void AddSimpleVersion_EmptyServiceCollection_RegistersServices()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            ServiceCollectionExtensions.AddSimpleVersion(services);

            // Assert
            services.Should().ContainSingle(descriptor => IsSingleton<IVersionCalculator, VersionCalculator>(descriptor));
            services.Should().ContainSingle(descriptor => IsSingleton<IEnvironmentVariableAccessor, EnvironmentVariableAccessor>(descriptor));
            services.Should().ContainSingle(descriptor => IsSingleton<IVersionEnvironment, AzureDevopsEnvironment>(descriptor));
            services.Should().HaveCount(3);
        }

        [Fact]
        public void AddSimpleVersion_CollectionContainsService_DoesNotChangeRegistration()
        {
            // Arrange
            var services = ServiceCollectionExtensions.AddSimpleVersion(new ServiceCollection());
            var originalServices = new ServiceDescriptor[services.Count];
            services.CopyTo(originalServices, 0);

            // Act
            var result = ServiceCollectionExtensions.AddSimpleVersion(services);

            // Assert
            result.Should().OnlyContain(x => originalServices.Contains(x));
        }

        private bool IsSingleton<T, K>(ServiceDescriptor descriptor)
        {
            return descriptor.ServiceType == typeof(T)
                && descriptor.ImplementationType == typeof(K)
                && descriptor.Lifetime == ServiceLifetime.Singleton;
        }
    }
}