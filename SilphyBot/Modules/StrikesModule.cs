using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using Discord.WebSocket;
using System.Text.RegularExpressions;

namespace SilphyBot.Modules
{
    [Name("Strikes")]
    [Group("strike")]
    public class StrikesModule : ModuleBase<SocketCommandContext>
    {
        /// <summary>
        /// This is the code for the strikes module.
        /// The Name attribute is the name of the module that shows up in the help list.
        /// The Group attribute is the group the commands are in. (for subcommands)
        /// </summary>
        #region Variables
        private static Dictionary<ulong, int> dict = new Dictionary<ulong, int>();
        private static List<ulong> ids = new List<ulong>();
        private static List<string> nams = new List<string>(); 

        private const string splitChar = BotData.splitChar;
        private const string strikeDataPath = BotData.strikeFilePath;

        private const int spacs = 32;
        #endregion
        #region Methods
        public SocketGuildUser FindUser(SocketGuild server, string username = null) {
            if (Regex.IsMatch(username, @".*#\d{4}$")) {
                foreach (SocketGuildUser usr in server.Users) {
                    if (usr.Username == username && usr.Discriminator == username.Substring(username.Length-4)) {
                        return usr;
                    }
                }
            }

            List<SocketGuildUser> userList = new List<SocketGuildUser>();

            foreach (SocketGuildUser usr in server.Users) {
                if (usr.Username == username) {
                    userList.Add(usr);
                }
            }
            if (userList.Count != 1) {
                return null;
            } else {
                return userList.First();
            }
        }

        public static void LoadData() {
            StreamReader sr = new StreamReader(strikeDataPath);
            ulong id = 0;
            int strikes = 0;
            while (!sr.EndOfStream) {
                string s = sr.ReadLine();

                if(s == null) continue;
                if (!(s.Contains(splitChar))) continue;

                string[] ss = s.Split(splitChar.ToCharArray()[0]);
                id = Convert.ToUInt64(ss[0]);
                strikes = Convert.ToInt32(ss[1]);
                nams.Add(ss[2]);

                dict.Add(id, strikes);
            }
            ids.AddRange(dict.Keys);
            sr.Close();
        }

