using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace Corbel.Util
{

    internal class ReflectUtil
    {
        internal static Type[] GetTypesWithAttribute(string nameSpaceName, Type attributeType)
        {

            Assembly assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes();
            List<Type> targetTypes = new();

            for (int i = 0; i < types.Length; i++)
            {
                var item = types[i];
                bool rightNameSpace = item.Namespace != null && item.Namespace.StartsWith(nameSpaceName);
                bool hasTargetAttribute = item.GetCustomAttribute(attributeType) != null;

                if (rightNameSpace && hasTargetAttribute) targetTypes.Add(item);
            }

            return targetTypes.ToArray();
        }

        internal static bool HasAttributeOnFieldOrProp(Type type, Type attributeType)
        {
            bool has = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Any(field => field.GetCustomAttribute(attributeType) != null);
            if (has) return true;

            has = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Any(prop => prop.GetCustomAttribute(attributeType) != null);
            return has;
        }
    }
}