

namespace ReworkZenithBeep.Data.Models.items
{
    public class ItemGuild : Entity
    {
        public string Name { get; set; } = string.Empty;
        public string Lang { get; set; } = "En-us";
        public string Prefix { get; set; } = "!";

        public ICollection<ItemRolesSelector> Roles { get; } = new List<ItemRolesSelector>();
        public ICollection<ItemRoomersLobby> RoomersLobby { get; } = new List<ItemRoomersLobby>();
    }
}
