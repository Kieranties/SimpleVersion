using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleVersion.Tokens
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class TokenValueOptionAttribute : Attribute
    {
        public TokenValueOptionAttribute(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public string Description { get; set; }

        public string Alias { get; set; }
    }
}
