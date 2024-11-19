

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReworkZenithBeep.Data.Models.items
{
    public class ItemsTempRoom
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public ulong ownerUser { get; set; }
        public ulong roomid { get; set; }
    }
}
