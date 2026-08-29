using DisCatSharp;
using DisCatSharp.ApplicationCommands;
using DisCatSharp.ApplicationCommands.Attributes;
using DisCatSharp.ApplicationCommands.Context;
using DisCatSharp.Enums;
using ZenithBeep.Data;

namespace ZenithBeep.DisCatSharp;

public sealed class RoomCommands(BotRepository repository) : ApplicationCommandsModule
{
    [SlashCommand("lobby-create", "Create a lobby for private voice channels")]
    [ApplicationCommandRequireUserPermissions(Permissions.ManageChannels)]
    public async Task CreateLobbyAsync(InteractionContext ctx)
    {
        if (await repository.GetLobbyAsync(ctx.Guild.Id) is not null) { await UtilityCommands.Reply(ctx, "Lobby already exists! >~<`", true); return; }
        var channel = await ctx.Guild.CreateVoiceChannelAsync("[+] Create voice channel [+]", reason: "ZenithBeep lobby");
        await repository.SetLobbyAsync(ctx.Guild.Id, channel.Id);
        await UtilityCommands.Reply(ctx, "Create lobby!");
    }

    [SlashCommand("lobby-delete", "Delete a lobby for private voice channels")]
    [ApplicationCommandRequireUserPermissions(Permissions.ManageChannels)]
    public async Task DeleteLobbyAsync(InteractionContext ctx)
    {
        var lobby = await repository.GetLobbyAsync(ctx.Guild.Id);
        if (lobby is not ulong id) { await UtilityCommands.Reply(ctx, "Lobby channel does not exist. >~<`", true); return; }
        await (await ctx.Client.GetChannelAsync(id)).DeleteAsync("ZenithBeep lobby removed");
        await repository.DeleteLobbyAsync(ctx.Guild.Id);
        await UtilityCommands.Reply(ctx, "The lobby has been removed!");
    }

    [SlashCommand("vclock", "Lock and unlock voice channel")]
    public async Task LockAsync(InteractionContext ctx)
    {
        var channel = ctx.Member?.VoiceState?.Channel;
        if (channel is null || !await repository.IsTemporaryRoomAsync(channel.Id)) { await UtilityCommands.Reply(ctx, "I couldn't close the voice channel >~<`", true); return; }
        var overwrite = channel.PermissionOverwrites.FirstOrDefault(x => x.Id == ctx.Guild.EveryoneRole.Id);
        var isOpen = overwrite?.Allowed.HasPermission(Permissions.UseVoice) ?? true;
        await channel.AddOverwriteAsync(ctx.Guild.EveryoneRole, isOpen ? Permissions.None : Permissions.UseVoice, isOpen ? Permissions.UseVoice : Permissions.None, "ZenithBeep room lock");
        await UtilityCommands.Reply(ctx, isOpen ? "The channel was closed!" : "The channel was opened!");
    }
}
