using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;

namespace SilphyBot.Modules
{
    [Group("tally")]
    public class TallyModule {
        public static Dictionary<string, int> tallies = new Dictionary<string, int>();


    }
}
