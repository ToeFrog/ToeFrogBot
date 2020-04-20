using System;
using System.Collections.Generic;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace FrogBot
{
    public class Bot
    {
        TwitchClient client;
        SoundProcessor soundProcessor = SoundProcessor.Current;

        public Bot(string clientToken)
        {
            ConnectionCredentials credentials = new ConnectionCredentials("ToeFrog", clientToken);
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };
            WebSocketClient customClient = new WebSocketClient(clientOptions);
            client = new TwitchClient(customClient);
            client.Initialize(credentials, "ToeFrog");

            client.OnLog += Client_OnLog;
            client.OnJoinedChannel += Client_OnJoinedChannel;
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnWhisperReceived += Client_OnWhisperReceived;
            client.OnNewSubscriber += Client_OnNewSubscriber;
            client.OnConnected += Client_OnConnected;
            client.OnChatCommandReceived += Client_OnChatCommandReceived;
            client.OnUserJoined += Client_OnUserJoined;
            client.OnUserLeft += Client_OnUserLeft;

            client.Connect();
        }

        private void Client_OnUserLeft(object sender, OnUserLeftArgs e)
        {
            // Say bye to people who leave the channel
            Console.WriteLine($"{e.Username} left the channel. Bye Felicia!");
        }

        private void Client_OnUserJoined(object sender, OnUserJoinedArgs e)
        {
            Console.WriteLine($"{e.Username} joined the channel. Welcome!");
        }

        private void Client_OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {
            Console.WriteLine($"Chat Command - {e.Command.ChatMessage.Username}: {e.Command.ChatMessage.Message}");

            // TODO: We may want !stop to be a mod only command. Maybe?
            if( e.Command.CommandText.ToLower() == "stop")
            {
                Console.WriteLine("Stopping sounds");
                soundProcessor.CurrentlyPlaying.Dispose();
                soundProcessor.Dispose();
            }
            else if (SoundCommand.Exists(e.Command.CommandText))
            {
                soundProcessor.Queue(new SoundCommand(e.Command.CommandText));
            }
        }

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            // Console.WriteLine($"{e.DateTime.ToString()}: {e.BotUsername} - {e.Data}");
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine($"Connected to {e.AutoJoinChannel}");
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Console.WriteLine("The bot has joined the channel.");
            client.SendMessage(e.Channel, "The bot has arrived!");
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            Console.WriteLine($"Message - {e.ChatMessage.Username}: {e.ChatMessage.Message}");
            if(e.ChatMessage.Message.ToLower().Contains("kappa"))
            {
                soundProcessor.Queue(new SoundCommand("bazinga"));
            }
        }

        private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            Console.WriteLine("Hey! We have a whisper waiting!");
        }

        private void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            if (e.Subscriber.SubscriptionPlan == SubscriptionPlan.Prime)
                client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the ToeFrog Pod! Thank you for that Twitch Prime sub!");
            else
                client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the ToeFrog Pod! I appreciate you and your sub!");
        }
    }
}