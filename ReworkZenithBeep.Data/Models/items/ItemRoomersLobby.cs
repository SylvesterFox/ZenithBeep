

using System.ComponentModel.DataAnnotations;
using System.Data;

namespace ReworkZenithBeep.Data.Models.items
{
    public class ItemRoomersLobby
    {
        [Key]
        public int keyId {  get; set; }
        public ulong Id { get; set; }
        public ItemGuild Guild { get; set; }
        public ulong LobbyId { get; set; }

        public ICollection<ItemsRoooms> itemsRoooms { get; } = new List<ItemsRoooms>();
    }
}
