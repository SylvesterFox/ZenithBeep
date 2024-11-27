
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using ReworkZenithBeep.Data.Models.items;

namespace ReworkZenithBeep.Data
{
    public class DataBot
    {
        private readonly IDbContextFactory<BotContext> _contextFactory;
        public DataBot(IDbContextFactory<BotContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        /// <summary>
        /// Creates and receives a guild from the base
        /// </summary>
        /// <param name="guild"></param>
        /// <returns></returns>
        public async Task<ItemGuild> GetOrCreateGuild(DiscordGuild guild)
        {
            ItemGuild itemGuild;
            using var context = _contextFactory.CreateDbContext();
            var query = context.Guilds.Where(x => x.Id == guild.Id);

            if (await query.AnyAsync() == false) 
            {
                itemGuild = new ItemGuild()
                {
                    Id = guild.Id,
                    Name = guild.Name,
                };
                context.Add(itemGuild);
                await context.SaveChangesAsync();
            } else itemGuild = await query.FirstAsync();

            return itemGuild;
        }

        /// <summary>
        /// Prefix update request
        /// </summary>
        /// <param name="guild"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public async Task UpdatePrefix(DiscordGuild guild, string prefix)
        {
            using var context = _contextFactory.CreateDbContext();
            var guildContext = await context.Guilds.Where(x => x.Id == guild.Id).FirstOrDefaultAsync();
            
            
            if (guildContext != null)
            {
                guildContext.Prefix = prefix;
            } else
            {
                context.Add(new ItemGuild {  Id = guild.Id, Name = guild.Name, Prefix = prefix });
            }

            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Add role
        /// </summary>
        /// <param name="guild"></param>
        /// <param name="messageId"></param>
        /// <param name="roleId"></param>
        /// <param name="channelId"></param>
        /// <param name="Emoji"></param>
        /// <returns></returns>
        public async Task<bool> CreateRolesSelector(DiscordGuild guild, ulong messageId, ulong roleId, ulong channelId, string Emoji)
        {
            using var context = _contextFactory.CreateDbContext();
            ItemGuild contextGuild = await GetOrCreateGuild(guild);
            ItemRolesSelector itemRolesSelector;
            var query = context.Roles.Where(x => x.roleId == roleId)
                .Where(x => x.Id == contextGuild.Id)
                .Where(x => x.messageId == messageId);
                

            if (await query.AnyAsync() == false)
            {
                itemRolesSelector = new ItemRolesSelector
                {
                    Id = contextGuild.Id,
                    messageId = messageId,
                    channelId = channelId,
                    roleId = roleId,
                    emojiButton = Emoji,
                };
                context.Add(itemRolesSelector);
                await context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// Get role from database
        /// </summary>
        /// <param name="guild">DiscordGuild entities</param>
        /// <param name="messageId">ulong id</param>
        /// <param name="emoji">string emoji</param>
        /// <returns></returns>
        public async Task<ItemRolesSelector?> GetRoleAutoMod(DiscordGuild guild, ulong messageId, string emoji)
        {
            using var context = _contextFactory.CreateDbContext();
            ItemGuild contextGuild = await GetOrCreateGuild(guild);
            var query = context.Roles.Where(x => x.messageId == messageId)
                .Where(x => x.Id == contextGuild.Id)
                .Where (x => x.emojiButton == emoji);

            if (await query.AnyAsync() == false)
            {
                return null;
            }

            return await query.FirstOrDefaultAsync();
        }


        /// <summary>
        /// Delete role from database
        /// </summary>
        /// <param name="guild">DiscordGuild entities</param>
        /// <param name="idkey">Key int id</param>
        /// <returns></returns>
        public async Task<bool> DeleteRoleAutoMod(DiscordGuild guild, int idkey)
        {
            using var context = _contextFactory.CreateDbContext();
            ItemGuild contextGuild = await GetOrCreateGuild(guild);
            var query = context.Roles.Where(x => x.Id == contextGuild.Id)
                .Where(x => x.keyId == idkey);

            if (await query.AnyAsync() == false) { return false; }
            var data = await query.FirstOrDefaultAsync();
            if (data != null)
            {
                context.Roles.Remove(data);
                await context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        /// <summary>
        ///  Get roles from key
        /// </summary>
        /// <param name="guild"></param>
        /// <param name="idkey"></param>
        /// <returns></returns>
        public async Task<ItemRolesSelector?> GetKeyRoles(DiscordGuild guild, int idkey)
        {
            using var context = _contextFactory.CreateDbContext();
            ItemGuild contextGuild = await GetOrCreateGuild(guild);
            var query = context.Roles.Where(x => x.Id == contextGuild.Id)
                .Where(x => x.keyId == idkey);

            if (await query.AnyAsync() == false) return null;

            return await query.FirstOrDefaultAsync();
        }

    }
}
