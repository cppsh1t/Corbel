#nullable enable

using Corbel.Structure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Corbel.IOC
{

    public class BeanDefinitionHolder
    {
        public SingleQueryMap<Type, string, BeanDefinition> Definitions { get; private set; } = new();

        public void PutDefinition(BeanDefinition definition)
        {
            Definitions.Put(definition.KeyType, definition.Name, definition);
        }

        public void PutDefinitions(params BeanDefinition[] definitions)
        {
            for (int i = 0; i < definitions.Length; i++)
            {
                var item = definitions[i];
                Definitions.Put(item.KeyType, item.Name, item);
            }
        }

        public void PutDefinitions(IEnumerable<BeanDefinition> definitions)
        {
            foreach (var def in definitions)
            {
                Definitions.Put(def.KeyType, def.Name, def);
            }
        }

        public BeanDefinition? QueryDefinition(Type? keyType, string? name)
        {
            return Definitions.Get(keyType, name);
        }

        public bool RemoveDefinition(BeanDefinition definition)
        {
            return Definitions.Remove(definition.KeyType, definition.Name);
        }

        public bool HasDefinition(BeanDefinition definition)
        {
            return Definitions.Values.Contains(definition);
        }

        public void ClearDefinition()
        {
            Definitions.Clear();
        }

    }
}