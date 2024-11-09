
using DSharpPlus.EventArgs;
using DSharpPlus;
using ReworkZenithBeep.Data;
using Microsoft.Extensions.DependencyInjection;

namespace ReworkZenithBeep.Handler
{
    public class RoleSelectorsHandler
    {
        public DataBot dataBot;
        public  RoleSelectorsHandler(IServiceProvider service)
        {
            dataBot = service.GetRequiredService<DataBot>();
        }

        public async Task MessageReactionAdd(DiscordClient sender, MessageReactionAddEventArgs args)
        {
            var dataRole = await dataBot.GetRoleAutoMod(args.Guild, args.Message.Id, args.Emoji);
            if (dataRole == null) {
                return;
            }

            var member = await args.Guild.GetMemberAsync(args.User.Id);
            var role = args.Guild.GetRole(dataRole.roleId);
            await member.GrantRoleAsync(role);
            
            Console.WriteLine($"test add from Role: {dataRole.roleId}");
            await Task.CompletedTask;
        }

        public async Task MessageReactionRemove(DiscordClient sender, MessageReactionRemoveEventArgs args)
        {
            var dataRole = await dataBot.GetRoleAutoMod(args.Guild, args.Message.Id, args.Emoji);
            if (dataRole == null)
            {
                return;
            }

            var member = await args.Guild.GetMemberAsync(args.User.Id);
            var role = args.Guild.GetRole(dataRole.roleId);
            await member.RevokeRoleAsync(role);

            Console.WriteLine($"test remove from Message: {dataRole.roleId}");
            await Task.CompletedTask;
        }
    }
}
