

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReworkZenithBeep.Data.Models.items
{
    [Keyless]
    public class ItemsRoooms
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public ulong LobbyId { get; set; }
        public ItemRoomersLobby ItemRoomersLobby { get; set; }
        public ulong userId { get; set; }
        public string nameChannel { get; set; } = string.Empty;
        public int limitChannel { get; set; } = 0;

    }
}
