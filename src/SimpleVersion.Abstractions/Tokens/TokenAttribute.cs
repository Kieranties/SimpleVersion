using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleVersion.Tokens
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TokenAttribute : Attribute
    {
        public TokenAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public string DefaultOption { get; set; }

        public string Description { get; set; }
    }
}
