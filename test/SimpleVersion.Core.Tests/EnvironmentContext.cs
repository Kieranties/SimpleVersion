// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Collections.Generic;

namespace SimpleVersion.Core.Tests
{
    public class EnvironmentContext : IDisposable
    {
        private readonly Dictionary<string, string> _initState = new Dictionary<string, string>();
        private bool _disposedValue = false;

        private static readonly Dictionary<string, string> _noBuildServerVars = new Dictionary<string, string>
        {
            ["TF_BUILD"] = "False"
        };

        public EnvironmentContext(Dictionary<string, string> state)
        {
            foreach (var entry in state)
            {
                _initState.Add(entry.Key, System.Environment.GetEnvironmentVariable(entry.Key));
                System.Environment.SetEnvironmentVariable(entry.Key, entry.Value);
            }
        }

        public static EnvironmentContext NoBuildServer() => new EnvironmentContext(_noBuildServerVars);

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
                        System.Environment.SetEnvironmentVariable(entry.Key, entry.Value);
                    }
                }

                _disposedValue = true;
            }
        }
    }
}
