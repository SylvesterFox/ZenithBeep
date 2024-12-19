

using System.ComponentModel.DataAnnotations.Schema;

namespace ReworkZenithBeep.Data.Models.items
{
    public class ItemTempRoom
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public ulong roomid { get; set; }
        public ulong Id { get; set; }
        
    }
}
