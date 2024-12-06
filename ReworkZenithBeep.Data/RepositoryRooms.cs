
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using ReworkZenithBeep.Data.Models.items;

namespace ReworkZenithBeep.Data
{
    public class RepositoryRooms : DataBot
    {

        private readonly IDbContextFactory<BotContext> _contextFactory;
        public RepositoryRooms(IDbContextFactory<BotContext> contextFactory) : base(contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<bool> CreateLobbyData(DiscordChannel channel)
        {
            ItemGuild itemGuild = await GetOrCreateGuild(channel.Guild);
            using var context = _contextFactory.CreateDbContext();
            var query = context.RoomersLobbies.Where(x => x.Id == itemGuild.Id);
            if (await query.AnyAsync() == false)
            {
                var lobbyData = new ItemRoomersLobby()
                {
                    Id = itemGuild.Id,
                    LobbyId = channel.Id
                };
                context.Add(lobbyData);
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteLobbyData(DiscordGuild guild)
        {
            using var context = _contextFactory.CreateDbContext();
            var query = context.RoomersLobbies.Where(x => x.Id == guild.Id);
            if (await query.AnyAsync() == true)
            {
                context.RoomersLobbies.Remove(query.First());
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<ItemRoomersLobby?> GetLobbyDataGuild(DiscordGuild guild)
        {
            using var context = _contextFactory.CreateDbContext();
            var query = context.RoomersLobbies.Where(x => x.Id == guild.Id);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<ItemRoomersLobby?> GetLobbyDataChannel(ulong channelId)
        {
            using var context = _contextFactory.CreateDbContext();
            var query = context.RoomersLobbies.Where(x => x.LobbyId == channelId);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<ItemRooomsSettings> GetOrCreateSettingsRoom(DiscordUser user, string nameChannel)
        {
            ItemUser userData = await GetOrCreateUser(user);
            ItemRooomsSettings settings;
            using var context = _contextFactory.CreateDbContext();
            var query = context.ItemsRooms.Where(x => x.Id == userData.Id);
            if (!await query.AnyAsync())
            {
                settings = new ItemRooomsSettings()
                {
                    Id = userData.Id,
                    nameChannel = nameChannel,
                };
                context.Add(settings);
                await context.SaveChangesAsync();
                return settings;
            } else {
                settings = await query.FirstAsync();
            }

            return settings;
        }

        public async Task<bool> CreateTempRoom(DiscordUser user, DiscordChannel channel)
        {
            ItemUser userData =  await GetOrCreateUser(user);
            using var context = _contextFactory.CreateDbContext();
            var query = context.ItemsTempRooms.Where(x => x.roomid == channel.Id);
            if (!await query.AnyAsync())
            {
                var TempRoom = new ItemTempRoom()
                {
                    Id = userData.Id,
                    roomid = channel.Id,
                };
                context.Add(TempRoom);
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> TempRoomAny(DiscordChannel channel)
        {
            using var context = _contextFactory.CreateDbContext();
            var query = context.ItemsTempRooms.Where(x => x.roomid == channel.Id);
            return await query.AnyAsync();
        }


        public async Task<bool> TryDeleteTempRoom(DiscordChannel channel)
        {
            using var context = _contextFactory.CreateDbContext();
            var query = context.ItemsTempRooms.Where(x => x.roomid == channel.Id);
            if (!await query.AnyAsync())
            {
                return false;
            }

            context.ItemsTempRooms.Remove(query.First());
            await context.SaveChangesAsync();
            return true;
        }
    }
}
