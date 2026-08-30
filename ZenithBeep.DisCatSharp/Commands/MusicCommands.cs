using DisCatSharp.ApplicationCommands;
using DisCatSharp.ApplicationCommands.Attributes;
using DisCatSharp.ApplicationCommands.Context;
using DisCatSharp.Lavalink;
using DisCatSharp.Lavalink.Entities;
using DisCatSharp.Lavalink.Enums;

namespace ZenithBeep.DisCatSharp;

/// <summary>Music commands backed by DisCatSharp.Lavalink's built-in per-guild queue.</summary>
public sealed class MusicCommands(BotOptions options) : ApplicationCommandsModule
{
    private bool Enabled => options.AUDIO_SERVICES;

    private static Task ReplyAsync(InteractionContext ctx, string message, bool ephemeral = false) => UtilityCommands.Reply(ctx, message, ephemeral);
    private Task DisabledAsync(InteractionContext ctx) => ReplyAsync(ctx, "Music services are disabled in `.env` (`AUDIO_SERVICES=false`).", true);
    private LavalinkGuildPlayer? GetPlayer(InteractionContext ctx) => ctx.Client.GetLavalink().GetGuildPlayer(ctx.Guild);

    [SlashCommand("join", "Connect music bot to voice-channel")]
    public async Task JoinAsync(InteractionContext ctx)
    {
        if (!Enabled) { await DisabledAsync(ctx); return; }
        var voice = ctx.Member?.VoiceState?.Channel;
        if (voice is null) { await ReplyAsync(ctx, "You are not connected to a voice channel", true); return; }
        var session = ctx.Client.GetLavalink().ConnectedSessions.Values.FirstOrDefault();
        if (session is null) { await ReplyAsync(ctx, "The Lavalink connection is not established", true); return; }
        await session.ConnectAsync(voice);
        await ReplyAsync(ctx, $"Connected to {voice.Mention}");
    }

    [SlashCommand("leave", "Leave from voice channel")]
    public async Task LeaveAsync(InteractionContext ctx)
    {
        if (!Enabled) { await DisabledAsync(ctx); return; }
        var player = GetPlayer(ctx);
        if (player is null) { await ReplyAsync(ctx, "Lavalink not connected.", true); return; }
        await player.DisconnectAsync();
        await ReplyAsync(ctx, "Leave from voice channel. Bye!");
    }

    [SlashCommand("play", "Statring playing")]
    public async Task PlayAsync(InteractionContext ctx, [Option("url", "Name track")] string query)
    {
        if (!Enabled) { await DisabledAsync(ctx); return; }
        var player = GetPlayer(ctx);
        if (player is null) { await ReplyAsync(ctx, "Connect the bot first with `/join`.", true); return; }
        var result = await player.LoadTracksAsync(LavalinkSearchType.Youtube, query);
        if (result.LoadType is LavalinkLoadResultType.Empty or LavalinkLoadResultType.Error) { await ReplyAsync(ctx, $"Nothing was found for {query}.", true); return; }
        if (result.LoadType == LavalinkLoadResultType.Track) player.AddToQueue(result.GetResultAs<LavalinkTrack>());
        else player.AddToQueue(result.GetResultAs<LavalinkPlaylist>());
        player.PlayQueue();
        await ReplyAsync(ctx, "Track added to queue.");
    }

    [SlashCommand("skip", "Skiping track")]
    public async Task SkipAsync(InteractionContext ctx, [Option("count", "Counts tracks skip")] long count = 1)
    {
        if (!Enabled) { await DisabledAsync(ctx); return; }
        var player = GetPlayer(ctx);
        if (player?.CurrentTrack is null) { await ReplyAsync(ctx, "Queue empty!", true); return; }
        for (var i = 0; i < Math.Clamp(count, 1, 25); i++) await player.SkipAsync();
        await ReplyAsync(ctx, "Track skipped.");
    }

