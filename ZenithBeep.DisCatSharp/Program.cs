using DisCatSharp;
using DisCatSharp.ApplicationCommands;
using DisCatSharp.EventArgs;
using DisCatSharp.Enums;
using DisCatSharp.Lavalink;
using DisCatSharp.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ZenithBeep.Data;
using ZenithBeep.DisCatSharp;

var options = EnvSettings.Load<BotOptions>();
if (string.IsNullOrWhiteSpace(options.TOKEN)) throw new InvalidOperationException("TOKEN is required in .env.");
var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton(options);
builder.Services.AddDbContextFactory<BotDbContext>(x => x.UseSqlite($"Data Source={Path.GetFullPath(options.DATABASE_PATH)}"));
builder.Services.AddSingleton<BotRepository>();
builder.Services.AddSingleton<VoiceRoomService>();
builder.Services.AddSingleton<RoleSelectorService>();
builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(x => { x.TimestampFormat = "yyyy-MM-dd HH:mm:ss "; x.SingleLine = true; });

var app = builder.Build();
await using (var scope = app.Services.CreateAsyncScope()) { var db = scope.ServiceProvider.GetRequiredService<IDbContextFactory<BotDbContext>>(); await (await db.CreateDbContextAsync()).Database.EnsureCreatedAsync(); }

SetProxy("HTTP_PROXY", options.PROXY_ENABLED ? options.HTTP_PROXY : null);
SetProxy("HTTPS_PROXY", options.PROXY_ENABLED ? options.HTTPS_PROXY : null);
var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Discord");
var client = new DiscordClient(new DiscordConfiguration
{
    Token = options.TOKEN,
    TokenType = TokenType.Bot,
    Intents = DiscordIntents.Guilds | DiscordIntents.GuildVoiceStates | DiscordIntents.GuildMessageReactions | DiscordIntents.GuildMembers,
    MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Information
});
var voiceRooms = app.Services.GetRequiredService<VoiceRoomService>();
var roles = app.Services.GetRequiredService<RoleSelectorService>();
client.VoiceStateUpdated += voiceRooms.OnVoiceStateUpdatedAsync;
client.MessageReactionAdded += roles.OnReactionAddedAsync;
client.MessageReactionRemoved += roles.OnReactionRemovedAsync;
client.ClientErrored += (_, e) => { logger.LogError(e.Exception, "Discord client error"); return Task.CompletedTask; };

var commands = client.UseApplicationCommands(new ApplicationCommandsConfiguration { ServiceProvider = app.Services });
RegisterCommands(commands, options.COMMAND_GUILD_ID);
if (options.AUDIO_SERVICES)
{
    var lavalink = client.UseLavalink();
    client.Ready += async (_, _) =>
    {
        try { await lavalink.ConnectAsync(new LavalinkConfiguration { Password = options.LAVALINK_PASSWORD, RestEndpoint = new ConnectionEndpoint { Hostname = options.LAVALINK_HOST, Port = options.LAVALINK_PORT }, SocketEndpoint = new ConnectionEndpoint { Hostname = options.LAVALINK_HOST, Port = options.LAVALINK_PORT }, EnableBuiltInQueueSystem = true }); logger.LogInformation("Lavalink connected to {Host}:{Port}", options.LAVALINK_HOST, options.LAVALINK_PORT); }
        catch (Exception ex) { logger.LogError(ex, "Lavalink connection failed; music commands remain unavailable"); }
    };
    logger.LogInformation("Music is enabled");
}
else logger.LogInformation("Music is disabled by configuration");

await client.ConnectAsync();
logger.LogInformation("ZenithBeep started");
await Task.Delay(Timeout.Infinite);

static void SetProxy(string variable, string? value) => Environment.SetEnvironmentVariable(variable, string.IsNullOrWhiteSpace(value) ? null : value);
static void RegisterCommands(ApplicationCommandsExtension extension, ulong guildId)
{
    if (guildId == 0) { extension.RegisterGlobalCommands<UtilityCommands>(); extension.RegisterGlobalCommands<RoomCommands>(); extension.RegisterGlobalCommands<RoleCommands>(); extension.RegisterGlobalCommands<MusicCommands>(); }
    else { extension.RegisterGuildCommands<UtilityCommands>(guildId); extension.RegisterGuildCommands<RoomCommands>(guildId); extension.RegisterGuildCommands<RoleCommands>(guildId); extension.RegisterGuildCommands<MusicCommands>(guildId); }
}
