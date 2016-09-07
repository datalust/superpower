using System;

namespace Superpower.Display
{
    [AttributeUsage(AttributeTargets.Field)]
    public class TokenAttribute : Attribute
    {
        public string Category { get; set; }

        public string Example { get; set; }

        public string Description { get; set; }
    }
}
