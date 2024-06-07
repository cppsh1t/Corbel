using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Corbel.DevConsole
{
    public interface ICommandParser
    {
        IEnumerable<ICommand> Parse(Type commandType);
    }

    public class CommandParser : ICommandParser
    {
        private const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public IEnumerable<ICommand> Parse(Type commandType)
        {
            List<ICommand> commands = new List<ICommand>();
            IEnumerable<MethodInfo> commandMethodInfos = commandType.GetMethods(flags)
                    .AsEnumerable().Where(methodInfo => methodInfo.GetCustomAttribute<CommandAttribute>() != null);

            foreach (MethodInfo methodInfo in commandMethodInfos)
            {
                CommandAttribute commandAttribute = methodInfo.GetCustomAttribute<FunctionCommandAttribute>()!;
                string commandName = commandAttribute.Name == string.Empty ? $"{commandType.Name}_{methodInfo.Name}".ToLower() : commandAttribute.Name;
                ICommand functionCommand = new FunctionCommand(commandName, methodInfo, commandAttribute);
                commands.Add(functionCommand);
            }

            IEnumerable<FieldInfo> commandFieldInfos = commandType.GetFields(flags)
                    .AsEnumerable().Where(methodInfo => methodInfo.GetCustomAttribute<CommandAttribute>() != null);

            foreach (FieldInfo fieldInfo in commandFieldInfos)
            {
                CommandAttribute commandAttribute = fieldInfo.GetCustomAttribute<FieldCommandAttribute>()!;

                bool empty = commandAttribute.Name == string.Empty;
                string getterName = empty ? $"get_{commandType.Name}_{fieldInfo.Name}".ToLower() : $"get_{commandAttribute.Name}";
                string setterName = empty ? $"set_{commandType.Name}_{fieldInfo.Name}".ToLower() : $"set_{commandAttribute.Name}";
                ICommand getterCommand = new GetterCommand(getterName, fieldInfo, commandAttribute);
                ICommand setterCommand = new SetterCommand(setterName, fieldInfo, commandAttribute);
                commands.Add(getterCommand);
                commands.Add(setterCommand);
            }

            return commands;
        }
    }
}