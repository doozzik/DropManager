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
            UnturnedPlayerEvents.OnPlayerDeath += Drop;
        }

        protected override void Unload()
        {
            UnturnedPlayerEvents.OnPlayerDeath -= Drop;
        }

        private void Drop(UnturnedPlayer player, EDeathCause cause, ELimb limb, Steamworks.CSteamID murderer)
        {
            if (!Configuration.Instance.LeftOtherDrop)
            {
                ClearAllItems(player);
                ClearAllClothes(player);
            }

            ClearBlackListItems(player, Configuration.Instance.BlackListIds);
            ClearBlackListClothes(player, Configuration.Instance.BlackListIds);

            DropAllItems(player);
            AddItems(player);
        }

        private void ClearAllItems(UnturnedPlayer player)
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

        private void ClearBlackListItems(UnturnedPlayer player, string blackList)
        {
            try
            {
                player.Player.equipment.dequip();
                for (byte page = 0; page < PlayerInventory.PAGES; page++)
                {
                    byte itemCount = player.Player.inventory.getItemCount(page);
                    if (itemCount > 0)
                    {
                        for (byte index = 0; index < itemCount; index++)
                        {
                            ushort id = player.Player.inventory.getItem(page, index).item.id;
                            if (blackList.Contains(id.ToString() + ','))
                            {
                                player.Player.inventory.removeItem(page, index);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log("[DropManager] Warning: " + e.Message);
            }
        }

        private void ClearAllClothes(UnturnedPlayer player)
        {
            try // this code belongs to Zaup mod developer
            {
                player.Player.Clothing.askWearBackpack(0, 0, new byte[0]);
                ClearAllItems(player);

                player.Player.Clothing.askWearGlasses(0, 0, new byte[0]);
                ClearAllItems(player);

                player.Player.Clothing.askWearHat(0, 0, new byte[0]);
                ClearAllItems(player);

                player.Player.Clothing.askWearMask(0, 0, new byte[0]);
                ClearAllItems(player);

                player.Player.Clothing.askWearPants(0, 0, new byte[0]);
                ClearAllItems(player);

                player.Player.Clothing.askWearShirt(0, 0, new byte[0]);
                ClearAllItems(player);

                player.Player.Clothing.askWearVest(0, 0, new byte[0]);
                ClearAllItems(player);
            }
            catch (Exception e)
            {
                Logger.Log("[DropManager] Warning: " + e.Message);
            }
        }

        private void ClearBlackListClothes(UnturnedPlayer player, string blackList)
        {
            try // this code belongs to Zaup mod developer
            {
                player.Player.Clothing.askWearBackpack(0, 0, new byte[0]);
                ClearBlackListItems(player, blackList);
                DropAllItems(player);

                player.Player.Clothing.askWearGlasses(0, 0, new byte[0]);
                ClearBlackListItems(player, blackList);
                DropAllItems(player);

                player.Player.Clothing.askWearHat(0, 0, new byte[0]);
                ClearBlackListItems(player, blackList);
                DropAllItems(player);

                player.Player.Clothing.askWearMask(0, 0, new byte[0]);
                ClearBlackListItems(player, blackList);
                DropAllItems(player);

                player.Player.Clothing.askWearPants(0, 0, new byte[0]);
                ClearBlackListItems(player, blackList);
                DropAllItems(player);

                player.Player.Clothing.askWearShirt(0, 0, new byte[0]);
                ClearBlackListItems(player, blackList);
                DropAllItems(player);

                player.Player.Clothing.askWearVest(0, 0, new byte[0]);
                ClearBlackListItems(player, blackList);
                DropAllItems(player);
            }
            catch (Exception e)
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
                            if(player.Player.inventory.getItemCount(page) > 0)
                            {
                                try
                                {
                                    player.Player.inventory.askDropItem(player.CSteamID, page, i, k);
                                }
                                catch (Exception e)
                                {
                                    Logger.Log("[DropManager] Warning: " + e.Message);
                                }
                            }
                            else
                            {
                                return;
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
