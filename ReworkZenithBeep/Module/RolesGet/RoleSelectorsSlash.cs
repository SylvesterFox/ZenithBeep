

using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using ReworkZenithBeep.Data;


namespace ReworkZenithBeep.Module.RolesGet
{
    public class RoleSelectorsSlash : ApplicationCommandModule
    {
        private readonly RoleSelectors _roles;
        public RoleSelectorsSlash(DataBot contextDB, IServiceProvider service)
        {
            _roles = new RoleSelectors(contextDB, service);
        }

        [SlashCommand("roleselector-create", "Create role selector")]
        [SlashCommandPermissions(DSharpPlus.Permissions.ManageRoles)]
        public async Task CommandCreateRoleSelect(
            InteractionContext context,
            [Option("Roles", "Select role")] DiscordRole discordRole, 
            [Option("MesaageId", "Message id")] string messageId, 
            [Option("Emoji", "Your emoji")] string emoji,
            [Option("Channel", "Select channel")] DiscordChannel? channel = null)
        {
            var id = Convert.ToUInt64(messageId);
            await _roles.CreateRolesCommand(context, channel, discordRole, id, emoji);
        }

        [SlashCommand("roleselector-delrole", "Delete role selector")]
        [SlashCommandPermissions(DSharpPlus.Permissions.ManageRoles)]
        public async Task CommandDeleteRoleSelect(InteractionContext context, [Option("KeyId", "Key role selector")] long keyRole)
        {
            var id = Convert.ToInt32(keyRole);
            await _roles.DeleteRolesCommand(context, id);
        }

        [SlashCommand("roleselector-list", "Get list role selector")]
        [SlashCommandPermissions(DSharpPlus.Permissions.ManageRoles)]
        public async Task ListRoleSelect(InteractionContext ctx, [Option("messageId", "Message ID")] string? messageId = null) {
            if (!string.IsNullOrEmpty(messageId)) {
                var id = Convert.ToUInt64(messageId);
                await _roles.ListRolesCommand(ctx, id);
            } else {
                await _roles.ListRolesCommand(ctx, null);
            }
           
        }
    }
}
