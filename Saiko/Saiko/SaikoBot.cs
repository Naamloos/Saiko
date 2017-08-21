﻿using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Interactivity;
using DSharpPlus.CommandsNext;
using System.Net;

namespace Saiko
{
    public class SaikoBot
    {
        // Just in case we want to do something with these..
        public DiscordClient Client;
        public InteractivityModule Interactivity;
        public CommandsNextModule Cnext;
        SaikoConfig _config;
        public DiscordColor Color => _config.Color; // Can't make config public! >:c
        public DateTimeOffset BotStart;
        public DateTimeOffset SocketStart;

        public SaikoBot(SaikoConfig cfg)
        {
            _config = cfg;
            SaikoHelpFormatter.HelpColor = cfg.Color;
            Client = new DiscordClient(new DiscordConfig()
            {
                AutomaticGuildSync = true,
                AutoReconnect = true,
                DiscordBranch = Branch.Stable,
                //EnableCompression = true,
                LogLevel = LogLevel.Debug,
                Token = cfg.Token,
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true
            });

            var b = new DependencyCollectionBuilder();
            b.AddInstance<SaikoBot>(this);

            Cnext = Client.UseCommandsNext(new CommandsNextConfiguration()
            {
                CaseSensitive = false,
                EnableDefaultHelp = true,
                EnableDms = false,
                EnableMentionPrefix = true,
                SelfBot = false,
                StringPrefix = cfg.Prefix,
                Dependencies = b.Build(),
            });

            Cnext.SetHelpFormatter<SaikoHelpFormatter>();

            Interactivity = Client.UseInteractivity();

            Cnext.RegisterCommands<Commands.Main>();
            Cnext.RegisterCommands<Commands.Data>();
            Cnext.RegisterCommands<Commands.Fun>();
            Cnext.RegisterCommands<Commands.Admin>();
            Cnext.RegisterCommands<Commands.Tools>();
            Cnext.RegisterCommands<Commands.Owner>();
            Cnext.RegisterCommands<Commands.Weeb>();
            Cnext.RegisterCommands<Commands.Hentai>();
            Cnext.RegisterCommands<Commands.Text>();
            Cnext.RegisterCommands<Commands.RandomCommands>();

            Client.SocketOpened += async () =>
            {
                await Task.Yield();
                SocketStart = DateTimeOffset.Now;
            };

            Client.Ready += async e =>
            {
                await e.Client.UpdateStatusAsync(new Game(_config.Status), UserStatus.Online);
            };

            Client.ClientError += async e =>
            {
                Client.DebugLogger.LogMessage(LogLevel.Error, "Saiko-Bot", $"Type: {e.Exception.GetType().ToString()},\nException:\n{e.Exception.ToString()}", DateTime.Now);
            };
        }

        public async Task ConnectAsync()
        {
            BotStart = DateTimeOffset.Now;

            if (Type.GetType("Mono.Runtime") != null) // Checking if ran on mono
            {
                Client.SetWebSocketClient<WebSocketSharpClient>();
                // Overriding because adding certificates for mono is a pain
                ServicePointManager.ServerCertificateValidationCallback = (s, cert, chain, ssl) => true;
                Client.DebugLogger.LogMessage(LogLevel.Info, "Saiko-Bot", "Mono runtime detected, using WebSocketSharp and overriding Certificates", DateTime.Now);
            }
            else if (Environment.OSVersion.Version.Major <= 6 && Environment.OSVersion.Version.Minor < 2) // Win7 = 6.1, Win8 is 6.2. Min is Win8
            {
                Client.SetWebSocketClient<WebSocket4NetClient>();
                Client.DebugLogger.LogMessage(LogLevel.Info, "Saiko-Bot", "Windows 7 or below detected, using WebSocket4Net", DateTime.Now);
            }
            else
                Client.DebugLogger.LogMessage(LogLevel.Info, "Saiko-Bot", "Windows 8 or up detected, using .NET WebSocket", DateTime.Now);

            Client.DebugLogger.LogMessage(LogLevel.Info, "Saiko-Bot", $"Prefix: {_config.Prefix}, Color: {_config.Color.ToString()}", DateTime.Now);

            await Client.ConnectAsync();
        }
    }
}
