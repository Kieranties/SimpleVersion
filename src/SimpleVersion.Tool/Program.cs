// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;

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
        /// <returns>0 if success, otherwise an error exit code.</returns>
        public static int Main(string[] args)
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

                var calculator = new VersionCalculator(path);
                calculator.WriteResult(Console.Out);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[Error] {ex.Message}");
                exitCode = -1;
            }
#pragma warning restore CA1031 // Do not catch general exception types

            return exitCode;
        }
    }
}
