using DisCatSharp;
using DisCatSharp.ApplicationCommands;
using DisCatSharp.ApplicationCommands.Attributes;
using DisCatSharp.ApplicationCommands.Context;
using DisCatSharp.Entities;
using DisCatSharp.Enums;
using ZenithBeep.Data;

namespace ZenithBeep.DisCatSharp;

public sealed class RoleCommands(BotRepository repository) : ApplicationCommandsModule
{
    [SlashCommand("roleselector-create", "Create role selector")]
    [ApplicationCommandRequireUserPermissions(Permissions.ManageRoles)]
    public async Task CreateAsync(InteractionContext ctx, [Option("Roles", "Select role")] DiscordRole role, [Option("MesaageId", "Message id")] string messageId, [Option("Emoji", "Your emoji")] string emoji, [Option("Channel", "Select channel")] DiscordChannel? channel = null)
    {
        // Discord requires an initial interaction response in under three seconds.
        // Message lookup, SQLite and reaction creation can take longer than that.
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

        if (!ulong.TryParse(messageId, out var id)) { await EditResponseAsync(ctx, "Message id must be a number."); return; }
        channel ??= ctx.Channel;
        DiscordMessage message;
        try { message = await channel.GetMessageAsync(id); } catch { await EditResponseAsync(ctx, "Message not found."); return; }
        if (!EmojiParser.TryParse(ctx.Client, emoji, out var discordEmoji))
        {
            await EditResponseAsync(ctx, "Invalid emoji. Use Unicode (✅), :name:, or custom emoji (<:name:id>).");
            return;
        }
        var selector = await repository.AddRoleSelectorAsync(new RoleSelectorEntity { GuildId = ctx.Guild.Id, MessageId = id, ChannelId = channel.Id, RoleId = role.Id, Emoji = discordEmoji.ToString() });
        if (selector is null)
        {
            await EditResponseAsync(ctx, "This message already has a role selector for this emoji.");
            return;
        }
        await message.CreateReactionAsync(discordEmoji);
        await EditResponseAsync(ctx, $"Role selector created. Delete key: `{selector.Id}` — Role: {role.Mention} — Emoji: {discordEmoji}");
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
