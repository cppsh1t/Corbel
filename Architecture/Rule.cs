#pragma warning disable CS8618
#nullable enable

using UnityEngine;
using Corbel.EventBus;
using Corbel.Extension;
using Corbel.IOC;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Corbel
{

    public interface IRegulator : ISubordinate
    {

    }

    public interface ISystem : IEmitter, IBeanQuester
    {

    }

    public interface IModel : IEmitter, IBeanQuester
    {

    }

    public interface IActor : IEmitter, IBeanQuester
    {

    }

    public interface IService : IBeanQuester
    {
        //传播级别
    }

    public interface IPropertyModel : IEmitter, IBeanQuester
    {

    }



    public abstract class System : ISystem
    {
        internal virtual void Init()
        {

        }

        public abstract Regulator GetRegulator();
    }

    public abstract class Model : IModel
    {

        internal virtual void Init()
        {

        }
        public abstract Regulator GetRegulator();
    }


    public abstract class Actor : MonoBehaviour, IActor
    {
        public virtual ActionModel ActionModel {get; protected set;} = new ActionModel();
        public virtual PropertyModel? GetPropertyModel()
        {
            return null;
        }

        public abstract Regulator GetRegulator();

    }


    public abstract class Service : IService
    {
        protected Regulator regulator;

        public Service(Regulator regulator) => this.regulator = regulator;

        public abstract void Execute();

        public Regulator GetRegulator()
        {
            return regulator;
        }
    }

    public abstract class Service<TResult> : IService
    {
        protected Regulator regulator;

        public Service(Regulator regulator) => this.regulator = regulator;

        public abstract TResult Execute();

        public Regulator GetRegulator()
        {
            return regulator;
        }
    }

    public class PropertyModel : IPropertyModel
    {
        protected Dictionary<Type, ICharacterProperty> modelProperties = new();
        public Regulator Regulator { get; set; }
        public ICollection<ICharacterProperty> Values => modelProperties.Values;

        public void AddProperty(ICharacterProperty property)
        {

            modelProperties.Put(property.GetType(), property);
        }

        public void AddProperty(ICollection<ICharacterProperty> properties)
        {
            foreach (ICharacterProperty prop in properties)
            {
                modelProperties.Put(prop.GetType(), prop);
            }
        }

        public void AddProperty(IEnumerable<ICharacterProperty> properties)
        {
            foreach (var prop in properties)
            {
                modelProperties.Put(prop.GetType(), prop);
            }
        }

        public void AddProperty(params ICharacterProperty[] properties)
        {
            AddProperty(properties as ICollection<ICharacterProperty>);
        }

        public Regulator GetRegulator()
        {
            return Regulator;
        }

        public T? Query<T>() where T : class, ICharacterProperty
        {
            return modelProperties.GetValueOrDefault(typeof(T)) as T;
        }

        public void Remove<T>()
        {
            modelProperties.Remove(typeof(T));
        }

        public void Remove(Type type)
        {
            modelProperties.Remove(type);
        }

        public void Clear()
        {
            modelProperties.Clear();
        }
    }

    public interface IEmitter : ISubordinate
    {
        //publish, subsribe 有不同传播级别
    }

    public interface IBeanQuester : ISubordinate
    {
        //getBean, 传播级别
    }

    public interface ISubordinate
    {
        Regulator GetRegulator();
    }

}
