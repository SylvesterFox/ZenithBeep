using DSharpPlus;
using DSharpPlus.Entities;

namespace ReworkZenithBeep.Settings;

public static class Status
{

    public static async Task UpdateStatus(DiscordClient client) {
        var guildCount = client.Guilds.Count;
        var activity = new DiscordActivity($" /help | Used on {guildCount} servers", ActivityType.Playing);
        await client.UpdateStatusAsync(activity);
    }

}
