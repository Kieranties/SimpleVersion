using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleVersion.Tokens
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TokenFallbackOptionAttribute : Attribute
    {
        public TokenFallbackOptionAttribute(string description)
        {
            Description = description;
        }

        public string Description { get; }
    }
}
