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

namespace FrogBot
{
    public class Program
    {
        private static void Main()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSecrets.json", optional: false, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            var clientToken = configuration.GetSection("Twitch")["clientToken"];

            Bot bot = new Bot(clientToken);
            Console.ReadLine();
        }
    }
}