﻿

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using ReworkZenithBeep.Data;
using ReworkZenithBeep.Data.Models.items;
using ReworkZenithBeep.MessageEmbeds;

namespace ReworkZenithBeep.Module.Rooms
{
    public partial class RoomsSelectors
    {
        private static RoomsSelectors instance;
        private readonly RepositoryRooms _dbContext;

        public RoomsSelectors(RepositoryRooms dbContext)
        {
            _dbContext = dbContext;
        }

        public static RoomsSelectors GetInstance(RepositoryRooms dbContext)
        {
            if (instance == null)
            {
                instance = new RoomsSelectors(dbContext);
            }
            return instance;
        }

        public async Task CreateLobbyCommand(InteractionContext ctx)
        {
            await ctx.DeferAsync(true);

            var lobbydata = await _dbContext.GetLobbyDataGuild(ctx.Guild);
            if (lobbydata != null) {
                var embedErrorCreate = EmbedTempalte.ErrorEmbed("Lobby already exists! >~<`", "LobbyNotCreate");
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedErrorCreate));
                return;
            }

            var channel = await ctx.Guild.CreateVoiceChannelAsync("[+] Create voice channel [+]");
            if (channel != null)
            {
                 
                bool dataCreated = await _dbContext.CreateLobbyData(channel);

                if (!dataCreated) {
                    var embedErrorCreate = EmbedTempalte.ErrorEmbed("Lobby not create! >~<`", "LobbyNotCreate");
                    await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedErrorCreate));
                    return;
                }

                var embedSuccess = new EmbedTempalte.DetailedEmbedContent
                {
                    Color = new DiscordColor("#72f963"),
                    Description = $"Create lobby!",
                    Title = "Success!"
                };
                var embed = EmbedTempalte.DetaliedEmbed(embedSuccess);
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(embed));
                return;
            }

            var embedError = EmbedTempalte.ErrorEmbed("This Lobby already exists! >~<\n if you want to delete it, you can do it with this command `/del-lobby`", "LobbyAlreadyExists");
            await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedError));
        }

        public async Task DeleteLobbyCommand(InteractionContext ctx)
        {
            await ctx.DeferAsync(true);
            ItemRoomersLobby? data = await _dbContext.GetLobbyDataGuild(ctx.Guild);
            if (data == null)
            {
                var embedErrorDel = EmbedTempalte.ErrorEmbed("Lobby channel does not exist. >~<`", "LobbyNotDelete");
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedErrorDel));
                return;
            }

            var channel = await ctx.Client.GetChannelAsync(data.LobbyId);
            var dataDelete = await _dbContext.DeleteLobbyData(ctx.Guild);
            await channel.DeleteAsync();
            if (!dataDelete)
            {
                DiscordEmbed embedErrorDel = EmbedTempalte.ErrorEmbed("For some reason the lobby cannot be deleted, or it no longer exists. >~<`", "LobbyNotDelete");
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedErrorDel));
                return;
            }


            var embedSuccess = new EmbedTempalte.DetailedEmbedContent
            {
                Color = new DiscordColor("#72f963"),
                Description = $"The lobby has been removed!",
                Title = "Success!"
            };
            var embed = EmbedTempalte.DetaliedEmbed(embedSuccess);
            await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(embed));
        }

        public async Task LockPrivateVoice(InteractionContext ctx)
        {
            await ctx.DeferAsync(true);
            if (ctx.Member.VoiceState != null)
            {
                var voice = ctx.Member.VoiceState;
                var privateVoice = await _dbContext.TempRoomAny(voice.Channel);
                if (!privateVoice)
                {
                    DiscordEmbed embedErrorDel = EmbedTempalte.ErrorEmbed("I couldn't close the voice channel >~<`", "NotLockRoom");
                    await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedErrorDel));
                    return;
                }

                var overwrite = voice.Channel.PermissionOverwrites.FirstOrDefault(o => o.Id == voice.Guild.EveryoneRole.Id);

                bool isAllowed = overwrite?.Allowed.HasPermission(Permissions.UseVoice) ?? false;
                bool isDenied = overwrite?.Denied.HasPermission(Permissions.UseVoice) ?? false;


                if (isAllowed && !isDenied) {
                    await voice.Channel.AddOverwriteAsync(voice.Guild.EveryoneRole, deny: Permissions.UseVoice);
                    var embedSuccess = new EmbedTempalte.DetailedEmbedContent
                    {
                        Color = new DiscordColor("#72f963"),
                        Description = $"The channel was closed!",
                        Title = "Success!"
                    };
                    var embed = EmbedTempalte.DetaliedEmbed(embedSuccess);
                    await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(embed));

                } else
                {
                    await voice.Channel.AddOverwriteAsync(voice.Guild.EveryoneRole, allow: Permissions.UseVoice);
                    var embedSuccess = new EmbedTempalte.DetailedEmbedContent
                    {
                        Color = new DiscordColor("#72f963"),
                        Description = $"The channel was opened!",
                        Title = "Success!"
                    };
                    var embed = EmbedTempalte.DetaliedEmbed(embedSuccess);
                    await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(embed));
                }
                

            }
        }
        
    }
}
