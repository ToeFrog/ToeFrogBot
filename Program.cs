using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TwitchLib.Client.Extensions;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Users;
using TwitchLib.Api.V5.Models.Subscriptions;
using System;
using System.IO;
using System.Text.Json;
using System.Reflection;
using System.Linq;

namespace ToeFrogBot
{
    public class Program
    {
        private static void Main()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSecrets.json", optional: false, reloadOnChange: true);
            // TODO: Register custom userSounds.json 
            IConfigurationRoot configuration = builder.Build();

            var clientToken = configuration.GetSection("Twitch")["clientToken"];

            // TODO: Get command types to dynamically load
            // Hint - https://github.com/tbd-develop/tbddotbot/blob/8903b6e03bb65fa296cc90248566823c220fd178/twitchstreambot/infrastructure/DependencyInjection/Container.cs#L74

            // Look for all of our command classes and then load their configurations
            //var commandTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.BaseType == (typeof(Command)));

            //foreach(var type in commandTypes)
            //{
            //    var fileName = type.Name + ".json";
            //    var genericType = type.MakeGenericType();

            //    var commands = LoadConfiguration<genericType>(fileName);
            //}


            // Deserialize chat commands from configuration
            var chatCommandsJson = File.ReadAllText("chatCommands.json");
            var chatCommands = JsonSerializer.Deserialize<List<ChatCommand>>(chatCommandsJson);

            Bot bot = new Bot(clientToken);
            // TODO: UserSounds is a type of sound command. Need to have that bound
            // so that it can be loaded using reflection
            bot.UserSounds = LoadConfiguration<UserSound>("userSounds.json");
            bot.ChatCommands = LoadConfiguration<ChatCommand>("chatCommands.json");
            Console.ReadLine();
        }

        // HACK: You can do this and it's better than having wet code,
        // but it's still not optimal. You SHOULD use reflection.
        public static List<T> LoadConfiguration<T>(string file)
        {
            var json = File.ReadAllText(file);
            var list = JsonSerializer.Deserialize<List<T>>(json);

            return list;
        }
    }
}