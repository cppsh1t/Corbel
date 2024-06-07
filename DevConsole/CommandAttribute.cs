#nullable enable
using System;
using System.Collections.Generic;

namespace Corbel.DevConsole
{

    public abstract class CommandAttribute : Attribute
    {
        public string Name { get; private set; } = string.Empty;
        public string? SuccessMessage { get; private set; }
        public string? ErrorMessage { get; private set; }

        public CommandAttribute() { }

        public CommandAttribute(string name, string successMessage, string errorMessage)
        {
            Name = name;
            SuccessMessage = successMessage;
            ErrorMessage = errorMessage;
        }

        public CommandAttribute(string name) : this(name, null!, null!) { }

        public CommandAttribute(string name, string successMessage) : this(name, successMessage, null!) { }

    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class FunctionCommandAttribute : CommandAttribute
    {
        public FunctionCommandAttribute() : base() { }

        public FunctionCommandAttribute(string name) : base(name)
        {
        }

        public FunctionCommandAttribute(string name, string successMessage) : base(name, successMessage)
        {
        }

        public FunctionCommandAttribute(string name, string successMessage, string errorMessage) : base(name, successMessage, errorMessage)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class FieldCommandAttribute : CommandAttribute
    {
        public FieldCommandAttribute() : base() { }

        public FieldCommandAttribute(string name) : base(name)
        {
        }

        public FieldCommandAttribute(string name, string successMessage) : base(name, successMessage)
        {
        }

        public FieldCommandAttribute(string name, string successMessage, string errorMessage) : base(name, successMessage, errorMessage)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CommandObjectAttribute : Attribute
    {

    }
}