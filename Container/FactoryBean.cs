#nullable enable

namespace Corbel.IOC
{
    public abstract class FactoryBeanBase
    {
        internal bool HasInit { get; set; } = false;

        public virtual bool IsSingleton { get; protected set; } = true;
        public virtual string? BeanName { get; protected set; }

        public abstract object GetObject();
    }

    public abstract class FactoryBean<T> : FactoryBeanBase
    {

    }

}