

using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.DependencyInjection;
using ReworkZenithBeep.Data;
using ReworkZenithBeep.Data.Models.items;
using ReworkZenithBeep.Extensions;
using ReworkZenithBeep.MessageEmbeds;
using ReworkZenithBeep.Services;
using ReworkZenithBeep.Settings;

namespace ReworkZenithBeep.Module.RolesGet
{
    public partial class RoleSelectors
    {
        public PaginationService Pagination;
        private static RoleSelectors instance;
        private readonly DataBot _dbContext;
        public RoleSelectors(DataBot dbContext, IServiceProvider service)
        {
            _dbContext = dbContext;
            Pagination = service.GetRequiredService<PaginationService>();
        }

        public static RoleSelectors GetInstance(DataBot dbContext, IServiceProvider service)
        {
            if (instance == null)
            {
                instance = new RoleSelectors(dbContext, service);
            }
            return instance;
        }

        private DiscordEmoji? _GetEmote(InteractionContext ctx, string emote)
        {
          
            if (DiscordEmoji.TryFromUnicode(emote, out var result))
            {
                return result;
            }

            IEmojiParser? emojiParser = new EmojiParser(emote);

            if (DiscordEmoji.TryFromGuildEmote(ctx.Client, emojiParser.GetId(), out var emoji1))
            {
                return emoji1;
            }
         
            return null;
        }

        public async Task CreateRolesCommand(InteractionContext ctx, DiscordChannel? channel, DiscordRole role, ulong msgId , string emoji)
        {
            await ctx.DeferAsync(true);
            DiscordMessage? _msg = await ctx.Channel.GetMessageAsync(msgId);
            DiscordChannel _channel = channel ?? ctx.Channel;
            if (channel != null)
            {
                _msg = await channel.GetMessageAsync(msgId);
            }

            if (_msg == null)
            {
                var embedError = EmbedTempalte.ErrorEmbed("Message by id was not found! >~<", "MassageNotFound");
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedError));
                return;
            }

            var _emoji = _GetEmote(ctx, emoji); 
            if (_emoji != null)
            {
                bool success = await _dbContext.CreateRolesSelector(ctx.Guild, _msg.Id, role.Id, _channel.Id, _emoji);
                if (success == false)
                {
                    var embedError = EmbedTempalte.ErrorEmbed("This object already exists! >~<", "DataObjectExists");
                    await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedError));
                    return;
                }


                await _msg.CreateReactionAsync(_emoji);
                var embedSuccess = new EmbedTempalte.DetailedEmbedContent {
                    Color = new DiscordColor("#72f963"),
                    Description = $"Add role on reaction {role.Mention}",
                    Title = "Success!"
                };
                var embed = EmbedTempalte.DetaliedEmbed(embedSuccess);
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(embed));
            }


        }

        public async Task DeleteRolesCommand(InteractionContext ctx, int keyid)
        {
            await ctx.DeferAsync(true);
            var dataRole = await _dbContext.GetKeyRoles(ctx.Guild, keyid);

            if (dataRole == null)
            {
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent("Not has been deleted!"));
                return;
            }

            var _msg = await ctx.Channel.GetMessageAsync(dataRole.messageId);

            if (_msg == null) {
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent("Not found message!"));
                return; 
            }


            var _emoji = _GetEmote(ctx, dataRole.emojiButton);
            if (_emoji != null)
            {
                bool success = await _dbContext.DeleteRoleAutoMod(ctx.Guild, keyid);
                if (success == false)
                {
                    await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent("Not has been deleted!"));
                    return;
                }

                await _msg.DeleteReactionsEmojiAsync(_emoji);
                var embedSuccess = new EmbedTempalte.DetailedEmbedContent
                {
                    Color = new DiscordColor("#72f963"),
                    Description = $"AutoMod role delete!",
                    Title = "Success!"
                };
                var embed = EmbedTempalte.DetaliedEmbed(embedSuccess);
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(embed));
            }
        }


        public async Task ListRolesCommand(InteractionContext ctx, ulong? messageid) {
            try
            {
                await ctx.DeferAsync(false);

                List<ItemRolesSelector> itemRoleslist = messageid != null
                    ? await _dbContext.GetListRoleSelector(ctx.Guild, messageid)
                    : await _dbContext.GetListRoleSelector(ctx.Guild);


                var rolesByMessageId = new Dictionary<ulong, string>();
                foreach (var itemRole in itemRoleslist)
                {
                    string roleInfo = $"**Delete key**: `{itemRole.keyId}` — **Role:** <@&{itemRole.roleId}> — **Emoji:** {itemRole.emojiButton}";

                    if (rolesByMessageId.TryGetValue(itemRole.messageId, out var existing))
                    {
                        rolesByMessageId[itemRole.messageId] = existing + "\n" + roleInfo;
                    }
                    else
                    {
                        rolesByMessageId[itemRole.messageId] = roleInfo;
                    }
                }

                var pagination = new PaginationMessage(
                    EmbedExtensions.PageFildRoleEmbed(rolesByMessageId, ctx.Guild.Id, ctx.Channel.Id),
                    title: "Role List",
                    embedColor: "#800080",
                    user: ctx.Member,
                    options: new AppearanceOptions
                    {
                        Timeout = TimeSpan.FromMinutes(5),
                        Style = DisplayStyle.Minimal,
                        OnStop = StopAction.DeleteMessage
                    });

                await Pagination.SendMessageInteractionAsync(ctx, pagination, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ListRolesCommand: {ex}");
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .WithContent("An error occurred while processing your request.")
                    .AsEphemeral(true));
            }
        }
    }
}
