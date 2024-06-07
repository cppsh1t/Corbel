#nullable enable
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Corbel.DevConsole
{
    public interface ICommand
    {
        public string Name { get; }
        bool Validate(string[]? originArguments, out object[]? arguments);
        bool Excute(object invoker, object[]? arguments, out string? successMessage, out string? errorMessage);
    }

    public class FunctionCommand : ICommand
    {
        private readonly MethodInfo methodInfo;
        private readonly CommandAttribute commandInfo;
        private readonly ParameterInfo[] paramInfos;

        public string Name => name;
        public string name;

        public FunctionCommand(string name, MethodInfo methodInfo, CommandAttribute commandInfo)
        {
            this.methodInfo = methodInfo;
            this.commandInfo = commandInfo;
            paramInfos = methodInfo.GetParameters();
            this.name = name;
        }

        public bool Validate(string[]? originArguments, out object[]? arguments)
        {
            arguments = null;
            if (originArguments == null && paramInfos.Length == 0) return true;
            if (originArguments == null || originArguments.Length != paramInfos.Length) return false;

            arguments = new object[paramInfos.Length];
            try
            {
                for (int i = 0; i < originArguments.Length; i++)
                {
                    object casted = Convert.ChangeType(originArguments[i], paramInfos[i].ParameterType);
                    arguments![i] = casted;
                }
                return true;
            }
            catch
            {
                arguments = null;
                return false;
            }
        }

        public bool Excute(object invoker, object[]? arguments, out string? successMessage, out string? errorMessage)
        {
            string dateString = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                object? result = methodInfo.Invoke(invoker, arguments);

                if (methodInfo.ReturnType != typeof(void))
                {
                    successMessage = $"[{dateString}]-[success-func]-[{Name}] -- [{commandInfo.SuccessMessage}] -- [{result}]";
                }
                else
                {
                    successMessage = $"[{dateString}]-[success-func]-[{Name}] -- [{commandInfo.SuccessMessage}]";
                }
                errorMessage = string.Empty;
                return true;
            }
            catch
            {
                successMessage = string.Empty;
                errorMessage = $"[{dateString}]-[failure-func]-[{Name}] -- [{commandInfo.ErrorMessage}]";
                return false;
            }
        }
    }

    public class GetterCommand : ICommand
    {
        private readonly FieldInfo fieldInfo;
        private readonly CommandAttribute commandInfo;
        public string Name => name;
        public string name;

        public GetterCommand(string name, FieldInfo fieldInfo, CommandAttribute commandInfo)
        {
            this.fieldInfo = fieldInfo;
            this.commandInfo = commandInfo;
            this.name = name;
        }

        public bool Validate(string[]? originArguments, out object[]? arguments)
        {
            arguments = null;
            return true;
        }

        public bool Excute(object invoker, object[]? arguments, out string? successMessage, out string? errorMessage)
        {
            string dateString = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                object? result = fieldInfo.GetValue(invoker);
                successMessage = $"[{dateString}]-[success-getter]-[{Name}] -- [{commandInfo.SuccessMessage}] -- [{result}]";
                errorMessage = string.Empty;
                return true;
            }
            catch
            {
                successMessage = string.Empty;
                errorMessage = $"[{dateString}]-[failure]-[{Name}] -- [{commandInfo.ErrorMessage}]";
                return false;
            }
        }
    }

    public class SetterCommand : ICommand
    {
        private readonly FieldInfo fieldInfo;
        private readonly CommandAttribute commandInfo;
        public string Name => name;
        public string name;

        public SetterCommand(string name, FieldInfo fieldInfo, CommandAttribute commandInfo)
        {
            this.fieldInfo = fieldInfo;
            this.commandInfo = commandInfo;
            this.name = name;
        }

        public bool Validate(string[]? originArguments, out object[]? arguments)
        {
            arguments = null;
            if (originArguments == null || originArguments.Length == 0) return false;

            arguments = new object[1];
            try
            {
                object result = Convert.ChangeType(originArguments[0], fieldInfo.FieldType);
                arguments = new object[] { result };
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Excute(object invoker, object[]? arguments, out string? successMessage, out string? errorMessage)
        {
            string dateString = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                object? oldValue = fieldInfo.GetValue(invoker);
                fieldInfo.SetValue(invoker, arguments![0]);
                string resultString = $"{oldValue} to {arguments![0]}";
                successMessage = $"[{dateString}]-[success-getter]-[{Name}] -- [{commandInfo.SuccessMessage}] -- [{resultString}]";
                errorMessage = string.Empty;
                return true;
            }
            catch
            {
                successMessage = string.Empty;
                errorMessage = $"[{dateString}]-[failure]-[{Name}] -- [{commandInfo.ErrorMessage}]";
                return false;
            }
        }
    }
}