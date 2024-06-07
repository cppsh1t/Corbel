using System;
using System.Collections.Generic;
using System.Linq;


namespace Corbel.Extension
{
    public static class TypeExtensison
    {
        public static bool IsAssignableTo(this Type self, Type target)
        {
            return target.IsAssignableFrom(self);
        }
    }
}