#nullable enable

using Corbel.Extension;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Corbel.IOC
{

    public class InjectInfo
    {
        public Type? KeyType { get; private set; }
        public string? Name { get; private set; }

        public InjectInfo(Type? keyType, string? name)
        {
            KeyType = keyType;
            Name = name;
        }

        public InjectInfo FixSelf(Type objectType)
        {
            KeyType ??= objectType;
            Name ??= objectType.Name.FirstLetterToLower();
            return this;
        }
    }

    public abstract class InjectObjectAttribute : Attribute
    {
        public InjectInfo InjectInfo { get; private set; }

        public InjectObjectAttribute() => InjectInfo = new InjectInfo(null, null);

        public InjectObjectAttribute(Type type) => InjectInfo = new InjectInfo(type, null);

        public InjectObjectAttribute(string name) => InjectInfo = new InjectInfo(null, name);

        public InjectObjectAttribute(Type type, string name) => InjectInfo = new InjectInfo(type, name);
    }


    [AttributeUsage(AttributeTargets.Class)]
    public class PillarAttribute : InjectObjectAttribute
    {
        public PillarAttribute()
        {
        }

        public PillarAttribute(Type type) : base(type)
        {
        }

        public PillarAttribute(string name) : base(name)
        {
        }

        public PillarAttribute(Type type, string name) : base(type, name)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class BeanAttribute : InjectObjectAttribute
    {
        public BeanAttribute() { }

        public BeanAttribute(Type type) : base(type)
        {
        }

        public BeanAttribute(string name) : base(name)
        {
        }

        public BeanAttribute(Type type, string name) : base(type, name)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Constructor)]
    public class InjectAttribute : InjectObjectAttribute
    {
        public InjectAttribute()
        {
        }

        public InjectAttribute(Type type) : base(type)
        {
        }

        public InjectAttribute(string name) : base(name)
        {
        }

        public InjectAttribute(Type type, string name) : base(type, name)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class LazyInitAttribute : Attribute
    {

    }

    public enum ScopeType
    {
        Singleton, Prototype
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ScopeAttribute : Attribute
    {
        public ScopeType ScopeType { get; private set; } = ScopeType.Singleton;

        public ScopeAttribute() { }
        public ScopeAttribute(ScopeType scopeType) => ScopeType = scopeType;
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class PillarScanAttribute : Attribute
    {
        public string ScanPath { get; private set; }

        public PillarScanAttribute(string scanPath) => ScanPath = scanPath;
    }
}