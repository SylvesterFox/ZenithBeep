using DisCatSharp;
using DisCatSharp.ApplicationCommands;
using DisCatSharp.ApplicationCommands.Attributes;
using DisCatSharp.ApplicationCommands.Context;
using DisCatSharp.Entities;
using DisCatSharp.Enums;

namespace ZenithBeep.DisCatSharp;

public sealed class UtilityCommands : ApplicationCommandsModule
{
    [SlashCommand("beep", "ping command!")]
    public Task BeepAsync(InteractionContext ctx) => Reply(ctx, "Beep!");

    [SlashCommand("help", "Get list commands")]
    public Task HelpAsync(InteractionContext ctx) => Reply(ctx, "`/lobby-create`, `/lobby-delete`, `/vclock`, `/roleselector-create`, `/roleselector-delrole`, `/roleselector-list`, `/join`, `/leave`, `/play`, `/skip`, `/queue`, `/remove`, `/looptrack`, `/pause`, `/clear`, `/seek`, `/move`");

    [SlashCommand("bugreport", "Bug report command")]
    public Task BugReportAsync(InteractionContext ctx) => Reply(ctx, "You can describe all problems and bugs on the bot GitHub page: https://github.com/SylvesterFox/ZenithBeep/issues", true);

    [SlashCommand("messages-clean", "Delete messages from channel")]
    [ApplicationCommandRequireUserPermissions(Permissions.Administrator)]
    public async Task CleanAsync(InteractionContext ctx, [Option("count", "Count of messages to delete")] long count)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
        var messages = await ctx.Channel.GetMessagesAsync((int)Math.Clamp(count, 1, 100));
        await ctx.Channel.DeleteMessagesAsync(messages);
        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"You can delete {messages.Count} messages"));
    }

    internal static Task Reply(InteractionContext ctx, string content, bool ephemeral = false)
    {
        var response = new DiscordInteractionResponseBuilder().WithContent(content);
        if (ephemeral) response.AsEphemeral();
        return ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, response);
    }
}
