
namespace ReworkZenithBeep.Settings
{
    public class BotConfig
    {
        public string TOKEN { get; set; } = string.Empty;
        public string LAVALINK_ADDRESS { get; set; } = "http://localhost:2333";
        public string LAVALINK_WEBSOCKET { get; set; } = "ws://localhost:2333/v4/websocket";
        public string LAVALINK_PASSWORD { get; set; } = "youshallnotpass";

        public string LOGS { get; set; } = "info";

        public bool AUDIO_SERVICES { get; set; } = false;
        public bool NODB_MODE { get; set; } = false;
    }
}
