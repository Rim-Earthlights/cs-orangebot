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
using Discord;

/**
 *  メッセージ受信時に関する処理
 */
namespace OrangeBot {
    public class Messages : ModuleBase<CommandContext> {

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
            string description = "";

            var user = Context.User;
            var test = Context.Message.MentionedUserIds;

            if (test.Count == 0){
                description += "test" + Environment.NewLine;
            }

            var embed = new EmbedBuilder();
            embed.WithTitle("デバッグ出力");
            embed.WithDescription(description);

            string message = String.Format("[debug] **これはデバッグ用メッセージです**");
            await ReplyAsync(message, embed: embed.Build());
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
                result.Add(rnd.Next(diceMax) + 1);
            }

            var roll = result.Select(x => x.ToString()).Aggregate((a, b) => a + ", " + b);

            var embed = new EmbedBuilder();
            embed.WithTitle("出目");
            embed.WithDescription(roll);

            await ReplyAsync(
                String.Format("[Dice] {0}面ダイスを{1}回振りました。合計は{2}です。", diceMax, diceNum, result.Sum()),
                embed: embed.Build()
                );
        }

        [Command("luck")]
        public async Task luck() {
            var rnd = new Random().Next(100);
            string message = "";
            string luck = "";

            if (rnd < 20) {
                luck = "大吉" + Environment.NewLine;
                luck += String.Format("おめでとういいことあるよ！！！");
            }
            else if (rnd < 45) {
                luck = "中吉" + Environment.NewLine;
                luck += String.Format("それなりにいいことありそう。");
            }
            else if (rnd < 75) {
                luck = "小吉" + Environment.NewLine;
                luck += String.Format("ふつうがいちばんだよね。");
            }
            else if (rnd < 90) {
                luck = "末吉" + Environment.NewLine;
                luck += String.Format("まあこういうときもあるよね。");
            }
            else if (rnd < 95) {
                luck = "凶" + Environment.NewLine;
                luck += String.Format("気をつけようね。");
            }
            else {
                luck = "大凶" + Environment.NewLine;
                luck += String.Format("逆にレアだしポジティブに考えてこ");
            }

            var embed = new EmbedBuilder();
            embed.WithTitle("運勢");
            embed.WithDescription(luck);

            await ReplyAsync(message, embed: embed.Build());

        }

        [Command("gacha")]
        public async Task gacha(string gn) {
            var num = int.Parse(gn);
            var rnd = new Random();

            var result = new List<int>();
            for (int i = 0; i < num; i++) {
                result.Add(rnd.Next(10000) + 1);
            }
            await ReplyAsync(String.Format(""));
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
            message += "```\n";
            foreach (var w in wheatherList) {
                var deg = "";
                if (w.Wind.Deg < 23 || w.Wind.Deg > 337) {
                    deg = "北";
                } else if (w.Wind.Deg < 68) {
                    deg = "北東";
                } else if (w.Wind.Deg < 113) {
                    deg = "東";
                } else if (w.Wind.Deg < 158) {
                    deg = "南東";
                } else if (w.Wind.Deg < 203) {
                    deg = "南";
                } else if (w.Wind.Deg < 248) {
                    deg = "南西";
                } else if (w.Wind.Deg < 293) {
                    deg = "西";
                }　else if (w.Wind.Deg <= 337) {
                    deg = "北西";
                }

                message += String.Format("[{0}] {1}({2})\n", w.Name, w.Weather[0].Main, w.Weather[0].Description);
                message += String.Format(" * 気温: {0:F1}℃ ({1:F1}℃ / {2:F1}℃)\n", w.Main.Temp, w.Main.TempMin, w.Main.TempMax);
                message += String.Format(" * 風速: {0} / {1:F1}m\n", deg, w.Wind.Speed);
            }
            message += "```\n";
            await ReplyAsync(message);
        }

        /// <summary>
        /// 天気を出力する(地名指定)
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

            url = String.Format("https://api.openweathermap.org/data/2.5/weather?lat={0}&lon={1}&appid={2}&lang=ja&units=metric", lat, lon, Global.forecastKey);
            text = await WebWrapper.GetAsync(url);
            var w = JsonConvert.DeserializeObject<WeatherJson>(text);

            var deg = "";
            if (w.Wind.Deg < 23 || w.Wind.Deg > 337) {
                deg = "北";
            }
            else if (w.Wind.Deg < 68) {
                deg = "北東";
            }
            else if (w.Wind.Deg < 113) {
                deg = "東";
            }
            else if (w.Wind.Deg < 158) {
                deg = "南東";
            }
            else if (w.Wind.Deg < 203) {
                deg = "南";
            }
            else if (w.Wind.Deg < 248) {
                deg = "南西";
            }
            else if (w.Wind.Deg < 293) {
                deg = "西";
            }
            else if (w.Wind.Deg <= 337) {
                deg = "北西";
            }

            string message = "";
            message += "```\n";
            message += String.Format("[{0}] {1}({2})\n", args[0], w.Weather[0].Main, w.Weather[0].Description);
            message += String.Format(" * 気温: {0:F1}℃ ({1:F1}℃ / {2:F1}℃)\n", w.Main.Temp, w.Main.TempMin, w.Main.TempMax);
            message += String.Format(" * 風速: {0} / {1:F1}m\n", deg, w.Wind.Speed);
            message += "```\n";

            await ReplyAsync(message);
        }

        /// <summary>
        /// 朝の定時処理
        /// </summary>
        /// <param name="arg">場所</param>
        /// <returns></returns>
        [Command("morning")]
        public async Task morning() {
            var request = new List<WeatherRequest>() {
                new WeatherRequest("北海道", "43.04", "141.21"),
                new WeatherRequest("東京都", "35.69", "139.70"),
                new WeatherRequest("神奈川", "35.26", "139.38"),
                new WeatherRequest("和歌山", "35.68", "139.76")
            };

            var wheatherList = new List<WeatherJson>();
            foreach (var w in request) {
                var url = String.Format("https://api.openweathermap.org/data/2.5/weather?lat={0}&lon={1}&appid={2}&lang=ja&units=metric", w.Lat, w.Lon, Global.forecastKey);
                var text = await WebWrapper.GetAsync(url);
                var json = JsonConvert.DeserializeObject<WeatherJson>(text);
                json.Name = w.Name;
                wheatherList.Add(json);
            }

            string message = "";
            message += String.Format("おはよう！\n");
            message += String.Format("{0}の天気だよ\n\n", DateTime.Now.ToString("D"));
            message += "```\n";
            foreach (var w in wheatherList) {
                var deg = "";
                if (w.Wind.Deg < 23 || w.Wind.Deg > 337) {
                    deg = "北";
                }
                else if (w.Wind.Deg < 68) {
                    deg = "北東";
                }
                else if (w.Wind.Deg < 113) {
                    deg = "東";
                }
                else if (w.Wind.Deg < 158) {
                    deg = "南東";
                }
                else if (w.Wind.Deg < 203) {
                    deg = "南";
                }
                else if (w.Wind.Deg < 248) {
                    deg = "南西";
                }
                else if (w.Wind.Deg < 293) {
                    deg = "西";
                }
                else if (w.Wind.Deg <= 337) {
                    deg = "北西";
                }

                message += String.Format("[{0}] {1}({2})\n", w.Name, w.Weather[0].Main, w.Weather[0].Description);
                message += String.Format(" * 気温: {0:F1}℃ ({1:F1}℃ / {2:F1}℃)\n", w.Main.Temp, w.Main.TempMin, w.Main.TempMax);
                message += String.Format(" * 風速: {0} / {1:F1}m\n", deg, w.Wind.Speed);
            }
            message += "```\n";
            // TODO: IDの指定方法
            await ReplyAsync(message);
        }
    }
}