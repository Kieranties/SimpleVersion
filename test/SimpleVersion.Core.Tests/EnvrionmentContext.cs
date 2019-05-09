// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Collections.Generic;

namespace SimpleVersion.Core.Tests
{
    public class EnvrionmentContext : IDisposable
    {
        private static readonly Dictionary<string, string> _noBuildServerVars = new Dictionary<string, string>
        {
            ["TF_BUILD"] = "False"
        };

        public static EnvrionmentContext NoBuildServer() => new EnvrionmentContext(_noBuildServerVars);

        private readonly Dictionary<string, string> _initState = new Dictionary<string, string>();

        public EnvrionmentContext(Dictionary<string, string> state)
        {
            foreach (var entry in state)
            {
                _initState.Add(entry.Key, Environment.GetEnvironmentVariable(entry.Key));
                Environment.SetEnvironmentVariable(entry.Key, entry.Value);
            }
        }

        private bool _disposedValue = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    foreach (var entry in _initState)
                    {
                        Environment.SetEnvironmentVariable(entry.Key, entry.Value);
                    }
                }

                _disposedValue = true;
            }
        }
    }
}
