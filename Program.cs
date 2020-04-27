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

            // Deserialize user sounds from configuration
            var userSoundsJson = File.ReadAllText("userSounds.json");
            var userSounds = JsonSerializer.Deserialize<List<UserSound>>(userSoundsJson);

            Bot bot = new Bot(clientToken);
            bot.UserSounds = userSounds;
            Console.ReadLine();
        }
    }
}