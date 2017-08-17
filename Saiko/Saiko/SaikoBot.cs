using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Interactivity;
using DSharpPlus.CommandsNext;

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
            Client = new DiscordClient(new DiscordConfig()
            {
                AutomaticGuildSync = true,
                AutoReconnect = true,
                DiscordBranch = Branch.Stable,
                EnableCompression = true,
                LogLevel = LogLevel.Debug,
                Token = cfg.Token,
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true
            });

            Cnext = Client.UseCommandsNext(new CommandsNextConfiguration()
            {
                CaseSensitive = false,
                EnableDefaultHelp = true,
                EnableDms = false,
                EnableMentionPrefix = true,
                HelpEmbedColor = cfg.Color,
                SelfBot = false,
                StringPrefix = cfg.Prefix
            });

            Interactivity = Client.UseInteractivity();

            Cnext.RegisterCommands<Commands.Main>();
            Cnext.RegisterCommands<Commands.Data>();
            Cnext.RegisterCommands<Commands.Fun>();
            Cnext.RegisterCommands<Commands.Admin>();
            Cnext.RegisterCommands<Commands.Tools>();
            Cnext.RegisterCommands<Commands.Owner>();

            Client.SocketOpened += async () =>
            {
                await Task.Yield();
                SocketStart = DateTimeOffset.Now;
            };

            Client.Ready += async e =>
            {
                await e.Client.UpdateStatusAsync(new Game(_config.Status), UserStatus.Online);
            };
        }

        public async Task ConnectAsync()
        {
            BotStart = DateTimeOffset.Now;
            await Client.ConnectAsync();
        }
    }
}
