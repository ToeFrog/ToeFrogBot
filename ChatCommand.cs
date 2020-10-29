using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib.Api.Core.Enums;

namespace ToeFrogBot
{
    public class ChatCommand : Command
    {
        public string Name { get; set; }
        public UserType UserType { get; set; }
        public string Message { get; set; }

        public ChatCommand(string name)
        {
            // Fetch the chat command from chatCommands.json

        }

        public override void Execute()
        {

            
        }
    }
