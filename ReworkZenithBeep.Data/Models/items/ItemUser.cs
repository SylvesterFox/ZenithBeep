

namespace ReworkZenithBeep.Data.Models.items
{
    public class ItemUser : Entity
    {
        public string UserName { get; set; } = string.Empty;
        public int cookieBox { get; set; } = 0;

        public ICollection<ItemRooomsSettings> Roomers { get; } = new List<ItemRooomsSettings>();
        public ICollection<ItemTempRoom> itemTempRooms { get; } = new List<ItemTempRoom>();
    }
}
