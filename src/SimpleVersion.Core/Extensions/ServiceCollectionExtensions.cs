// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using Microsoft.Extensions.DependencyInjection.Extensions;
using SimpleVersion;
using SimpleVersion.Environment;

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

            // Environment
            services.TryAddSingleton<IEnvironmentVariableAccessor, EnvironmentVariableAccessor>();
            services.TryAddSingleton<IVersionEnvironment, AzureDevopsEnvironment>();

            // Processing
            services.TryAddSingleton<IVersionCalculator, VersionCalculator>();

            return services;
        }
    }
}
