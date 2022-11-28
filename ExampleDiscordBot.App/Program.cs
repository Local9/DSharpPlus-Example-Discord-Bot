using DSharpPlus;
using DSharpPlus.Entities;
using ExampleDiscordBot.App.Config;
using ExampleDiscordBot.App.Entities;
using ExampleDiscordBot.App.TimedScripts;
using Microsoft.Extensions.Logging;

namespace ExampleDiscordBot.App
{
    class Program
    {
        public readonly EventId BotEventId = new EventId(42, "Example-Discord-Bot");
        public static ulong BOT_MESSAGE_CHANNEL { get; private set; }
        public static ulong BOT_ERROR_MESSAGE_CHANNEL { get; private set; }
        public static ulong BOT_GUILD_ID { get; private set; }

        public static DiscordClient? Client { get; private set; }
        public static Configuration Configuration { get; private set; }

        static GameServerStatus? _gameServerStatus;

        static void Main(string[] args)
        {
            Program program = new();
            program.MainAsync().GetAwaiter().GetResult();
        }

        async Task MainAsync()
        {
            Configuration = await ApplicationConfig.GetConfig();

            DiscordConfiguration discordConfiguration = new()
            {
                Token = Configuration.Token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged,
                MinimumLogLevel = LogLevel.Debug
            };
            
            if (Configuration.Channels.ContainsKey("error"))
                BOT_ERROR_MESSAGE_CHANNEL = Configuration.Channels["error"];

            if (Configuration.Channels.ContainsKey("message"))
                BOT_MESSAGE_CHANNEL = Configuration.Channels["message"];
            
            BOT_GUILD_ID = Configuration.Guild;

            Client = new DiscordClient(discordConfiguration);

            Client.Ready += Client_Ready;
            Client.GuildAvailable += Client_GuildAvailable;
            Client.ClientErrored += Client_ClientErrored;

            await Client.ConnectAsync();

            _gameServerStatus = new(Client);

            await SendLogMessage($"Discord Bot has started");

            await Task.Delay(-1);
        }

        public async static Task SendErrorMessage(string message)
        {
            await SendMessage(BOT_ERROR_MESSAGE_CHANNEL, message);
        }

        public async static Task SendLogMessage(string message)
        {
            await SendMessage(BOT_MESSAGE_CHANNEL, message);
        }

        public async static Task SendMessage(ulong channelId, string message)
        {
            if (Client is null)
                return;

            DiscordGuild discordGuild = await Client.GetGuildAsync(BOT_GUILD_ID);

            DiscordChannel discordChannel = await Client.GetChannelAsync(channelId);
            await discordChannel.SendMessageAsync(message);
        }

        private Task Client_Ready(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs e)
        {
            sender.Logger.LogInformation(BotEventId, "Perseverance is ready to process events.");
            return Task.CompletedTask;
        }

        private Task Client_GuildAvailable(DiscordClient sender, DSharpPlus.EventArgs.GuildCreateEventArgs e)
        {
            sender.Logger.LogInformation(BotEventId, $"Guild available: {e.Guild.Name}");
            return Task.CompletedTask;
        }

        private Task Client_ClientErrored(DiscordClient sender, DSharpPlus.EventArgs.ClientErrorEventArgs e)
        {
            sender.Logger.LogError(BotEventId, e.Exception, "Exception occured");
            return Task.CompletedTask;
        }
    }
}