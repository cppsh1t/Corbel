using Corbel.Extension;
using System;
using System.Collections.Generic;

namespace Corbel
{

    public interface IUnRegister
    {
        void UnRegister();
    }
    public struct UnRegistration : IUnRegister
    {
        private Action? onUnRegister;
        public UnRegistration(Action onUnRegister) => this.onUnRegister = onUnRegister;

        public void UnRegister()
        {
            onUnRegister?.Invoke();
            onUnRegister = null;
        }
    }

    public class SetUnRegistration : IUnRegister
    {
        private readonly HashSet<IUnRegister> unRegistrations = new();

        public SetUnRegistration() { }

        public SetUnRegistration(params IUnRegister[] unRegisters)
        {
            AddIUnRegister(unRegisters);
        }

        public SetUnRegistration(SetUnRegistration other, params IUnRegister[] unRegisters)
        {
            var unRegistrations = other.unRegistrations;
            this.unRegistrations.AddAll(unRegistrations);
            AddIUnRegister(unRegisters);
        }

        public void AddIUnRegister(IUnRegister unRegister)
        {
            unRegistrations.Add(unRegister);
        }

        public void AddIUnRegister(params IUnRegister[] unRegisters)
        {
            unRegistrations.AddAll(unRegisters);
        }

        public void UnRegister()
        {
            foreach (var unRegistration in unRegistrations)
            {
                unRegistration.UnRegister();
            }
        }
    }
}