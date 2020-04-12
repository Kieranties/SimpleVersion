// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using Microsoft.Extensions.DependencyInjection.Extensions;
using SimpleVersion;
using SimpleVersion.Environment;
using SimpleVersion.Pipeline;
using SimpleVersion.Pipeline.Formatting;
using SimpleVersion.Serialization;

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
        /// <param name="path">The path to the repository.</param>
        /// <returns>An updated collection based on <paramref name="services"/>.</returns>
        public static IServiceCollection AddSimpleVersion(
            this IServiceCollection services,
            string path)
        {
            Assert.ArgumentNotNull(services, nameof(services));

            services.AddSingleton<ISerializer, Serializer>();
            services.AddSingleton<IVersionRepository>(
                sp => ActivatorUtilities.CreateInstance<GitVersionRepository>(sp, path));

            // TODO: Resolve environment
            services.TryAddSingleton<IEnvironmentVariableAccessor, EnvironmentVariableAccessor>();
            services.TryAddSingleton<IVersionEnvironment, AzureDevopsEnvironment>();

            services.AddSingleton<IVersionPipeline, VersionPipeline>();

            // TODO: Order is important - validate
            services.AddSingleton<IVersionPipelineProcessor, VersionFormatProcessor>();
            services.AddSingleton<IVersionPipelineProcessor, Semver1FormatProcessor>();
            services.AddSingleton<IVersionPipelineProcessor, Semver2FormatProcessor>();

            return services;
        }
    }
}
