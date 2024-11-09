

using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using ReworkZenithBeep.Data;


namespace ReworkZenithBeep.Module.RolesGet
{
    public class RoleSelectorsSlash : ApplicationCommandModule
    {
        private readonly RoleSelectors _roles;
        public RoleSelectorsSlash(DataBot contextDB)
        {
            _roles = new RoleSelectors(contextDB);
        }

        [SlashCommand("roleselector-create", "Create role selector")]
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
        public async Task CommandDeleteRoleSelect(InteractionContext context, [Option("KeyId", "Key role selector")] long keyRole)
        {
            var id = Convert.ToInt32(keyRole);
            await _roles.DeleteRolesCommand(context, id);
        }
    }
}
