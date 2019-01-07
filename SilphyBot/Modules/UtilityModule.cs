using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord.Commands;
using Discord;

namespace SilphyBot.Modules
{
    public class UtilityModule : ModuleBase<SocketCommandContext> {
        private CommandService _cs;
        private IServiceProvider _sp;

        public UtilityModule(CommandService cs, IServiceProvider sp) {
            _cs = cs;
            _sp = sp;
        }

        [Command("updatecommands")]
        [Summary("Scans all of the messages I have missed and executes any commmands that I find")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task UpdateCommandsAsync() {
            await ReplyAsync("I'm looking for commands");

            foreach (IMessage x in Context.Channel.GetMessagesAsync(10).Flatten().Result.ToList<IMessage>()) {
                if(x.ToString().StartsWith(Program.prefix)) {
                    int argPos = 0;
                    await _cs.ExecuteAsync(Context, argPos, _sp);
                }
            }
        }
    }
}
