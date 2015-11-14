using Rocket.API;
using System.Collections.Generic;

namespace DropManager
{
    public class DropManagerConfiguration : IRocketPluginConfiguration
    {
        public bool LeftOtherDrop;

        public List<Item> Items;

        public void LoadDefaults()
        {
            LeftOtherDrop = true;

            Items = new List<Item>
            {
                new Item("Venison Raw meat", 514, 1, 2),
                new Item("Tomato", 340, 0, 1),
            };
        }
    }
}
