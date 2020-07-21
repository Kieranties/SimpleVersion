using System;

namespace SimpleVersion.Tokens
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TokenAttribute : Attribute
    {
        public TokenAttribute(string key)
        {
            Key = key;
        }

        public string Key { get; }

        public string Description { get; set; }
    }
}
