using CSharpOsu;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SaiCore.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using SaiCore.Helpers;

namespace SaiCore
{
    public class Saiko
    {
        internal DiscordClient _client;
        internal InteractivityExtension _interactivity;
        internal CommandsNextExtension _cnext;
        internal Config _config;
        internal CancellationTokenSource _cts;
        internal OsuClient _osu;
        internal DateTimeOffset BotStart;
        internal DateTimeOffset SocketStart;

        public Saiko()
        {
            if (!File.Exists("config.json"))
            {
                File.Create("config.json").Close();
                File.WriteAllText("config.json", JsonConvert.SerializeObject(new Config()));
                Console.WriteLine("Please fill in config.json");
                Console.ReadKey();
                Environment.Exit(0);
                return;
            }

            this._config = Config.Load("config.json");

            this._client = new DiscordClient(new DiscordConfiguration()
            {
                Token = _config.Token,
                GatewayCompressionLevel = GatewayCompressionLevel.Stream,
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug
            });

            this._interactivity = _client.UseInteractivity(new InteractivityConfiguration()
            {
                PaginationBehavior = TimeoutBehaviour.DeleteReactions,
                PaginationTimeout = TimeSpan.FromSeconds(60),
                Timeout = TimeSpan.FromSeconds(60)
            });

            var deps = new ServiceCollection()
                .AddSingleton(this)
                .BuildServiceProvider();

            this._cnext = _client.UseCommandsNext(new CommandsNextConfiguration()
            {
                CaseSensitive = false,
                EnableDefaultHelp = true,
                EnableDms = false,
                EnableMentionPrefix = true,
                IgnoreExtraArguments = true,
                Selfbot = false,
                StringPrefixes = new List<string>() { _config.Prefix },
                Services = deps
            });

			this._cnext.SetHelpFormatter<HelpFormatter>();

			this._cnext.RegisterCommands(Assembly.GetExecutingAssembly());

            this._cts = new CancellationTokenSource();

            this.BotStart = DateTimeOffset.Now;

            _client.SocketOpened += async () =>
            {
                await Task.Yield();
                this.SocketStart = DateTimeOffset.Now;
            };

            _client.Ready += async e =>
            {
                await _client.UpdateStatusAsync(new DiscordActivity("anime :3", ActivityType.Watching), UserStatus.Online);
            };

            _cnext.CommandErrored += async e =>
            {
                _client.DebugLogger.LogMessage(LogLevel.Critical, "OOF", $"{e.Exception.ToString()}", DateTime.Now);
            };

            _osu = new OsuClient(_config.OsuToken);
        }

        public async Task RunAsync()
        {
            await this._client.ConnectAsync();
            await WaitForCancellation();
            await this._client.DisconnectAsync();
        }

        private async Task WaitForCancellation()
        {
            while (!_cts.IsCancellationRequested)
            {
                await Task.Delay(500);
            }
        }
    }
}
