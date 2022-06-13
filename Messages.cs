using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

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

        [Command("dice")]
        public async Task dice([Remainder]string d) {
            var di = d.Split(' ').Select(x => int.Parse(x)).ToArray();
            if (di.Length < 2) {
                await ReplyAsync("[ERROR] ダイスの指定が不正");
                return;
            }

            var diceNum = di[0];
            var diceMax = di[1];

            var rnd = new Random();
            var result = new List<int>();
            for (int i = 0; i < diceNum; i++) {
                result.Add(rnd.Next(diceMax));
            }

            var message = result.Select(x => x.ToString()).Aggregate((a, b) => a + ", " + b);
            await ReplyAsync(String.Format("[Dice] {0}", message));
        }


        /// <summary>
        /// 天気を出力する
        /// </summary>
        /// <param name="arg">場所</param>
        /// <returns></returns>
        [Command("tenki")]
        public async Task tenki() {
            var request = new List<WeatherRequest>() {
                new WeatherRequest("北海道", "43.04", "141.21"),
                new WeatherRequest("東京都", "35.69", "139.70"),
                new WeatherRequest("神奈川", "35.26", "139.38"),
                new WeatherRequest("和歌山", "35.68", "139.76")
            };

            var wheatherList = new List<WeatherJson>();
            foreach(var w in request) {
                var url = String.Format("https://api.openweathermap.org/data/2.5/weather?lat={0}&lon={1}&appid={2}&lang=ja&units=metric", w.Lat, w.Lon, Global.forecastKey);
                var text = await WebWrapper.GetAsync(url);
                var json = JsonConvert.DeserializeObject<WeatherJson>(text);
                json.Name = w.Name;
                wheatherList.Add(json);
            }

            string message = "";
            foreach(var w in wheatherList) {
                message += String.Format("[{0}] {1}({2})\n", w.Name, w.Weather[0].Main, w.Weather[0].Description);
                message += String.Format(" * 気温: {0:F1}℃ ({1:F1}℃ / {2:F1}℃)\n", w.Main.Temp, w.Main.TempMin, w.Main.TempMax);
            }
            await ReplyAsync(message);
        }

        /// <summary>
        /// 天気を出力する
        /// </summary>
        /// <param name="arg">場所</param>
        /// <returns></returns>
        [Command("tenki")]
        public async Task tenki([Remainder] string arg) {
            var args = arg.Split(' ');

            var url = String.Format("http://api.openweathermap.org/geo/1.0/direct?q={0}&limit=5&appid={1}", args[0], Global.forecastKey);

            var text = await WebWrapper.GetAsync(url);

            var json = JArray.Parse(text);

            var j = json[0];
            var lat = j["lat"].ToString();
            var lon = j["lon"].ToString();

            var wheatherList = new List<WeatherJson>();
            url = String.Format("https://api.openweathermap.org/data/2.5/weather?lat={0}&lon={1}&appid={2}&lang=ja", lat, lon, Global.forecastKey);
            text = await WebWrapper.GetAsync(url);
            wheatherList.Add(JsonConvert.DeserializeObject<WeatherJson>(text));

            string message = "";
            foreach (var w in wheatherList) {
                message += String.Format("[{0}] {1}({2})\n", args[0], w.Weather[0].Main, w.Weather[0].Description);
            }
            await ReplyAsync(message);
        }
    }
}