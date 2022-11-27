using DSharpPlus;
using DSharpPlus.Entities;
using ExampleDiscordBot.App.Config;
using ExampleDiscordBot.App.Entities;
using ExampleDiscordBot.App.TimedScripts;

namespace ExampleDiscordBot.App
{
    class Program
    {
        public static ulong BOT_MESSAGE_CHANNEL { get; private set; }
        public static ulong BOT_ERROR_MESSAGE_CHANNEL { get; private set; }
        public static ulong BOT_GUILD_ID { get; private set; }

        public static DiscordClient Client { get; private set; }
        public static Configuration Configuration { get; private set; }
        static GameServerStatus _gameServerStatus;

        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            Configuration = await ApplicationConfig.GetConfig();

            DiscordConfiguration discordConfiguration = new()
            {
                Token = "My First Token",
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged
            };
            
            if (Configuration.Channels.ContainsKey("error"))
                BOT_ERROR_MESSAGE_CHANNEL = Configuration.Channels["error"];

            if (Configuration.Channels.ContainsKey("error"))
                BOT_MESSAGE_CHANNEL = Configuration.Channels["message"];
            
            BOT_GUILD_ID = Configuration.Guild;

            Client = new DiscordClient(discordConfiguration);
            await Client.ConnectAsync();

            _gameServerStatus = new(Client);

            SendLogMessage($"Discord Bot has started");

            await Task.Delay(-1);
        }

        public async static void SendErrorMessage(string message)
        {
            DiscordChannel discordChannel = await Client.GetChannelAsync(BOT_ERROR_MESSAGE_CHANNEL);
            discordChannel.SendMessageAsync(message);
        }

        public async static void SendLogMessage(string message)
        {
            DiscordChannel discordChannel = await Client.GetChannelAsync(BOT_MESSAGE_CHANNEL);
            discordChannel.SendMessageAsync(message);
        }

        public async static void SendMessage(ulong channelId, string message)
        {
            DiscordChannel discordChannel = await Client.GetChannelAsync(channelId);
            discordChannel.SendMessageAsync(message);
        }
    }
}