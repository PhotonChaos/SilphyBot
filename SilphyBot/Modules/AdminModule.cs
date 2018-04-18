using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;

namespace SilphyBot.Modules
{
    [Name("Admin")]
    class AdminModule : ModuleBase<SocketCommandContext> {
        [Command("ban")]
        [Name("ban")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task BanMemberAsync(ulong id, string reason = "") {
            await Context.Guild.AddBanAsync(id);
        }

        [Command("ban")]
        [Name("ban")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task BanMemberAsync(SocketGuildUser user) {
            await Context.Guild.AddBanAsync(user);
        }
    }
}
