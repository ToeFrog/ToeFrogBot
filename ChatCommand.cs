using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Text;
using System.Text.Json;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Client;

namespace ToeFrogBot
{
    public class ChatCommand : ICommand
    {
        public string Name { get; set; }
        public UserType UserType { get; set; }
        public string Message { get; set; }
        public string[] Parameters { get; set; }

        public ChatCommand() { }

        public ChatCommand(string name)
        {
            ChatCommand command;

            if (!ChatCommand.Exists(name, out command))
            {
                throw new Exception($"The ChatCommand {name} could not be found");
            }

            this.Name = command.Name;
            this.UserType = command.UserType;
            this.Message = command.Message;
            this.Parameters = command.Parameters;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public static bool Exists(string name, out ChatCommand command)
        {
            var commands = LoadCommands();
            command = commands.Where(c => c.Name == name).FirstOrDefault();

            return command != null;
        }

        public void Execute()
        {
            Program.Bot.Client.SendMessage("ToeFrog", string.Format(this.Message, this.Parameters));
        }

        private static List<ChatCommand> LoadCommands()
        {
            var json = File.ReadAllText("chatCommands.json");
            return JsonSerializer.Deserialize<List<ChatCommand>>(json);
        }
    }
}