    [SlashCommand("queue", "Queue list")]
    public async Task QueueAsync(InteractionContext ctx)
    {
        if (!Enabled) { await DisabledAsync(ctx); return; }
        var player = GetPlayer(ctx);
        if (player is null) { await ReplyAsync(ctx, "Lavalink not connected.", true); return; }
        var tracks = player.Queue.Take(15).Select((track, index) => $"`{index + 1}.` {track.Info.Title}").ToList();
        await ReplyAsync(ctx, tracks.Count > 0 ? string.Join('\n', tracks) : "The queue is empty");
    }

    [SlashCommand("remove", "Remove track from queue")]
    public async Task RemoveAsync(InteractionContext ctx, [Option("Nunber", "Removing number track from queue.")] long position)
    {
        if (!Enabled) { await DisabledAsync(ctx); return; }
        var player = GetPlayer(ctx);
        if (player is null || position < 1 || position > player.Queue.Count) { await ReplyAsync(ctx, $"Unable to delete track `{position}`. Wrong number.", true); return; }
        player.RemoveFromQueueAt((int)position - 1);
        await ReplyAsync(ctx, $"`{position}.` remove from queue");
    }

    [SlashCommand("looptrack", "Looping track")]
    public async Task LoopAsync(InteractionContext ctx)
    {
        if (!Enabled) { await DisabledAsync(ctx); return; }
        var player = GetPlayer(ctx);
        if (player?.CurrentTrack is null) { await ReplyAsync(ctx, "Queue empty!", true); return; }
        player.SetRepeatMode(RepeatMode.Current);
        await ReplyAsync(ctx, "Track looped!");
    }

    [SlashCommand("pause", "Pause and resume track.")]
    public async Task PauseAsync(InteractionContext ctx)
    {
        if (!Enabled) { await DisabledAsync(ctx); return; }
        var player = GetPlayer(ctx);
        if (player?.CurrentTrack is null) { await ReplyAsync(ctx, "Queue empty!", true); return; }
        await player.PauseAsync();
        await ReplyAsync(ctx, "Suspended");
    }

    [SlashCommand("clear", "Clear all queue music bot.")]
    public async Task ClearAsync(InteractionContext ctx)
    {
        if (!Enabled) { await DisabledAsync(ctx); return; }
        var player = GetPlayer(ctx);
        if (player is null) { await ReplyAsync(ctx, "Lavalink not connected.", true); return; }
        player.ClearQueue();
        await ReplyAsync(ctx, "Clear queue!");
    }

    [SlashCommand("seek", "Seek track")]
    public async Task SeekAsync(InteractionContext ctx, [Option("Timecode", "Seek track, mm:ss or hh:mm:ss")] string timeCode)
    {
        if (!Enabled) { await DisabledAsync(ctx); return; }
        var player = GetPlayer(ctx);
        if (player?.CurrentTrack is null || !TryParseTimeCode(timeCode, out var position)) { await ReplyAsync(ctx, "Invalid format. Use `/seek hh:mm:ss` or `/seek mm:ss`", true); return; }
        await player.SeekAsync(position);
        await ReplyAsync(ctx, $"Rewind to {position:hh\\:mm\\:ss}");
    }

    [SlashCommand("move", "Move tracks to the top of the queue")]
    public async Task MoveAsync(InteractionContext ctx, [Option("Position", "Move track to the top of the queue")] long position)
    {
        if (!Enabled) { await DisabledAsync(ctx); return; }
        var player = GetPlayer(ctx);
        if (player is null || position < 1 || position > player.Queue.Count) { await ReplyAsync(ctx, "Invalid track number.", true); return; }
        var track = player.Queue[(int)position - 1];
        player.RemoveFromQueueAt((int)position - 1);
        player.AddToQueueAt(0, track);
        await ReplyAsync(ctx, $"Moved `{track.Info.Title}` to the top of the queue.");
    }

    private static bool TryParseTimeCode(string value, out TimeSpan result) => TimeSpan.TryParse(value, out result) && result >= TimeSpan.Zero;
}
