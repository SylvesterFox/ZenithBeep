namespace ZenithBeep.DisCatSharp;

public sealed class BotOptions
{
    public string TOKEN { get; set; } = string.Empty;
    public ulong COMMAND_GUILD_ID { get; set; }
    public string DATABASE_PATH { get; set; } = "data/zenithbeep.db";
    public string LAVALINK_HOST { get; set; } = "lavalink";
    public int LAVALINK_PORT { get; set; } = 2333;
    public string LAVALINK_PASSWORD { get; set; } = "youshallnotpass";
    public bool AUDIO_SERVICES { get; set; }
    public bool PROXY_ENABLED { get; set; }
    public string? HTTP_PROXY { get; set; }
    public string? HTTPS_PROXY { get; set; }
}
public static class EnvSettings
{
    public static T Load<T>() where T : new()
    {
        DotNetEnv.Env.TraversePath().Load();
        var config = new T();
        foreach (var property in typeof(T).GetProperties().Where(x => x.CanWrite))
        {
            var value = Environment.GetEnvironmentVariable(property.Name);
            if (string.IsNullOrWhiteSpace(value)) continue;
            property.SetValue(config, Convert.ChangeType(value, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType));
        }
        return config;
    }
}
