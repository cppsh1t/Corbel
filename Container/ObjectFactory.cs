#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Corbel.IOC
{

    public interface IObjectFactory
    {
        object? GetBean(BeanDefinition definition);
        bool InjectByRuntime(object obj);
        bool InjectByPreCompile(object obj, BeanDefinition definition);

    }



    public class ObjectFactory : IObjectFactory
    {
        private readonly Func<Type, string?, object?> objectGetter;

        private const BindingFlags injectFlag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        public ObjectFactory(Func<Type, string?, object?> objectGetter)
        {
            this.objectGetter = objectGetter;
        }

        public object? GetBean(BeanDefinition definition)
        {
            return definition.CreateMode switch
            {
                CreateMode.ByConstructor => GetBeanFromClass(definition),
                CreateMode.ByMethod => GetBeanFromMethod(definition),
                CreateMode.ByFactoryBean => GetBeanFromFactoryBean(definition),
                _ => throw new InvalidOperationException("Wrong CreateMode"),
            };
        }

        private object? GetBeanFromClass(BeanDefinition definition)
        {
            var type = definition.ObjectType;
            var autoCon = type.GetConstructors().Where(con => con.GetCustomAttribute<InjectAttribute>() != null).FirstOrDefault();

            if (autoCon == null)
            {
                return Activator.CreateInstance(type);
            }

            var parameters = autoCon.GetParameters();
            if (parameters.Length == 0)
            {
                return autoCon.Invoke(default);
            }

            IEnumerable<(Type paramType, string? paramName)> paramPair = parameters.Select(p => (p.ParameterType, p.Name));
            var paramResults = paramPair.Select(pair => objectGetter(pair.paramType, pair.paramName));
            if (paramResults.Any(p => p == null)) throw new InvalidOperationException($"{autoCon}缺少对应的参数Bean");
            return autoCon.Invoke(paramResults.ToArray());
        }

        private object GetBeanFromMethod(BeanDefinition definition)
        {
            var method = definition.CreateMethod!;
            var parameters = method.GetParameters();
            if (parameters.Length == 0)
            {
                return method.Invoke(definition.Invoker!, default) ?? throw new InvalidOperationException("无法调用");
            }

            IEnumerable<(Type paramType, string? paramName)> paramPair = parameters.Select(p => (p.ParameterType, p.Name));
            var paramResults = paramPair.Select(pair => objectGetter(pair.paramType, pair.paramName));
            if (paramResults.Any(p => p == null)) throw new InvalidOperationException($"{method}缺少对应的参数Bean");
            return method.Invoke(definition.Invoker, paramResults.ToArray()) ?? throw new InvalidOperationException("无法调用");
        }

        private object? GetBeanFromFactoryBean(BeanDefinition definition)
        {
            var factoryKeyType = definition.FactoryDefinition!.KeyType;
            var factory = (objectGetter(factoryKeyType, null) as FactoryBeanBase)!;
            var product = factory.GetObject();
            if (!factory.HasInit)
            {
                factory.HasInit = true;
                var newName = factory.BeanName;
                var newObjectType = product.GetType();
                var isSingleton = factory.IsSingleton;
                BeanDefinition.UpdateDefOfProduct(definition, newObjectType, newName, isSingleton);
            }
            return product;
        }

        public bool InjectByRuntime(object obj)
        {
            var type = obj.GetType();
            var fields = type.GetFields(injectFlag).Where(f => f.GetCustomAttribute<InjectAttribute>() != null).ToArray();
            var props = type.GetProperties(injectFlag).Where(p => p.GetCustomAttribute<InjectAttribute>() != null && p.CanWrite).ToArray();
            return InjectFields(obj, fields) && InjectProps(obj, props);
        }

        public bool InjectByPreCompile(object obj, BeanDefinition definition)
        {
            var fields = definition.InjectFields ?? Array.Empty<FieldInfo>();
            var props = definition.InjectProps ?? Array.Empty<PropertyInfo>();
            return InjectFields(obj, fields) && InjectProps(obj, props);
        }

        private bool InjectFields(object target, FieldInfo[] fields)
        {
            bool injectSuccess = true;

            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                var keyType = field.FieldType;
                var name = field.Name;
                var result = objectGetter(keyType, name);
                if (result != null)
                {
                    field.SetValue(target, result);
                }
                else
                {
                    injectSuccess = false;
                }
            }
            return injectSuccess;
        }

        private bool InjectProps(object target, PropertyInfo[] props)
        {
            bool injectSuccess = true;

            for (int i = 0; i < props.Length; i++)
            {
                var prop = props[i];
                var keyType = prop.PropertyType;
                var name = prop.Name;
                var result = objectGetter(keyType, name);
                if (result != null)
                {
                    prop.SetValue(target, result);
                }
                else
                {
                    injectSuccess = false;
                }
            }
            return injectSuccess;
        }
    }
}