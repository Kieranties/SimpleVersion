using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleVersion.Tokens
{
    /// <summary>
    /// Represents a request for token parsing.
    /// </summary>
    public interface ITokenRequest
    {
        void Parse(string optionValue);
    }
}
