using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Discord;
using Newtonsoft.Json;

namespace OrangeBot {
    class Program {

        private DiscordSocketClient client;
        public static CommandService commands;
        public static IServiceProvider services;

        private static Timer timer;
        private static Dictionary<string, bool> flags = new() {
            { "morning", false }
        };

        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync() {
            client = new DiscordSocketClient();
            commands = new CommandService();
            services = new ServiceCollection().BuildServiceProvider();

            TimerCallback tc = async state => {
                if (DateTime.Now.Hour == 23 && DateTime.Now.Minute == 59) {
                    foreach(var f in flags) {
                        flags[f.Key] = false;
                    }
                    return;
                }

                
                if (!flags["morning"]) {
                    if (DateTime.Now.Hour == 6 && DateTime.Now.Minute == 30) {
                        flags["morning"] = true;
                        await morning();
                    }
                }
            };

            timer = new Timer(tc, null, 500, 1000);

            client.Log += Log;
            client.MessageReceived += CommandReceived;

            var config = new Configure(Global.iniPath);
            WebWrapper.cc = new System.Net.CookieContainer();

            string token = config.Get("SECRET_KEY");
            Global.forecastKey = config.Get("FORECAST_KEY");

            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task CommandReceived(SocketMessage messageParam) {
            var message = messageParam as SocketUserMessage;

            if (message == null) {
                return;
            }
            // 発言者がbot, 自身に反応することはしない
            if (message.Author.IsBot) {
                return;
            }

            int argPos = 0;

            // prefix command
            if (message.HasCharPrefix('.', ref argPos)) {
                var context = new CommandContext(client, message);

                var result = await commands.ExecuteAsync(context, argPos, services);

                if (!result.IsSuccess) { await context.Channel.SendMessageAsync(result.ErrorReason); }
            }

            // mention command
            if (message.HasMentionPrefix(client.CurrentUser, ref argPos)) {
                if (message.Content.Contains(client.CurrentUser.Mention.Replace("!", ""))) {
                    if (message.Content.Contains("debug")) {
                        string d = "";
                        foreach (var c in message.Content.Split(new char[] { ' ', '　' })) {
                            d += c + Environment.NewLine;
                        }

                        var embed = new EmbedBuilder();
                        embed.WithTitle("デバッグ出力");
                        embed.WithDescription(d);

                        await message.ReplyAsync(
                            String.Format("[Debug] テスト用だよ～"),
                            embed: embed.Build()
                            );
                    }
                    if (message.Content.Contains("言語は")) {
                        await message.ReplyAsync(
                            String.Format("今動いてる言語は[{0}]だよ～！{1}コードはここで見れるよ！{1}{2}", "C#", Environment.NewLine, @"https://gitlab.com/Rim_EarthLights/cs-orangebot")
                            );
                    }
                    return;
                }
            }
        }

        /// <summary>
        /// 朝の定時処理
        /// </summary>
        /// <param name="arg">場所</param>
        /// <returns></returns>
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
            // 日付を日本基準の表記へ
            var culture = new System.Globalization.CultureInfo("ja-JP");

            string message = "";
            message += String.Format("おはよう！\n");
            message += String.Format("{0}の天気だよ\n\n", DateTime.Now.ToString("D", culture));

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
            // TODO: IDの指定方法
            await client.GetGuild(974941026038976543).GetTextChannel(974941026038976545).SendMessageAsync(message);
        }

        private Task Log(LogMessage message) {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }
    }
}