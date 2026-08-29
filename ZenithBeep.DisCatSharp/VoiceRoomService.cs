using DisCatSharp;
using DisCatSharp.Entities;
using DisCatSharp.EventArgs;
using DisCatSharp.Enums;
using ZenithBeep.Data;
using Microsoft.Extensions.Logging;

namespace ZenithBeep.DisCatSharp;

public sealed class VoiceRoomService(BotRepository repository, ILogger<VoiceRoomService> logger)
{
    public async Task OnVoiceStateUpdatedAsync(DiscordClient client, VoiceStateUpdateEventArgs e)
    {
        if (e.Guild is null || e.User.IsBot) return;
        var after = e.After?.Channel; var before = e.Before?.Channel;
        if (after is not null)
        {
            if (await repository.GetLobbyAsync(e.Guild.Id) == after.Id)
            {
                var member = await e.Guild.GetMemberAsync(e.User.Id);
                var bot = await e.Guild.GetMemberAsync(client.CurrentUser.Id);
                var overwrites = new[]
                {
                    new DiscordOverwriteBuilder(member).Allow(Permissions.ManageChannels | Permissions.MoveMembers),
                    new DiscordOverwriteBuilder(bot).Allow(Permissions.ManageChannels | Permissions.MoveMembers),
                    new DiscordOverwriteBuilder(e.Guild.EveryoneRole).Allow(Permissions.UseVoice)
                };
                var created = await e.Guild.CreateVoiceChannelAsync($"{e.User.Username}'s Lair", after.Parent, userLimit: 0, overwrites: overwrites, reason: "ZenithBeep temporary room");
                await repository.AddTemporaryRoomAsync(e.Guild.Id, created.Id, e.User.Id);
                await created.PlaceMemberAsync(member);
                logger.LogInformation("Created temporary room {ChannelId} for {UserId} in guild {GuildId}", created.Id, e.User.Id, e.Guild.Id);
            }
        }
        if (before is not null) await DeleteIfEmptyAsync(e.Guild.Id, before);
    }

    public async Task DeleteIfEmptyAsync(ulong guildId, DiscordChannel channel)
    {
        if (!await repository.IsTemporaryRoomAsync(channel.Id) || channel.Users.Count != 0) return;
        await channel.DeleteAsync("ZenithBeep temporary room is empty");
        await repository.DeleteTemporaryRoomAsync(channel.Id);
        logger.LogInformation("Deleted empty temporary room {ChannelId}", channel.Id);
    }
}
