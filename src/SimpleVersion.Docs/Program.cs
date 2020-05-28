// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.IO;
using System.Linq;
using SimpleVersion.Configuration;

namespace SimpleVersion.Docs
{
    /// <summary>
    /// Command line entry point.
    /// </summary>
    public sealed class Program
    {
        /// <summary>
        /// Entry point for Docs invocation.
        /// </summary>
        /// <param name="args">The array of arguments.</param>
        public static void Main(string[] args)
        {
            if (args == null || args.Length != 1)
            {
                Console.Error.WriteLine("You must provide the output path as a single parameter.");
                return;
            }

            var path = args[0];

            SchemaGenerator.Generate(typeof(RepositoryConfiguration), path);
        }
    }
}
