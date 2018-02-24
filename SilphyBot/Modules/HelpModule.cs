using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace SilphyBot.Modules
{
    [Name("Help")]
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService service;

        public HelpModule(CommandService cs) {
            service = cs;
        }

        [Command("help")]
        [Summary("Displays the help menu")]
        public async Task HelpAsync() {
            EmbedBuilder builder = new EmbedBuilder() {
                Color = new Color(255, 124, 218),
                Description = "Here are all the things you can do!\n"
            };

            foreach (ModuleInfo module in service.Modules) {
                string desc = "";

                foreach (CommandInfo command in module.Commands) {
                    PreconditionResult result = await command.CheckPreconditionsAsync(Context);

                    if (result.IsSuccess) {
                        desc += $"**{command.Aliases.First()}**";

                        for (int i = 0; i < command.Parameters.Count(); i++) {
                            desc += $" [{command.Parameters.ElementAt(i).Name}] ";
                        }



                        desc += "\n";
                    }
                }
                desc += "\n\n\n  ";

                if (!string.IsNullOrWhiteSpace(desc)) {
                    builder.AddField(x => {
                        x.Name = module.Name;
                        x.Value = desc;
                        x.IsInline = false;
                    });
                }
            }
            await Context.Message.Author.SendMessageAsync("", false, builder.Build());
        }

        [Command("help")]
        [Summary("Displays help for a command")]
        public async Task HelpCommandAsync([Summary("The name of the command to get help for.")]string commandName) {
            foreach (ModuleInfo m in service.Modules) {
                foreach (CommandInfo c in m.Commands) {
                    if (c.Name == commandName) {
                        EmbedBuilder builder = new EmbedBuilder() {
                            Color = new Color(255, 124, 218),
                            Description = $"Info for the {commandName} command:\n"
                        };

                        string desc = c.Summary + "\n";

                        for(int i = 0; i < c.Parameters.Count(); i++) {
                            desc += $"{c.Parameters.ElementAt(i).Name}: {c.Parameters.ElementAt(i).Summary}\n";
                        }

                        builder.AddField(x => {
                            x.Name = m.Name + " " + c.Name;
                            x.Value = desc;
                            x.IsInline = false;
                        });

                        await Context.Message.Author.SendMessageAsync("", false, builder.Build());
                    }
                }
            }
        }
    }
}
