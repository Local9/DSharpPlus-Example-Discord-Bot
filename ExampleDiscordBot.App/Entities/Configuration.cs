using Newtonsoft.Json;

namespace ExampleDiscordBot.App.Entities
{
    struct Configuration
    {
        [JsonProperty("token")]
        public string Token { get; private set; }

        [JsonProperty("guild")]
        public ulong Guild { get; private set; }

        [JsonProperty("channels")]
        public Dictionary<string, ulong> Channels { get; private set; }

        [JsonProperty("servers")]
        public List<Server> Servers { get; private set; }
    }

    struct Server
    {
        [JsonProperty("label")]
        public string Label { get; private set; }

        [JsonProperty("ip")]
        public string IP { get; private set; }

        [JsonProperty("port")]
        public int Port { get; private set; }

        [JsonProperty("connect")]
        public string Connect { get; private set; }

        public override string ToString() => $"{IP}:{Port}";
    }
}
