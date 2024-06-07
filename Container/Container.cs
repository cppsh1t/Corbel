#pragma warning disable CS8618
#nullable enable
using System.Reflection;
using Corbel.Extension;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Corbel.IOC
{

    public interface IBeanGetter
    {
        T? Resolve<T>();
        object? Resolve(Type type);
        object? Resolve(string name);
    }

    public interface IContainer : IBeanGetter
    {
        void Launch();
        void Close();
        bool Inject(object obj);
        bool RegisterSingleton(BeanDefinition definition, object singleton);
        bool RegisterSingleton(Type keyType, string? name, object singleton);
        void RegisterPillar(Type type);
        void RegisterPillar<T>();
        void RegisterClass(Type keyType, Type? type = null, string? name = null, bool isSingleton = true, bool lazyInit = false);
        void RegisterClass<T, U>(string? name = null, bool isSingleton = true, bool lazyInit = false);
        void RegisterPillars(params Type[] types);
        void RegisterBean(BeanDefinition def);
    }

    public class Container : IContainer
    {
        protected BeanDefinitionHolder definitionHolder = new();
        protected BeanDefinitionHolder cacheDefHolder = new();
        protected IObjectFactory objectFactory;
        public bool CanOverride { get; set; } = false;


        private readonly Dictionary<BeanDefinition, object> singletonObjects = new();

        private readonly Dictionary<BeanDefinition, object> singletonCache = new();
        private readonly Dictionary<BeanDefinition, object> prototypeCache = new();


        protected Type configClass;
        protected object configObj;

        public Container()
        {
            InitContainer();
        }

        public Container(Type configType, bool autoLaunch)
        {
            configClass = configType;
            configObj = Activator.CreateInstance(configType) ?? throw new InvalidOperationException($"配置类必须有无参构造函数");
            ProcessConfig();
            InitContainer();
            if (autoLaunch) Launch();
        }


        public Container(Type configType) : this(configType, true)
        {

        }


        protected virtual void ProcessConfig()
        {
            //Process Pillar
            var scanPath = configClass.GetCustomAttribute<PillarScanAttribute>()?.ScanPath;
            if (scanPath != null)
            {
                var definitions = BeanParser.GetBeanDefsFromClass(scanPath);
                definitionHolder.PutDefinitions(definitions);
                var remainDefs = definitions.Where(def => def.IsSingleton && !def.LazyInit);
                cacheDefHolder.PutDefinitions(remainDefs);

                //Process FactoryBean
                var productDefs = BeanParser.GetBeanDefsFromFactoryBean(definitions);
                definitionHolder.PutDefinitions(productDefs);
                //由于信息不全，不能交给remianDef做主动创建，而是在创建Pillar时创建单例的product
            }

            //Process Bean
            {
                var definitions = BeanParser.GetBeanDefsFromMethod(configClass, configObj);
                definitionHolder.PutDefinitions(definitions);
                var remainDefs = definitions.Where(def => !def.LazyInit);
                cacheDefHolder.PutDefinitions(remainDefs);
            }
        }


        protected virtual void InitContainer()
        {
            RegisterSingleton(typeof(IContainer), "self", this);
            object? GetInFactory(Type keyType, string? name)
            {
                var def = definitionHolder.QueryDefinition(keyType, name);
                if (def == null) return null;

                return CreateBean(def);
            }
            objectFactory = new ObjectFactory(GetInFactory);
        }


        public T? Resolve<T>()
        {
            return (T)Resolve(typeof(T))!;
        }

        public object? Resolve(Type type)
        {
            var def = definitionHolder.QueryDefinition(type, null);
            return def != null ? CreateBean(def) : null;
        }

        public object? Resolve(string name)
        {
            var def = definitionHolder.QueryDefinition(null, name);
            return def != null ? CreateBean(def) : null;
        }

        public bool Inject(object obj) 
        {
            return objectFactory.InjectByRuntime(obj);
        }

        private void InjectBean(object bean, BeanDefinition definition)
        {
            if (definition.InjectMode == InjectMode.None) return;
            var targetCache = definition.IsSingleton ? singletonCache : prototypeCache;

            if (!targetCache.ContainsKey(definition))
            {
                targetCache.Put(definition, bean);
                objectFactory.InjectByPreCompile(bean, definition);
                targetCache.Remove(definition);
            }
        }

        public bool RegisterSingleton(Type keyType, string? name, object singleton)
        {
            name ??= keyType.Name.FirstLetterToLower();
            var def = new BeanDefinition(keyType, name, singleton.GetType());
            return RegisterSingleton(def, singleton);
        }

        public bool RegisterSingleton(BeanDefinition definition, object singleton)
        {
            if (!singletonObjects.ContainsKey(definition) || CanOverride)
            {
                definitionHolder.PutDefinition(definition);
                singletonObjects.Put(definition, singleton);
                return true;
            }

            return false;
        }

        public void RegisterPillar(Type type)
        {
            var pillarAtt = type.GetCustomAttribute<PillarAttribute>();
            if (pillarAtt == null) return;

            var def = BeanDefinition.CreateFromPillar(type);
            definitionHolder.PutDefinition(def);
            var needCreate = def.IsSingleton && !def.LazyInit;
            if (needCreate)
            {
                cacheDefHolder.PutDefinition(def);
            }
        }

        public void RegisterPillar<T>()
        {
            RegisterPillar(typeof(T));
        }

        public void RegisterClass(Type keyType, Type? type = null, string? name = null, bool isSingleton = true, bool lazyInit = false)
        {
            name ??= keyType.Name.FirstLetterToLower();
            type ??= keyType;
            var def = new BeanDefinition(keyType, name, type)
            {
                CreateMode = CreateMode.ByConstructor,
                IsSingleton = isSingleton,
                LazyInit = lazyInit
            };
            definitionHolder.PutDefinition(def);
            var needCreate = def.IsSingleton && !def.LazyInit;
            if (needCreate)
            {
                cacheDefHolder.PutDefinition(def);
            }
        }

        public void RegisterClass<T, U>(string? name = null, bool isSingleton = true, bool lazyInit = false)
        {
            RegisterClass(typeof(T), typeof(U), name, isSingleton, lazyInit);
        }

        public void RegisterPillars(params Type[] types)
        {
            for (int i = 0; i < types.Length; i++)
            {
                RegisterPillar(types[i]);
            }
        }

        public void RegisterBean(BeanDefinition def)
        {
            definitionHolder.PutDefinition(def);
            var needCreate = def.IsSingleton && !def.LazyInit;
            if (needCreate)
            {
                cacheDefHolder.PutDefinition(def);
            }
        }

        public void Launch()
        {
            int lastCount = cacheDefHolder.Definitions.Values.Count;
            int loopTime = 0;
            do
            {
                Launch(loopTime);
                loopTime = cacheDefHolder.Definitions.Values.Count - lastCount;
                lastCount = cacheDefHolder.Definitions.Values.Count;
            }
            while
            (lastCount != cacheDefHolder.Definitions.Values.Count);

            cacheDefHolder.ClearDefinition();
        }

        private void Launch(int start)
        {
            var remainDefs = cacheDefHolder.Definitions.Values;
            foreach (BeanDefinition def in remainDefs)
            {
                CreateBean(def);
            }
        }

        public void Close()
        {
            foreach (var (def, singleton) in singletonObjects)
            {
                if (def.ObjectType.IsAssignableTo(typeof(IBean)))
                {
                    (singleton as IBean)?.OnDestroyBean();
                }
            }

            singletonObjects.Clear();
        }

        protected object? CreateBean(BeanDefinition definition)
        {
            var objectType = definition.ObjectType;

            if (!definition.IsSingleton)
            {
                var instance = prototypeCache.GetValueOrDefault(definition);
                if (instance == null)
                {
                    instance = objectFactory.GetBean(definition);
                    if (instance == null) return null;
                    InjectBean(instance, definition);
                    IntitBean(instance, objectType);
                    ProduceFromFactoryBean(instance);
                }
                return instance;

            }

            else
            {
                var instance = singletonObjects!.GetValueOrDefault(definition, singletonCache.GetValueOrDefault(definition));
                if (instance == null)
                {
                    instance = objectFactory.GetBean(definition);
                    if (instance == null) return null;
                    InjectBean(instance, definition);
                    IntitBean(instance, objectType);
                    RegisterSingleton(definition, instance);
                    ProduceFromFactoryBean(instance);
                }
                return instance;
            }
        }


        private static void IntitBean(object bean, Type beanType)
        {
            if (beanType.IsAssignableTo(typeof(IBean)))
            {
                (bean as IBean)?.OnInitBean();
            }
        }

        protected virtual void ProduceFromFactoryBean(object bean)
        {
            var beanType = bean.GetType();
            if (BeanParser.IsFactoryBean(beanType))
            {
                var factory = (bean as FactoryBeanBase)!;
                if (!factory.IsSingleton) return;

                var keyType = bean.GetType().BaseType!.GetGenericArguments().First();
                var def = definitionHolder.QueryDefinition(keyType, null)!;
                var product = objectFactory.GetBean(def)!;
                RegisterSingleton(def, product);
            }

            if (beanType.IsAssignableTo(typeof(EnumerableFactoryBean)))
            {
                var factory = (bean as EnumerableFactoryBean)!;
                foreach (var def in factory)
                {
                    RegisterBean(def);
                }
            }

        }

    }
}