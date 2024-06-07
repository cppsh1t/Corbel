using System.Reflection;
using Corbel.Extension;
using Corbel.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Corbel.IOC
{
    internal class BeanParser
    {
        public static BeanDefinition[] GetBeanDefsFromMethod(Type configType, object configObject)
        {
            var beanMethods = configType.GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .Where(method => method.GetCustomAttribute<BeanAttribute>() != null);

            return beanMethods.Select(method => BeanDefinition.CreateFromBean(method, configObject)).ToArray();
        }

        public static BeanDefinition[] GetBeanDefsFromClass(string scanPath)
        {
            var pillarTypes = ReflectUtil.GetTypesWithAttribute(scanPath, typeof(PillarAttribute));
            var defs = new BeanDefinition[pillarTypes.Length];

            for (int i = 0; i < pillarTypes.Length; i++)
            {
                var item = pillarTypes[i];
                var def = BeanDefinition.CreateFromPillar(item);
                defs[i] = def;
            }

            return defs;
        }

        public static BeanDefinition[] GetBeanDefsFromFactoryBean(ICollection<BeanDefinition> pillarDefs)
        {
            var defs = new List<BeanDefinition>();
            foreach (BeanDefinition pillarDef in pillarDefs)
            {
                Type type = pillarDef.ObjectType;
                if (typeof(FactoryBeanBase).IsAssignableFrom(type))
                {
                    BeanDefinition def = BeanDefinition.CreateFromFactoryBean(type, pillarDef);
                    defs.Add(def);
                }
            }

            return defs.ToArray();
        }

        public static bool IsFactoryBean(Type type)
        {
            return type.BaseType != null && typeof(FactoryBeanBase).IsAssignableFrom(type.BaseType);
        }
    }
}