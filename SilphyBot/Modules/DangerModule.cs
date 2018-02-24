using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace SilphyBot.Modules
{
    [Name("Danger")]
    [Group("danger")]
    class DangerModule : ModuleBase<SocketCommandContext> {
        [Command("!!Hacked")]
        [Summary("Only use this command if the bot has been hacked. This will disconnect the bot from all servers.")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task HackedAsync() {
            await ReplyAsync("Goodbye everyone! I hope to see you all again soon!");
            Console.WriteLine("The bot has been hacked!");
            foreach (SocketGuild server in Context.Client.Guilds) {
                Console.WriteLine("Leaving {0}", server.Name);
                await server.LeaveAsync();
            }
            Console.WriteLine("The bot has successfully left all servers.");
        }
    }
}
