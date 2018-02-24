using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using SilphyBot.Properties;

namespace SilphyBot
{
    class Program
    {
        /// <summary>
        /// This is the main class that the bot will run in.
        /// Some bots may only have one class, as it is possible to have all commands implemented manually.
        /// This is generally not a good idea as it can lead to source files with over ten thousand lines.
        /// The most common practice is to split the mot into multiple classes, or "Modules," which allow for
        /// easier portability of features, as well as an easier way to make commands with aliases, permissions, etc.
        /// The main and startup methods are the hardest parts of the entire bot, but lickily you can just copy-paste most of it.
        /// </summary>

        #region Variables
        private const string token = BotData.Token; // This is the bot token. It is located in the BotData class which has not been published to this repository for security reasons.
        private const ulong testSilphyChannelID = BotData.testChannelID;
        private const ulong silphyChannelID = BotData.silphyChannelID;
        private const string prefix = "!"; // the bot command prefix
        private const bool noPrefix = false; // true if the prefix is not used, false otherwise
        private const bool sayErrors = false; // should the bot say errors in discord
        private static bool testMode = Settings.Default.testMode; // this is to set the testing mode. true is for on, false is for off.
        private static bool releaseMode = Settings.Default.releaseMode; // this is so that the bot can be released as an exe with a strikes file, instead of moving it to the directory

        private CommandService _commands;
        private DiscordSocketClient _client;
        private IServiceProvider _services;

        public enum SLogType
        {
            Normal, 
            Log,
            Error,
            Warning
        }
        #endregion
        #region Methods
        private static void Main(string[] args) {
            if (testMode) {
                Console.Title = "Silphy Bot - Test Mode";
            } else {
                Console.Title = "Silphy Bot";
            }
           
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit); // This calls the CurrentDomain_ProcessExit() method when the program closes

            Setup(); // This is just a method that I put in to sort my code. This is not necessary.

            // The actual bot starts after this point
            new Program().StartAsync().GetAwaiter().GetResult();
        }

        private static void Setup() {
            Modules.StrikesModule.LoadData();
        }

        // This is the startup method for the Discord part of the bot
        public async Task StartAsync() {
            _client = new DiscordSocketClient();
            _commands = new CommandService();

            _services = new ServiceCollection().AddSingleton(_client).AddSingleton(_commands).BuildServiceProvider();
            await InstallCommandsAsync();


            _client.Log += Log;

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        // This adds the commads and the modules
        public async Task InstallCommandsAsync() {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly()); // this is what searches the program for module classes
        }

        private async Task HandleCommandAsync(SocketMessage messageParam) {
            SocketUserMessage message = messageParam as SocketUserMessage;

            if (message == null) return; // Null Checking
            if (message.Author.IsBot) return;
            if((message.Channel.Id != silphyChannelID && !testMode) || (message.Channel.Id != testSilphyChannelID && testMode)) return;
            int argPos = 0;
            if (!(message.HasStringPrefix(prefix, ref argPos) && !noPrefix)) return;
            SocketCommandContext context = new SocketCommandContext(_client, message);

            IResult result = await _commands.ExecuteAsync(context, argPos, _services); // This is what calls the command methods

            // The error handling
            if(!result.IsSuccess) {
                string error = result.ErrorReason;
                CommandError? e = result.Error;
                if (sayErrors) {
                    // This is a config oprion that says if the bot will give a response in discord for command errors
                    if (e == CommandError.BadArgCount) {
                        await context.Channel.SendMessageAsync("0-0 too many things");
                    } else if (e == CommandError.UnknownCommand) {
                        await context.Channel.SendMessageAsync("mow");
                    } else if (e == CommandError.MultipleMatches) {
                        await context.Channel.SendMessageAsync("These instructions are confusing");
                    } else if(e == CommandError.UnmetPrecondition) {
                        await context.Channel.SendMessageAsync("o3o I'm not supposed to let you do that");
                    } else {
                        await context.Channel.SendMessageAsync("something broke and i can't fix ono");
                    }
                }
                SLog(context, SLogType.Error, error); // log the error in the console window
            }
        }

        private async Task SyncCommands(SocketGuild guild) {
            StreamReader sr = new StreamReader(BotData.dateFilePath);
            while(sr.EndOfStream) {
                string s = sr.ReadLine();

                if (s == null) break;

                string[] ss = s.Split(BotData.splitChar.ToCharArray()[0]);
                int[] q = new int[6];
                
                for(int i = 0; i < ss.Length; i++) {
                    q[i] = Convert.ToInt32(ss[i]);
                }

                DateTime dt = new DateTime(q[0], q[1], q[2], q[3], q[4], q[5]);

                // Console.WriteLine($"TEST: {guild.GetTextChannel(BotData.testChannelID).GetMessagesAsync(100).ElementAt(0).ToString()}");
                
            }
            sr.Close();
            File.WriteAllText(BotData.dateFilePath, String.Empty);
            // await HandleCommandAsync();
        }

        private Task Log(LogMessage msg) {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private static void SLog(SocketCommandContext ctx, SLogType type, string text) {
            string chnl = ctx.Message.Channel.Name;
            string user = ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator;
            string format = "";

            if(type == SLogType.Error) {
                format = chnl + ":" + user + "|" + " ERROR: " + text;
            } else {
                format = chnl + ":" + user + "|" + text;
            }

            Console.WriteLine(format);
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e) {
            DateTime dt = DateTime.Now;

            int year = dt.Year;
            int month = dt.Month;
            int day = dt.Day;
            int hour = dt.Hour;
            int minute = dt.Minute;
            int second = dt.Second;
            File.WriteAllText(BotData.dateFilePath, $"{year}|{month}|{day}|{hour}|{minute}|{second}");
        }
        #endregion
    }
}
