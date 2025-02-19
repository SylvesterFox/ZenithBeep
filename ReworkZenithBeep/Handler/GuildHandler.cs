using DSharpPlus;
using DSharpPlus.EventArgs;
using ReworkZenithBeep.Settings;

namespace ReworkZenithBeep.Handler;

public class GuildHandler
{
    public static async Task OnGuildAvailble(DiscordClient sender, GuildCreateEventArgs e) {
        await Status.UpdateStatus(sender);
    }

    public static async Task OnGuildUavailble(DiscordClient sender, GuildDeleteEventArgs e) {
        await Status.UpdateStatus(sender);
    }

}
