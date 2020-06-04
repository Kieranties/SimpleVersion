// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using Microsoft.Extensions.DependencyInjection;
using SimpleVersion.Environment;
using SimpleVersion.Pipeline;
using SimpleVersion.Pipeline.Formatting;
using SimpleVersion.Serialization;
using SimpleVersion.Tokens;

namespace SimpleVersion.Tool
{
    /// <summary>
    /// Command line entry point.
    /// </summary>
    public sealed class Program
    {
        /// <summary>
        /// Entry point for SimpleVersion invocation.
        /// </summary>
        /// <param name="args">The array of arguments.</param>
        public static void Main(string[] args)
        {
            var exitCode = 0;
            try
            {
                // TODO: arguments and options
                var path = System.IO.Directory.GetCurrentDirectory();
                if (args?.Length > 0)
                {
                    path = args[0];
                }

                var calculator = ResolveCalculator(path);
                calculator.WriteResult(Console.Out);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[Error] {ex.Message}");
                exitCode = 1;
            }
            finally
            {
                System.Environment.ExitCode = exitCode;
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        private static IVersionCalculator ResolveCalculator(string path)
        {
            var services = new ServiceCollection()
                .AddSingleton<IVersionCalculator>(sp => ActivatorUtilities.CreateInstance<VersionCalculator>(sp, path))
                .AddSingleton<ISerializer, Serializer>()
                .AddSingleton<IEnvironmentVariableAccessor, EnvironmentVariableAccessor>()
                .AddSingleton<IVersionEnvironment, AzureDevopsEnvironment>()
                .AddSingleton<DefaultVersionEnvironment>()
                .AddSingleton<ITokenEvaluator, TokenEvaluator>()
                .AddSingleton<ITokenHandler, BranchNameTokenHandler>()
                .AddSingleton<ITokenHandler, LabelTokenHandler>()
                .AddSingleton<ITokenHandler, SemverTokenHandler>()
                .AddSingleton<ITokenHandler, ShaTokenHandler>()
                .AddSingleton<ITokenHandler, ShortBranchNameTokenHandler>()
                .AddSingleton<ITokenHandler, VersionTokenHandler>()
                .AddSingleton<IVersionProcessor, EnvironmentVersionProcessor>()
                .AddSingleton<IVersionProcessor, GitRepositoryVersionProcessor>()
                .AddSingleton<IVersionProcessor, VersionVersionProcessor>()
                .AddSingleton<IVersionProcessor, FormatsVersionProcessor>()
                .BuildServiceProvider();

            return services.GetRequiredService<IVersionCalculator>();
        }
    }
}
