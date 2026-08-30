using DisCatSharp.ApplicationCommands;
using DisCatSharp.ApplicationCommands.Attributes;
using DisCatSharp.ApplicationCommands.Context;
using DisCatSharp.Entities;
using DisCatSharp.Enums;
using ZenithBeep.Data;
using Microsoft.Extensions.Logging;

namespace ZenithBeep.DisCatSharp;

public sealed class RoleCommands(BotRepository repository, ILogger<RoleCommands> logger) : ApplicationCommandsModule
{
    [SlashCommand("roleselector-create", "Create role selector")]
    [ApplicationCommandRequireUserPermissions(Permissions.ManageRoles)]
    public async Task CreateAsync(InteractionContext ctx, 
    [Option("Roles", "Select role")] DiscordRole role, 
    [Option("MesaageId", "Message id")] string messageId, 
    [Option("Emoji", "Your emoji")] string emoji, 
    [Option("Channel", "Select channel")] DiscordChannel? channel = null,
    [Option("RequiredRole", "User must have this role")] DiscordRole? requiredRole = null,
    [Option("DeniedReason", "Reason shown when user lacks required role")] string? deniedReason = null

    )
    {
         await ctx.CreateResponseAsync(
        InteractionResponseType.DeferredChannelMessageWithSource,
        new DiscordInteractionResponseBuilder().AsEphemeral());

    if (!ulong.TryParse(messageId, out var id))
    {
        await EditResponseAsync(
            ctx,
            "Message id must be a number.");

        return;
    }

    channel ??= ctx.Channel;

    DiscordMessage message;

    try
    {
        message = await channel.GetMessageAsync(id);
    }
    catch
    {
        await EditResponseAsync(
            ctx,
            "Message not found.");

        return;
    }

    if (!EmojiParser.TryParse(
            ctx.Client,
            emoji,
            out var discordEmoji))
    {
        await EditResponseAsync(
            ctx,
            "Invalid emoji. Use Unicode (✅), :name:, or custom emoji (<:name:id>).");

        return;
    }

    // If a reason was supplied without a required role,
    // the reason has no meaning.
    if (requiredRole is null &&
        !string.IsNullOrWhiteSpace(deniedReason))
    {
        await EditResponseAsync(
            ctx,
            "DeniedReason can only be used when RequiredRole is specified.");

        return;
    }

    var selector = await repository.AddRoleSelectorAsync(
        new RoleSelectorEntity
        {
            GuildId = ctx.Guild.Id,
            MessageId = id,
            ChannelId = channel.Id,
            RoleId = role.Id,
            Emoji = discordEmoji.ToString(),

            RequiredRoleId = requiredRole?.Id,
            DeniedReason = string.IsNullOrWhiteSpace(deniedReason)
                ? null
                : deniedReason.Trim()
        });

    if (selector is null)
    {
        await EditResponseAsync(
            ctx,
            "This message already has a role selector for this emoji.");

        return;
    }

    try
    {
        await message.CreateReactionAsync(discordEmoji);
    }
    catch (Exception ex)
    {
        logger.LogError(
            ex,
            "Failed to create reaction {Emoji} on message {MessageId}",
            discordEmoji,
            id);
    }

    var requirementText = requiredRole is null
        ? "No required role"
        : $"Required role: {requiredRole.Mention}";

    await EditResponseAsync(
        ctx,
        $"Role selector created.\n" +
        $"Delete key: `{selector.Id}`\n" +
        $"Role: {role.Mention}\n" +
        $"Emoji: {discordEmoji}\n" +
        $"{requirementText}");
    }

    [SlashCommand("roleselector-delrole", "Delete role selector")]
    [ApplicationCommandRequireUserPermissions(Permissions.ManageRoles)]
    public async Task DeleteAsync(InteractionContext ctx, [Option("KeyId", "Key role selector")] long key)
    {
        var found = await repository.DeleteRoleSelectorAsync(ctx.Guild.Id, (int)key);
        await UtilityCommands.Reply(ctx, found ? "Role selector deleted." : "Role selector not found.", !found);
    }

    [SlashCommand("roleselector-list", "Get list role selector")]
    [ApplicationCommandRequireUserPermissions(Permissions.ManageRoles)]
    public async Task ListAsync(InteractionContext ctx, [Option("messageId", "Message ID")] string? messageId = null)
    {
        var items = await repository.ListRoleSelectorsAsync(ctx.Guild.Id, ulong.TryParse(messageId, out var parsed) ? parsed : null);
        var text = items.Count == 0 ? "No role selectors." : string.Join('\n', items.Select(x => $"**Delete key**: `{x.Id}` — **Role:** <@&{x.RoleId}> — **Emoji:** {x.Emoji}"));
        await UtilityCommands.Reply(ctx, text, true);
    }

    private static Task EditResponseAsync(InteractionContext ctx, string content) =>
        ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent(content));
}
