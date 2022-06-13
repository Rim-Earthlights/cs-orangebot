using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using System.Collections.Generic;

/**
 *  メッセージ受信時に関する処理
 */
namespace OrangeBot {
    public class Messages : ModuleBase {

        /// <summary>
        /// デバッグ用
        /// </summary>
        /// <returns></returns>
        [Command("debug")]
        public async Task debug([Remainder]string text) {
            string message = "[debug] これは**デバッグ用メッセージ**です 引数:" + text;
            await ReplyAsync(message);
        }

        /// <summary>
        /// デバッグ用
        /// </summary>
        /// <returns></returns>
        [Command("debug")]
        public async Task debug() {
            string message = "[debug] これは**デバッグ用メッセージ**です 引数:";
            await ReplyAsync(message);
        }


        /// <summary>
        /// 天気を出力する
        /// </summary>
        /// <returns></returns>
        [Command("tenki")]
        public async Task tenki() {
            string message = "[debug] NotImplemented";
            await ReplyAsync(message);
        }
    }
}