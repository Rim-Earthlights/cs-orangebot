using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangeBot {
    internal class Global {
        internal const string iniPath = "set.ini";
        internal static string forecastKey;
        internal static Word2Vec.Net.Word2Vec vec;
        internal static ulong BotUid;
    }
}
