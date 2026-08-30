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
        if (guild is null ||
            user.Id == client.CurrentUser.Id ||
            user.IsBot)
        {
            return;
        }

        var selector = await repository.GetRoleSelectorAsync(
            guild.Id,
            message.Id,
            channel.Id,
            emoji.ToString());

        if (selector is null)
            return;

        var member = await guild.GetMemberAsync(user.Id);
        var role = guild.GetRole(selector.RoleId);

        if (role is null)
        {
            logger.LogWarning(
                "Configured role {RoleId} no longer exists",
                selector.RoleId);

            return;
        }

        // Check requirement only when granting the role.
        if (add && selector.RequiredRoleId is not null)
        {
            var requiredRole = guild.GetRole(selector.RequiredRoleId.Value);

            if (requiredRole is null)
            {
                logger.LogWarning(
                    "Required role {RequiredRoleId} for selector {SelectorId} no longer exists",
                    selector.RequiredRoleId.Value,
                    selector.Id);

                return;
            }

            var hasRequiredRole = member.Roles.Any(
                x => x.Id == requiredRole.Id);

            if (!hasRequiredRole)
            {
                logger.LogInformation(
                    "User {UserId} cannot receive role {RoleId} because required role {RequiredRoleId} is missing",
                    user.Id,
                    role.Id,
                    requiredRole.Id);

                await SendDeniedReasonAsync(
                    user,
                    selector.DeniedReason,
                    requiredRole);

                return;
            }
        }

        if (add)
        {
            await member.GrantRoleAsync(
                role,
                "ZenithBeep role selector");
        }
        else
        {
            await member.RevokeRoleAsync(
                role,
                "ZenithBeep role selector");
        }

        logger.LogInformation(
            "{Action} role {RoleId} for {UserId}",
            add ? "Granted" : "Revoked",
            role.Id,
            user.Id);
    }

     private async Task SendDeniedReasonAsync(
        DiscordUser user,
        string? reason,
        DiscordRole requiredRole)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            reason =
                $"You need the {requiredRole.Name} role to receive this role.";
        }

        try
        {
            await user.SendMessageAsync(
                $"❌ {reason}");
        }
        catch (Exception ex)
        {
            logger.LogDebug(
                ex,
                "Could not send role selector denied message to user {UserId}",
                user.Id);
        }
    }
}
