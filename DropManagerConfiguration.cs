using Rocket.API;
using System.Collections.Generic;

namespace DropManager
{
    public class DropManagerConfiguration : IRocketPluginConfiguration
    {
        public bool ShowWarnings;
        public string LeftOtherDrop;

        public List<Item> Items;

        public string BlackListIds;

        public void LoadDefaults()
        {
            ShowWarnings = true;
            LeftOtherDrop = "100%";
            
            Items = new List<Item>
            {
                new Item("Venison Raw meat", 514, 1, 2),
                new Item("Tomato", 340, 0, 1),
                new Item("5 dollars", 1051, 1, 1)
            };

            BlackListIds = "47132, 58950, 41829";
        }
    }
}
