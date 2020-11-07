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
using System.Net.Http;
using System.Runtime.CompilerServices;

namespace ToeFrogBot
{
    public class Program
    {
        public static Bot Bot { get; set; }

        private static void Main()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSecrets.json", optional: false, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            var clientToken = configuration.GetSection("Twitch")["clientToken"];

            Bot = new Bot(clientToken);

            foreach (var ct in Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.GetInterfaces().Contains(typeof(ICommand))))
            {
                Bot.CommandTypes.Add(ct);
            }

            Console.ReadLine();
        }
    }
}