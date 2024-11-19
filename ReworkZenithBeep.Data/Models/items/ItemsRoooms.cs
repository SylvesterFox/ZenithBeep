

namespace ReworkZenithBeep.Data.Models.items
{
    public class ItemsRoooms
    {
        public ulong userId { get; set; }
        public ulong LobbyId { get; set; }
        public ItemRoomersLobby ItemRoomersLobby { get; set; }
        public string nameChannel { get; set; } = string.Empty;
        public int limitChannel { get; set; } = 0;

    }
}
