using DisCatSharp;
using DisCatSharp.Entities;
using DisCatSharp.EventArgs;
using ZenithBeep.Data;
using Microsoft.Extensions.Logging;

namespace ZenithBeep.DisCatSharp;

public sealed class RoleSelectorService(BotRepository repository, ILogger<RoleSelectorService> logger)
{
    public async Task OnReactionAddedAsync(DiscordClient client, MessageReactionAddEventArgs e) => await ChangeRoleAsync(client, e.Guild, e.Channel, e.Message, e.User, e.Emoji, true);
    public async Task OnReactionRemovedAsync(DiscordClient client, MessageReactionRemoveEventArgs e) => await ChangeRoleAsync(client, e.Guild, e.Channel, e.Message, e.User, e.Emoji, false);
    private async Task ChangeRoleAsync(DiscordClient client, DiscordGuild? guild, DiscordChannel channel, DiscordMessage message, DiscordUser user, DiscordEmoji emoji, bool add)
    {
        if (guild is null || user.Id == client.CurrentUser.Id || user.IsBot) return;
        var selector = await repository.GetRoleSelectorAsync(guild.Id, message.Id, channel.Id, emoji.ToString());
        if (selector is null) return;
        var member = await guild.GetMemberAsync(user.Id); var role = guild.GetRole(selector.RoleId);
        if (role is null) { logger.LogWarning("Configured role {RoleId} no longer exists", selector.RoleId); return; }
        if (add) await member.GrantRoleAsync(role, "ZenithBeep role selector"); else await member.RevokeRoleAsync(role, "ZenithBeep role selector");
        logger.LogInformation("{Action} role {RoleId} for {UserId}", add ? "Granted" : "Revoked", role.Id, user.Id);
    }
}
