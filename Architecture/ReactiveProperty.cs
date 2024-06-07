using System;

namespace Corbel
{
    public interface IReactiveProperty<T>
    {
        void SetValueNotReactive(T newValue);
        void InvokeCallBack();
        IUnRegister Register(Action<T> callback);
        void UnRegister(Action<T> callback);
    }

    public class ReactiveProperty<T> : IReactiveProperty<T>, ICharacterProperty
    {
        protected T insideValue;
        protected Action<T> onChanged = v => { };

        public T Value
        {
            get => insideValue;
            set
            {
                if (value == null && insideValue == null) return;
                if (value != null && value.Equals(insideValue)) return;

                insideValue = value;
                onChanged.Invoke(value);
            }
        }

        public ReactiveProperty(T initValue)
        {
            insideValue = initValue;
        }

        public ReactiveProperty(T initValue, Action<T> onChanged)
        {
            insideValue = initValue;
            this.onChanged += onChanged;
        }

        public void SetValueNotReactive(T newValue)
        {
            insideValue = newValue;
        }

        public void InvokeCallBack()
        {
            onChanged.Invoke(Value);
        }

        public IUnRegister Register(Action<T> callback)
        {
            UnRegistration unRegistration = new(() => UnRegister(callback));
            onChanged += callback;
            return unRegistration;
        }

        public void UnRegister(Action<T> callback)
        {
            onChanged -= callback;
        }

        public static implicit operator T(ReactiveProperty<T> property)
        {
            return property.Value;
        }

        public override string ToString()
        {
            return Value?.ToString() ?? string.Empty;
        }

        public void Clear()
        {
            onChanged = null!;
        }
    }
}