
using DSharpPlus.SlashCommands;
using ReworkZenithBeep.Data;

namespace ReworkZenithBeep.Module.Rooms
{
    public class RoomsSelectorsSlash : ApplicationCommandModule
    {
        private readonly RoomsSelectors _roomsSelectors;
        public RoomsSelectorsSlash(RepositoryRooms repositoryRooms)
        {
            _roomsSelectors = new RoomsSelectors(repositoryRooms);
        }

        [SlashCommand("lobby-create", "Create a lobby for private voice channels")]
        [SlashCommandPermissions(DSharpPlus.Permissions.ManageChannels)]
        public async Task CommandCreateLobby(InteractionContext ctx)
        {
            await _roomsSelectors.CreateLobbyCommand(ctx);
        }

        [SlashCommand("lobby-delete", "Delete a lobby for private voice channels")]
        [SlashCommandPermissions(DSharpPlus.Permissions.ManageChannels)]
        public async Task CommandDeleteLobby(InteractionContext ctx)
        {
            await _roomsSelectors.DeleteLobbyCommand(ctx);
        }

        [SlashCommand("vclock", "Lock and unlock voice channel")]
        public async Task CommandLockAndUnlockVoice(InteractionContext ctx)
        {
            await _roomsSelectors.LockPrivateVoice(ctx);
        }
    }
}
