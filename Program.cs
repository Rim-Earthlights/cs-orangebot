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

namespace OrangeBot {
    class Program {

        private DiscordSocketClient client;
        public static CommandService commands;
        public static IServiceProvider services;

        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync() {
            client = new DiscordSocketClient();
            commands = new CommandService();
            services = new ServiceCollection().BuildServiceProvider();

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

            if (!(message.HasCharPrefix('.', ref argPos)
                || message.HasMentionPrefix(client.CurrentUser, ref argPos)
                )) { return; }

            var context = new CommandContext(client, message);

            var result = await commands.ExecuteAsync(context, argPos, services);

            if (!result.IsSuccess) { await context.Channel.SendMessageAsync(result.ErrorReason); }
        }

        private Task Log(LogMessage message) {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }
    }
}