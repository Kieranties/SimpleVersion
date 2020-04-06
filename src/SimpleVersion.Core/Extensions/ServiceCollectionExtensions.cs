using Microsoft.Extensions.DependencyInjection.Extensions;
using SimpleVersion;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extensions to support usage of <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers defaults services.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to be updated.</param>
        /// <returns>An updated collection based on <paramref name="services"/>.</returns>
        public static IServiceCollection AddSimpleVersion(this IServiceCollection services)
        {
            Assert.ArgumentNotNull(services, nameof(services));

            services.TryAddSingleton<IVersionCalculator, VersionCalculator>();
            services.TryAddSingleton<IVersionEnvironment, VersionEnvironment>();

            return services;
        }
    }
}
