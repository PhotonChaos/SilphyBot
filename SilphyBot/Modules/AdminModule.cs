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
        [Summary("Bans a user from the server.")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task BanMemberAsync([Summary("The ID of the user to ban")]ulong id, [Summary("The reason that the user will see")]params string[] reason) {
            string s = "";
            foreach (string ss in reason) {
                s += ss + " ";
            }

            await Context.Guild.AddBanAsync(id, reason:s);

            await ReplyAsync($"*throws hammer*\nThe person with an ID of {id} has been banned o3o");
        }

        [Command("ban")]
        [Name("ban")]
        [Summary("Bans a user from the server.")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task BanMemberAsync([Summary("The ID of the user to ban")]SocketGuildUser user, [Summary("The reason that the user will see")]params string[] reason) {
            string s = "";

            foreach(string ss in reason) {
                s += ss+" ";
            }

            await Context.Guild.AddBanAsync(user, reason:s);

            await ReplyAsync($"*throws hammer*\n{user.Username}#{user.Discriminator} has been banned o3o");
        }
    }
}