        public static void SaveData() {
            File.WriteAllText(strikeDataPath, String.Empty);
            StreamWriter sw = new StreamWriter(strikeDataPath);
            ulong id;
            int strikes;

            File.SetAttributes(strikeDataPath, FileAttributes.Normal);

            int i = 0;
            foreach (KeyValuePair<ulong, int> kv in dict) {
                id = kv.Key;
                strikes = kv.Value;

                sw.WriteLine("{1}{0}{2}{0}{3}", splitChar, id, strikes, nams.ElementAt(i));
                i++;
            }
            sw.Close();
        }
        #endregion
        #region Commands
        [Command("add")]
        [Summary("Adds 1 strike to the specified user.")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task AddStrike([Summary("The user to strike.")] SocketUser user = null) {
            if (user == null) {
                await ReplyAsync("Who?");
            } else {
                ulong id = user.Id;

                if(!(ids.Contains(id))) {
                    ids.Add(id);
                    nams.Add(user.Username + "#" + user.Discriminator);
                    dict.Add(id, 0);
                }

                dict[id] += 1;
                string s = (dict[id] == 1) ? "" : "s";
                await Context.Channel.SendMessageAsync("*scratches in a tally*\n" + user.Username + " now has " + dict[id] + " strike" + s + " o3o");

                SaveData();
            }
        }

        [Command("add")] 
        [Summary("Adds 1 strike to the specified user.")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task AddStrike([Summary("The name of the user to strike")] params string[] username) {
            if (username.Count() <= 0) {
                await ReplyAsync("Who?");
            } else {
                string nname = "";
                foreach(string nam in username) {
                    nname += nam + " ";
                }
                nname = nname.Substring(0, nname.Length - 1);

                SocketGuildUser user = FindUser(Context.Guild, nname);
                if(user == null) {
                    await ReplyAsync("There could be many people named that, or there could be no one ouo.\nPlease say their name again with their 4 digit tag,"+
                        " like `username#0000`. \nIf that was their name and number, I can't find them :p.\nIf the user is a tricky trickster and has #XXXX as a part of the username, make sure to put " +
                        "their real tag at the end!");
                } else {
                    ulong id = user.Id;

                    if (!(ids.Contains(id))) {
                        ids.Add(id);
                        nams.Add(user.Username + "#" + user.Discriminator);
                        dict.Add(id, 0);
                    }

                    dict[id] += 1;
                    string s = (dict[id] == 1) ? "" : "s";
                    await Context.Channel.SendMessageAsync("*scratches in a tally*\n" + user.Username + " now has " + dict[id] + " strike" + s + " ovo");
                }

                SaveData();
            }
        }

        [Command("remove")]
        [Summary("Removes 1 strike from the specified user.")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task RemoveStrike([Summary("The name of the user to remove the strike from")] SocketGuildUser user = null) {
            if(user == null) {
                await ReplyAsync("Who?");
            } else {
                ulong id = user.Id;

                if (!(ids.Contains(id))) {
                    ids.Add(id);
                    nams.Add(user.Username + "#" + user.Discriminator);
                    dict.Add(id, 0);
                }

                dict[id] -= 1;
                string s = (dict[id] == 1) ? "" : "s";
                await Context.Channel.SendMessageAsync("*rubs out a tally*\n" + user.Username + " now has " + dict[id] + " strike" + s + " o3o");

                SaveData();
            }
        }

        [Command("remove")]
        [Summary("Removes 1 strike from the specified user.")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task RemoveStrike([Summary("The name of the user to remove the strike from")] params string[] name) {
            if (name.Count() <= 0) {
                await ReplyAsync("Who?");
            } else {
                string nname = "";
                foreach (string nam in name) {
                    nname += nam + " ";
                }

                nname = nname.Substring(0, nname.Length - 1);

                SocketGuildUser user = FindUser(Context.Guild, nname);

                if (user == null) {
                    await ReplyAsync("Who?");
                } else {
                    ulong id = user.Id;

                    if (!(ids.Contains(id))) {
                        ids.Add(id);
                        nams.Add(user.Username + "#" + user.Discriminator);
                        dict.Add(id, 0);
                    }

                    dict[id] -= 1;
                    string s = (dict[id] == 1) ? "" : "s";
                    await Context.Channel.SendMessageAsync("*rubs out a tally*\n" + user.Username + " now has " + dict[id] + " strike" + s + " ovo");

                    SaveData();
                }
            }
        }


        [Command("set")]
        [Summary("Set's the number of strikes a user has")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task SetStrike([Summary("The number of strikes you want to set")]int strikes, [Summary("The user to strike")]SocketUser user) {
            ulong id = user.Id;
            if (!(ids.Contains(id))) {
                ids.Add(id);
                nams.Add(user.Username + "#" + user.Discriminator);
                dict.Add(id, 0);
            }

            bool moreStrikes = strikes > dict[id];
            bool sameStrikes = strikes == dict[id];
            bool oneDif = strikes - dict[id] == 1 || dict[id] - strikes == 1;
            bool negStrike = strikes < 0;
            bool negVal = dict[id] < 0;

            dict[id] = strikes;
            string s = (strikes == 1) ? "":"s";
            SaveData();

            string ss = "*";

            if (strikes != 0) {
                if (sameStrikes) {
                    ss += "looks at the tally*\n" + user.Username + " already has " + strikes + " strike" + s + "!";
                } else {
                    if (moreStrikes) {
                        if (oneDif) {
                            ss += "scratches in a tally*\n" + user.Username + " now has " + strikes + " strike" + s + "!";
                        } else {
                            ss += "scratches in a few tallies*\n" + user.Username + " now has " + strikes + " strike" + s + "!";
                        }
                    } else {
                        if (oneDif) {
                            ss += "rubs out a tally*\n" + user.Username + " now has " + strikes + " strike" + s + "!";
                        } else {
                            ss += "rubs out a few tallies*\n" + user.Username + " now has " + strikes + " strike" + s + "!";
                        }
                    }
                }
            } else {
                ss += "rubs out all of the tallies*\n" + user.Username + " now has " + strikes + " strike" + s + " x3";
            }

            await Context.Channel.SendMessageAsync(ss);
        }

        [Command("set")]
        [Summary("Set's the number of strikes a user has")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task SetStrike([Summary("The number of strikes you want to set")]int strikes, [Summary("The user to strike")]params string[] name) {
            if (name.Count() <= 0) {
                await ReplyAsync("Who?");
            } else {
                string nname = "";
                foreach (string nam in name) { 
                    nname += nam + " ";
                }
                nname = nname.Substring(0, nname.Length - 1);

                SocketGuildUser user = FindUser(Context.Guild, nname);

                if (user == null) {
                    await ReplyAsync("There could be many people named that, or there could be no one ouo.\nPlease say their name again with their 4 digit tag," +
                        " like `username#0000`. \nIf that was their name and number, I can't find them :p.\nIf the user is a tricky trickster and has #XXXX as a part of the username, make sure to put " +
                        "their real tag at the end!");
                } else {

                    ulong id = user.Id;
                    if (!(ids.Contains(id))) {
                        ids.Add(id);
                        nams.Add(user.Username + "#" + user.Discriminator);
                        dict.Add(id, 0);
                    }

                    bool moreStrikes = strikes > dict[id];
                    bool sameStrikes = strikes == dict[id];
                    bool oneDif = strikes - dict[id] == 1 || dict[id] - strikes == 1;
                    bool negStrike = strikes < 0;
                    bool negVal = dict[id] < 0;

                    dict[id] = strikes;
                    string s = (strikes == 1) ? "" : "s";
                    SaveData();

                    string ss = "*";

                    if (strikes != 0) {
                        if (sameStrikes) {
                            ss += "looks at the tally*\n" + user.Username + " already has " + strikes + " strike" + s + "!";
                        } else {
                            if (moreStrikes) {
                                if (oneDif) {
                                    ss += "scratches in a tally*\n" + user.Username + " now has " + strikes + " strike" + s + "!";
                                } else {
                                    ss += "scratches in a few tallies*\n" + user.Username + " now has " + strikes + " strike" + s + "!";
                                }
                            } else {
                                if (oneDif) {
                                    ss += "wipes out a tally*\n" + user.Username + " now has " + strikes + " strike" + s + "!";
                                } else {
                                    ss += "wipes out a few tallies*\n" + user.Username + " now has " + strikes + " strike" + s + "!";
                                }
                            }
                        }
                    } else {
                        ss += "wipes out all of the tallies*\n" + user.Username + " now has " + strikes + " strike" + s + " x3";
                    }

                    await Context.Channel.SendMessageAsync(ss);
                }
            }
        }

        [Command("get")]
        [Summary("Gets the number of strikes a user has")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task GetStrike([Summary("The user to get the strikes of.")]SocketGuildUser user) {
            if(!ids.Contains(user.Id)) {
                ids.Add(user.Id);
                nams.Add(user.Username + "#" + user.Discriminator);
                dict.Add(user.Id, 0);
            }

            int strikes = dict[user.Id];

            string s = (strikes == 1) ? "" : "s";

            await ReplyAsync("*looks at tally*\n" + user.Username + " has " + strikes + " strike" + s);
            SaveData();
        }

        [Command("get")]
        [Summary("Gets the number of strikes a user has")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task GetStrike([Summary("The user to get the strikes of.")]params string[] name) {
            if (name.Count() <= 0) {
                await ReplyAsync("Who?");
            } else {
                string nname = "";
                foreach (string nam in name) {
                    nname += nam + " ";
                }
                nname = nname.Substring(0, nname.Length - 1);

                SocketGuildUser user = FindUser(Context.Guild, nname);

                if (user == null) {
                    await ReplyAsync("There could be many people named that, or there could be no one ouo.\nPlease say their name again with their 4 digit tag," +
                        " like `username#0000`. \nIf that was their name and number, I can't find them :p.\nIf the user is a tricky trickster and has #XXXX as a part of the username, make sure to put " +
                        "their real tag at the end!");
                } else {
                    ulong id = user.Id;

                    if(!ids.Contains(id)) {
                        ids.Add(id);
                        nams.Add(user.Username + "#" + user.Discriminator);
                        dict.Add(id, 0);
                    }

                    int strikes = dict[user.Id];
                    string s = (strikes == 1) ? "" : "s";

                    await ReplyAsync("*looks at tally*\n" + user.Username + " has " + strikes + " strike" + s);

                }
            }
            SaveData();
        }
        
        [Command("list")]
        [Summary("Lists all users with strikes")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task ListStrike() {
            string s = "*picks up tally board*\nhere is the list:\n```";
            List<string> pages = new List<string>();
            string page = "";
            
            for(int i = 0; i < nams.Count(); i++) {
                string sp = " ";

                for(int spa = 0; spa < spacs-nams.ElementAt(i).Length; spa++) {
                    sp += " ";
                }

                page += (i + 1) + ". " + nams.ElementAt(i) + sp + "| " + dict[ids.ElementAt(i)] + "\n";

                if(((i + 1) % 20 == 0) || (i == nams.Count() - 1)) {
                    pages.Add(page);
                    page = "";
                }
            }

            int j = 0;

            foreach(string tmp_page in pages) {
                j++;
                s += "Page " + j + " of " + pages.Count() + ":\n" + tmp_page + "```";
                await ReplyAsync(s);
            }
        }        
             
        [Command("watch")]  
        [Summary("Adds someone to the list with 0 strikes.")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task StrikeWarn([Summary("The user to put on the list.")]SocketGuildUser user) {
            ulong id = user.Id;

            if (!ids.Contains(id)) {
                ids.Add(id);
                nams.Add(user.Username + "#" + user.Discriminator);
                dict.Add(id, 0);
            }

            await ReplyAsync($"I am now watching {user.Username} very closely *>\\_>*");

            SaveData();
        }

        [Command("watch")]
        [Summary("Adds someone to the list with 0 strikes.")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task StrikeWarn([Summary("The user to put on the list.")]params string[] name) {
            if (name.Count() <= 0) {
                await ReplyAsync("Who?");
            } else {
                string nname = "";
                foreach (string nam in name) {
                    nname += nam + " ";
                }
                nname = nname.Substring(0, nname.Length - 1);

                SocketGuildUser user = FindUser(Context.Guild, nname);

                ulong id = user.Id;

                if (!ids.Contains(id)) {
                    ids.Add(id);
                    nams.Add(user.Username + "#" + user.Discriminator);
                    dict.Add(id, 0);
                }

                await ReplyAsync($"I am now watching {user.Username} very closely *>\\_>*");

            }
            SaveData();
        }
        #endregion
    }
}
