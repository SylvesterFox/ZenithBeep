

using System.ComponentModel.DataAnnotations.Schema;

namespace ReworkZenithBeep.Data.Models.items
{
    public class ItemRooomsSettings
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public ulong Id { get; set; }
        public ItemUser User { get; set; }
        public string nameChannel { get; set; } = string.Empty;
        public int limitChannel { get; set; } = 0;
    }
}
