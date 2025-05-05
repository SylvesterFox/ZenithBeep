
using DSharpPlus.EventArgs;
using DSharpPlus;
using ReworkZenithBeep.Data;
using Microsoft.Extensions.DependencyInjection;
using DSharpPlus.Entities;

namespace ReworkZenithBeep.Handler
{
    public class VoiceRoomsHandler
    {
        private RepositoryRooms _repositoryRooms;
        public VoiceRoomsHandler(IServiceProvider serviceProvider)
        {
            _repositoryRooms = serviceProvider.GetRequiredService<RepositoryRooms>();
        }

        public async Task OnRoomStateUpdated(DiscordClient sender, VoiceStateUpdateEventArgs e)
        {

            var joinedChannel = e.After?.Channel;
            var leftChannel = e.Before?.Channel;

            var user = e.User;
            var member = (DiscordMember)user;

           if (joinedChannel != null)
           {
                var isLobby = await _repositoryRooms.GetLobbyDataChannel(joinedChannel.Id);
                if (isLobby != null)
                {
                    // Создание приватной комнаты
                    var dataSettings = await _repositoryRooms.GetOrCreateSettingsRoom(user, $"{user.Username}'s Lair");
                    var bot = await e.Guild.GetMemberAsync(sender.CurrentUser.Id);
                    var category = joinedChannel.Parent;

                    var overwrites = new DiscordOverwriteBuilder[]
                    {
                        new DiscordOverwriteBuilder(member).Allow(Permissions.ManageChannels),
                        new DiscordOverwriteBuilder(bot).Allow(Permissions.ManageChannels),
                        new DiscordOverwriteBuilder(e.Guild.EveryoneRole).Allow(Permissions.UseVoice)
                    };

                    var newChannel = await e.Guild.CreateVoiceChannelAsync(
                        dataSettings.nameChannel,
                        user_limit: dataSettings.limitChannel,
                        overwrites: overwrites
                    );

                    if (category != null)
                        await newChannel.ModifyAsync(c => c.Parent = category);

                    var success = await _repositoryRooms.CreateTempRoom(user, newChannel);
                    if (success)
                        await newChannel.PlaceMemberAsync(member);
                    else
                        Console.WriteLine("Failed to register temp room");
                }
            }

            if (leftChannel != null)
            {
                // Удаляем только если канал был временным
                Console.WriteLine($"{user.Username} left or switched from {leftChannel.Name}");
                await DeleteRoom(leftChannel);
            }

            await Task.CompletedTask;
        }

        public async Task DeleteRoom(DiscordChannel channel)
        {
            bool deleteIs = false;
            var temproom = await _repositoryRooms.TempRoomAny(channel);
            if (channel.Users.Count == 0 && temproom)
            {
                deleteIs = await _repositoryRooms.TryDeleteTempRoom(channel);
            }

            if (deleteIs)
            {
                await channel.DeleteAsync();
            }
        }
    }
}
