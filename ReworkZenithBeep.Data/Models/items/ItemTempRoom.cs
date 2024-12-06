

using System.ComponentModel.DataAnnotations.Schema;

namespace ReworkZenithBeep.Data.Models.items
{
    public class ItemTempRoom
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public ulong Id { get; set; }
        public ItemUser User { get; set; }
        public ulong roomid { get; set; }
    }
}
