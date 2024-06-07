using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Corbel.DevConsole
{
    public interface ICommandGenerator 
    { 
        Dictionary<string, List<ICommand>> Generate();
    }

    public class CommandGenerator : ICommandGenerator
    {
        private readonly ICommandParser commandParser;

        public CommandGenerator(ICommandParser commandParser) => this.commandParser = commandParser;

        public CommandGenerator(): this(new CommandParser()) { }

        public Dictionary<string, List<ICommand>> Generate()
        {
            IEnumerable<Type> types = Assembly.GetExecutingAssembly().GetTypes().Where(type => type.GetCustomAttribute<CommandObjectAttribute>() != null);
            Dictionary<string, List<ICommand>> map = new Dictionary<string, List<ICommand>>();
            foreach(Type type in types) 
            {
                IEnumerable<ICommand> commands = commandParser.Parse(type);
                foreach(ICommand command in commands)
                {
                    if (map.ContainsKey(command.Name))
                    {
                        map[command.Name].Add(command);
                    }
                    else 
                    {
                        map.Add(command.Name, new List<ICommand>(){command});
                    }
                }
            }

            return map;
        }
    }
}