using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Numerics;
using System.Threading;

namespace SilphyBot.Modules
{
    [Name("Basic")]
    public class BasicModule : ModuleBase<SocketCommandContext>
    {
        public static BigInteger a = 0;
        public const int maxDice = 10;
        public const ulong testGeneralId = 301897489735483392;
        public const ulong generalId = 308783710709481482;

        [Command("say")]
        [Summary("Makes silphy say a message in the #general channel")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SayAsync([Summary("The text to echo")]params string[] echo) {
            string s = "";
            foreach(string str in echo) {
                s += str + " ";
            }

            await Context.Guild.GetTextChannel(testGeneralId).SendMessageAsync(s);
        }
        
        [Command("say")]
        [Summary("Says a phrase in the channel you pass in")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task SayAsync([Summary("The text channel")]SocketTextChannel ch, [Summary("The message")][Remainder()]string echo) {
            await ch.SendMessageAsync(echo);
        }

        [Command("roll")]
        [Summary("Rolls X Y-sided dice")]
        public async Task RollAsync([Summary("The number of dice to roll")]int dice, [Summary("The number of sides on each die")]int sides) {
            string s = "";

            if (dice > maxDice) {
                await ReplyAsync("o3o I can't roll all those dice");
            } else if(sides > 1000000) {
                await ReplyAsync("I can't find a die with that many sides");
            } else {
                for (int i = 1; i <= dice; i++) {
                    Random random = new Random(DateTime.Now.Millisecond);
                    s += "🎲 I rolled a " + random.Next(1, sides + 1) + " on die " + i + "\n";
                    
                }
                
                await ReplyAsync(s);
            }
        }

        [Command("addnum")]
        [Summary("Adds to a number")]
        public async Task AddAsync([Summary("The number you want to add")]Int64 num) {
            a += num;
            await ReplyAsync("The number is now" + a + "!");
        }

        [Command("whois")]
        [Summary("Gives the username and tag of the user with that ID")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task WhoIs([Summary("The ID of the person you want to find")]ulong id) {
            SocketGuildUser user = null;

            foreach (SocketGuildUser usr in Context.Guild.Users) {
                if (usr.Id == id) {
                    user = usr;
                    break;  
                }
            }

            if(user != null) {
                await ReplyAsync("I found them!\n" + id + " is actually " + user.Username + "#" + user.Id + "!");
            } else {
                await ReplyAsync("I looked as hard as I could, but I couldnt find them ono");
            }
        }

        [Command("test")]
        [Summary("Tests if the bot is working")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task TestCommand() {
            await ReplyAsync("o3o");
        }


        [Command("randommember")]
        [Alias("rm", "random")]
        [Summary("Finds a random user on the server")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task FindRandomMemberAsync(params string[] _role) {
            string role = "";
            foreach(string r in _role) {
                role += r + ' ';
            }
            role = role.Remove(role.Length - 1);
            SocketRole rrole = null;
            foreach(SocketRole r in Context.Guild.Roles) {
                //Console.WriteLine("\"" + role + "\"");
                if(r.Name.StartsWith(role)) {
                    rrole = r;
                    Console.WriteLine("Role Found!");
                }
            }

            List<string> users = new List<string>();
            foreach (SocketGuildUser user in Context.Guild.Users) {
                if(user.Roles.Contains(rrole)) {
                    //Console.WriteLine("A");
                    users.Add(user.Username + '#' + user.Discriminator);
                }
            }
            Random randy = new Random();
            string selectedUser = users.ElementAt(randy.Next(users.Count()-1));

            await ReplyAsync("o3o I choose: \n\n" + selectedUser);
        }

        [Command("shutdown")]
        [Summary("Turn off the bot.")]
        [RequireOwner]
        public async Task Shutdown() {
            await ReplyAsync("Goodbye 💤");
            await Context.Client.LogoutAsync();

            Console.WriteLine("Bot has recieved shutdown command.");

            Environment.Exit(0);
        }
    }
}
