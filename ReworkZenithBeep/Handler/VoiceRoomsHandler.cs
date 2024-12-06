
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
            // Check if the user joined a voice channel
            if (e.Before?.Channel == null && e.After?.Channel != null)
            {
                Console.WriteLine($"{e.User.Username} joined the voice channel {e.After.Channel.Name}");
                Console.WriteLine($"Category: {e.Channel.IsCategory}");
                var getLobby = await _repositoryRooms.GetLobbyDataChannel(e.Channel.Id);
                if (getLobby != null)
                {
                    var dataSettings = await _repositoryRooms.GetOrCreateSettingsRoom(e.User, $"{e.User.Username}'s Lair");
                    var member = (DiscordMember) e.After.User;
                    var overWriteBuilderUser = new DiscordOverwriteBuilder[]
                    {
                        new DiscordOverwriteBuilder(member).Allow(Permissions.ManageChannels)
                    };
                    var channel = await e.Guild.CreateVoiceChannelAsync(dataSettings.nameChannel, user_limit: dataSettings.limitChannel, overwrites:overWriteBuilderUser);
                    var dataTemp = await _repositoryRooms.CreateTempRoom(e.User, channel);
                    if (!dataTemp)
                    {
                        Console.WriteLine("Not working");
                        return;
                    }

                    await channel.PlaceMemberAsync(member);
                }
            }
            // Check if the user left a voice channel
            else if (e.Before?.Channel != null && e.After?.Channel == null)
            {
                Console.WriteLine($"{e.User.Username} left the voice channel {e.Before.Channel.Name}");
                await DeleteRoom(e.Before.Channel);
            }
            // Check if the user switched voice channels
            else if (e.Before?.Channel != null && e.After?.Channel != null && e.Before.Channel != e.After.Channel)
            {
                Console.WriteLine($"{e.User.Username} switched from {e.Before.Channel.Name} to {e.After.Channel.Name}");
                await DeleteRoom(e.Before.Channel);
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
