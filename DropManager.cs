using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using Rocket.Core.Logging;
using Rocket.Unturned.Events;
using SDG.Unturned;
using System;

namespace DropManager
{
    public class DropManager : RocketPlugin<DropManagerConfiguration>
    {
        public static DropManager Instance;

        private System.Random rand = new System.Random();
        private int random;

        protected override void Load()
        {
            Instance = this;

            UnturnedPlayerEvents.OnPlayerDeath += Drop;
        }

        private void Drop(UnturnedPlayer player, EDeathCause cause, ELimb limb, Steamworks.CSteamID murderer)
        {
            if (!Configuration.Instance.LeftOtherDrop)
            {
                ClearItems(player);
                ClearClothes(player);
            }

            DropAllItems(player);
            AddItems(player);
        }

        private void ClearItems(UnturnedPlayer player)
        {
            try // this code belongs to Zaup mod developer
            {
                player.Player.equipment.dequip();
                for (byte page = 0; page < PlayerInventory.PAGES; page++)
                {
                    byte itemCount = player.Player.inventory.getItemCount(page);
                    if (itemCount > 0)
                    {
                        for (byte item = 0; item < itemCount; item++)
                        {
                            player.Player.inventory.removeItem(page, 0);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log("[DropManager] Warning: " + e.Message);
            }
        }

        private void ClearClothes(UnturnedPlayer player)
        {
            try // this code belongs to Zaup mod developer
            {
                player.Player.Clothing.askWearBackpack(0, 0, new byte[0]);
                for (byte p2 = 0; p2 < player.Player.Inventory.getItemCount(2); p2++)
                {
                    player.Player.Inventory.removeItem(2, 0);
                }
                player.Player.Clothing.askWearGlasses(0, 0, new byte[0]);
                for (byte p2 = 0; p2 < player.Player.Inventory.getItemCount(2); p2++)
                {
                    player.Player.Inventory.removeItem(2, 0);
                }
                player.Player.Clothing.askWearHat(0, 0, new byte[0]);
                for (byte p2 = 0; p2 < player.Player.Inventory.getItemCount(2); p2++)
                {
                    player.Player.Inventory.removeItem(2, 0);
                }
                player.Player.Clothing.askWearMask(0, 0, new byte[0]);
                for (byte p2 = 0; p2 < player.Player.Inventory.getItemCount(2); p2++)
                {
                    player.Player.Inventory.removeItem(2, 0);
                }
                player.Player.Clothing.askWearPants(0, 0, new byte[0]);
                for (byte p2 = 0; p2 < player.Player.Inventory.getItemCount(2); p2++)
                {
                    player.Player.Inventory.removeItem(2, 0);
                }
                player.Player.Clothing.askWearShirt(0, 0, new byte[0]);
                for (byte p2 = 0; p2 < player.Player.Inventory.getItemCount(2); p2++)
                {
                    player.Player.Inventory.removeItem(2, 0);
                }
                player.Player.Clothing.askWearVest(0, 0, new byte[0]);
                for (byte p2 = 0; p2 < player.Player.Inventory.getItemCount(2); p2++)
                {
                    player.Player.Inventory.removeItem(2, 0);
                }
            }
            catch(Exception e)
            {
                Logger.Log("[DropManager] Warning: " + e.Message);
            }
        }

        private void DropAllItems(UnturnedPlayer player)
        {
            player.Player.equipment.dequip();
            for (byte page = 0; page < PlayerInventory.PAGES; page++)
            {
                byte itemCount = player.Player.inventory.getItemCount(page);
                if (itemCount > 0)
                {
                    byte width = player.Player.inventory.getWidth(page);
                    byte height = player.Player.inventory.getHeight(page);

                    for (byte i = 0; i < width; i++)
                    {
                        for (byte k = 0; k < height; k++)
                        {
                            try
                            {
                                player.Player.inventory.askDropItem(player.CSteamID, page, i, k);
                            }
                            catch(Exception e)
                            {
                                Logger.Log("[DropManager] Warning: " + e.Message);
                            }
                        }
                    }
                }
            }
        }

        private void AddItems(UnturnedPlayer player)
        {
            try
            {
                foreach(Item item in Configuration.Instance.Items) // maybe hard to understand, sorry
                {
                    random = rand.Next(item.min, item.max + 1);
                    
                    if (random > 0)
                    {
                        SDG.Unturned.Item itemToAdd = new SDG.Unturned.Item(item.id, true);
                        for (int i = random; i > 0; i--)
                        {
                            player.Player.inventory.tryAddItem(itemToAdd, true);
                            DropAllItems(player);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Logger.Log("[DropManager] Wanring: " + e.Message);
            }
        }
    }
}
