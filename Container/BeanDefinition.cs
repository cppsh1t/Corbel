#nullable enable
using System.Reflection;
using Corbel.Extension;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Corbel.IOC
{

    public enum CreateMode
    {
        ByOutside,
        ByConstructor,
        ByMethod,
        ByFactoryBean
    }

    public enum InjectMode
    {
        None,
        PreCompile,
        Runtime
    }

    public class BeanDefinition
    {

        private const BindingFlags injectFlag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        public Type KeyType { get; private set; }
        public string Name { get; private set; }

        public Type ObjectType { get; private set; }

        public bool LazyInit { get; set; } = false;

        public bool IsSingleton { get; set; } = true;

        public CreateMode CreateMode { get; set; }
        public InjectMode InjectMode { get; set; }

        public FieldInfo[]? InjectFields { get; set; }

        public PropertyInfo[]? InjectProps { get; set; }

        public ConstructorInfo? Constructor { get; set; }

        public MethodInfo? CreateMethod { get; set; }

        public object? Invoker { get; set; }

        public BeanDefinition? FactoryDefinition { get; set; }

        public BeanDefinition(Type keyType, string name, Type objectType)
        {
            KeyType = keyType;
            Name = name;
            ObjectType = objectType;
        }

        public BeanDefinition(InjectInfo injectInfo, Type objectType)
        {
            KeyType = injectInfo.KeyType!;
            Name = injectInfo.Name!;
            ObjectType = objectType;
        }

        public static BeanDefinition CreateFromPillar(Type type)
        {
            var injectInfo = type.GetCustomAttribute<InjectObjectAttribute>()!.InjectInfo.FixSelf(type);

            var fields = type.GetFields(injectFlag).Where(f => f.GetCustomAttribute<InjectAttribute>() != null);
            var props = type.GetProperties(injectFlag).Where(p => p.GetCustomAttribute<InjectAttribute>() != null && p.CanWrite);

            var definition = new BeanDefinition(injectInfo, type)
            {
                CreateMode = CreateMode.ByConstructor,
                InjectMode = InjectMode.PreCompile,
                InjectFields = fields.ToArray(),
                InjectProps = props.ToArray()
            };

            if (type.GetCustomAttribute<LazyInitAttribute>() != null)
            {
                definition.LazyInit = true;
            }

            if (type.GetCustomAttribute<ScopeAttribute>()?.ScopeType == ScopeType.Prototype)
            {
                definition.IsSingleton = false;
            }

            definition.Constructor = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(c => c.GetCustomAttribute<InjectAttribute>() != null);


            return definition;
        }

        public static BeanDefinition CreateFromBean(MethodInfo method, object configObject)
        {
            var injectInfo = method.GetCustomAttribute<InjectObjectAttribute>()!.InjectInfo.FixSelf(method.ReturnType);
            var definition = new BeanDefinition(injectInfo, method.ReturnType)
            {
                CreateMode = CreateMode.ByMethod,
                InjectMode = InjectMode.None,
                CreateMethod = method,
                Invoker = configObject
            };

            if (method.GetCustomAttribute<LazyInitAttribute>() != null)
            {
                definition.LazyInit = true;
            }
            return definition;
        }

        public static BeanDefinition CreateFromFactoryBean(Type factoryType, BeanDefinition factoryDef)
        {
            var keyType = factoryType.BaseType!.GetGenericArguments().First();
            var name = keyType.Name.FirstLetterToLower();
            return new BeanDefinition(keyType, name, keyType)
            {
                CreateMode = CreateMode.ByFactoryBean,
                InjectMode = InjectMode.None,
                FactoryDefinition = factoryDef,
                IsSingleton = false
            };
        }


        public static void UpdateDefOfProduct(BeanDefinition definition, Type objectType, string? newName, bool isSingleton)
        {
            definition.ObjectType = objectType;
            definition.IsSingleton = isSingleton;
            if (newName != null && newName != string.Empty)
            {
                definition.Name = newName;
            }
        }

        public override string ToString()
        {
            return "{" + $"KeyType: {KeyType}, Name: {Name}, ObjectType: {ObjectType}" + "}";
        }
    }
}