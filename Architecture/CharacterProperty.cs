using System;

namespace Corbel
{

    public interface ICharacterProperty
    {
    }


    public abstract class CharacterProperty<T> : ICharacterProperty
    {
        public virtual T Value { get; set; }
        public Type PropertyType { get; }

        public CharacterProperty(T value)
        {
            Value = value;
            PropertyType = typeof(T);
        }

        public static implicit operator T(CharacterProperty<T> property)
        {
            return property.Value;
        }

        public override string ToString()
        {
            return Value?.ToString() ?? "";
        }
    }

}