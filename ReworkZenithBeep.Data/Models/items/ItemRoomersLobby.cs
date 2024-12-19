

using System.ComponentModel.DataAnnotations.Schema;


namespace ReworkZenithBeep.Data.Models.items
{
    public class ItemRoomersLobby
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public ulong Id { get; set; }
        public ItemGuild Guild { get; set; }
        public ulong LobbyId { get; set; }
    }
}
