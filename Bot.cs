using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace ToeFrogBot
{
    public class Bot
    {
        private SoundProcessor _soundProcessor = SoundProcessor.Current;
        private List<Type> _commandTypes = null;

        public TwitchClient Client { get; private set; }
        public string ChannelName { get; private set; }

        public List<Type> CommandTypes
        {
            get
            {
                if (this._commandTypes == null)
                {
                    this._commandTypes = new List<Type>();
                }

                return this._commandTypes;
            }
        }

        public Bot(string clientToken)
        {
            ConnectionCredentials credentials = new ConnectionCredentials("ToeFrog", clientToken);
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };
            WebSocketClient customClient = new WebSocketClient(clientOptions);
            Client = new TwitchClient(customClient);
            Client.Initialize(credentials, "ToeFrog");

            Client.OnLog += Client_OnLog;
            Client.OnJoinedChannel += Client_OnJoinedChannel;
            Client.OnMessageReceived += Client_OnMessageReceived;
            Client.OnWhisperReceived += Client_OnWhisperReceived;
            Client.OnNewSubscriber += Client_OnNewSubscriber;
            Client.OnConnected += Client_OnConnected;
            Client.OnChatCommandReceived += Client_OnChatCommandReceived;
            Client.OnUserJoined += Client_OnUserJoined;
            Client.OnUserLeft += Client_OnUserLeft;
            Client.OnWhisperSent += Client_OnWhisperSent;

            Client.Connect();
        }

        private void Client_OnUserLeft(object sender, OnUserLeftArgs e)
        {
            // Say bye to people who leave the channel
            Console.WriteLine($"{e.Username} left the channel. Bye Felicia!");
        }

        private void Client_OnUserJoined(object sender, OnUserJoinedArgs e)
        {
            Console.WriteLine($"{e.Username} joined the channel. Welcome!");

            // REVIEW: See if this is a feature that we still want
            //// Check userSounds to see if the user has a sound file
            //var userSoundsJson = File.ReadAllText("userSounds.json");
            //var userSounds = JsonSerializer.Deserialize<List<UserSound>>(userSoundsJson);

            //UserSound userSound = userSounds.Find(s => s.Username.ToLower() == e.Username.ToLower());
            //if (userSound != null)
            //{
            //    // User was found, play the sound
            //    if (SoundCommand.Exists(userSound.Sound))
            //    {
            //        soundProcessor.Queue(new SoundCommand(userSound.Sound));
            //    }
            //}
        }

        private void Client_OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {
            Console.WriteLine($"Chat Command - {e.Command.ChatMessage.Username}: {e.Command.ChatMessage.Message}");

            // If stop is used, kill what is currently playing and dispose. 
            if (e.Command.CommandText.ToLower() == "stop" && e.Command.ChatMessage.UserType >= UserType.Moderator)
            {
                _soundProcessor.CurrentlyPlaying?.Dispose();
                _soundProcessor.Dispose();
            }
            else
            {
                // Loop through all of the items in Bot.CommandTypes and figure out if this command
                // exists in any of them.
                foreach (var c in this.CommandTypes)
                {
                    var method = c.GetMethod("Exists");
                    object[] methodParams = { e.Command.CommandText, null };
                    ICommand cmd = null;
                    bool exists = (bool)method.Invoke(null, methodParams);

                    if (exists)
                    {
                        cmd = (ICommand)methodParams[1];

                        // Doing this because right now ChatCommands are the only ones with parameters
                        // This could change in the future and will need to be elevated to the interface
                        if (c.Name == "ChatCommand")
                            ((ChatCommand)cmd).Parameters = e.Command.ChatMessage.Message.Split(' ').Skip(1).ToArray();

                        if (cmd.GetType() == typeof(SoundCommand))
                        {
                            _soundProcessor.Queue((SoundCommand)cmd);
                            break;
                        }
                        else
                        {
                            cmd.Execute();
                            break;
                        }
                    }
                }
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
            Client.SendMessage(e.Channel, "The bot has arrived!");
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            Console.WriteLine($"Message - {e.ChatMessage.Username}: {e.ChatMessage.Message}");
            if(e.ChatMessage.Message.ToLower().Contains("kappa"))
            {
                _soundProcessor.Queue(new SoundCommand("bazinga"));
            }
        }

        private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            Console.WriteLine("Hey! We have a whisper waiting!");
        }

        private void Client_OnWhisperSent(object sender, OnWhisperSentArgs e)
        {
            Console.WriteLine($"A whisper was just sent to {e.Username}");
        }

        private void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            if (e.Subscriber.SubscriptionPlan == SubscriptionPlan.Prime)
                Client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the ToeFrog Pod! Thank you for that Twitch Prime sub!");
            else
                Client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the ToeFrog Pod! I appreciate you and your sub!");
        }
    }
}