using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using Rocket.Core.Logging;
using Rocket.Unturned.Events;
using SDG.Unturned;
using System;
using System.Collections.Generic;

namespace DropManager
{
    public class DropManager : RocketPlugin<DropManagerConfiguration>
    {
        public static DropManager Instance;

        private bool showWarnings;
        private List<ushort> blackList = new List<ushort>();
        
        protected override void Load()
        {
            Instance = this;

            UnturnedPlayerEvents.OnPlayerDeath += Drop;
        }

        protected override void Unload()
        {
            UnturnedPlayerEvents.OnPlayerDeath -= Drop;
        }

        private void Drop(UnturnedPlayer player, EDeathCause cause, ELimb limb, Steamworks.CSteamID murderer)
        {
            this.showWarnings = Configuration.Instance.ShowWarnings;

            if (!Configuration.Instance.LeftOtherDrop) // if we want to clear players inventory before death
            {
                ClearAllItems(player);
                ClearAllClothes(player);
            }
            else
            {
                BlackListController(Configuration.Instance.BlackListIds); // convert string to ushort list

                ClearBlackListedItems(player); // remove only items, which contains in blacklist
                ClearBlackListedClothes(player);

                DropAllItems(player); // we will drop all items for get a free space to add new items
            }
            
            AddItems(player); // add new items
        }

        private void BlackListController(string configBlackList)
        {
            configBlackList = configBlackList.Replace(" ", "");
            string[] array = configBlackList.Split(',');
            
            foreach (string str in array)
            {
                try
                {
                    blackList.Add(Convert.ToUInt16(str));
                }
                catch (Exception e)
                {
                    if (showWarnings)
                    {
                        Logger.LogWarning(@"[DropManager] Warning: Cant convert your BlackListIds string to integer.");
                        Logger.LogWarning(@"[DropManager] Warning: You can use only: numbers 0-9, whitespaces ("" ""), and commas ("","")");
                        Logger.LogWarning(@"[DropManager] Warning: For example: 12, 28, 300, 301, 312");
                        Logger.LogWarning(@"[DropManager] Warning: Full problem description: " + e.Message);
                    }
                }
            }
        }

        private void ClearAllItems(UnturnedPlayer player)
        {
            for (byte page = 0; page < PlayerInventory.PAGES; page++)
            {
                byte itemCount = player.Player.inventory.getItemCount(page);
                for (byte index = 0; index < itemCount; index++)
                {
                    try
                    {
                        player.Player.inventory.removeItem(page, index);
                        index--;
                        itemCount--;
                    }
                    catch (Exception e)
                    {
                        if (showWarnings)
                        {
                            Logger.LogWarning(@"[DropManager] Warning: Cant clean inventory for player " + player.CharacterName);
                            Logger.LogWarning(@"[DropManager] Warning: Full problem description: " + e.Message);
                        }
                    }
                }
            }
        }

        private void ClearBlackListedItems(UnturnedPlayer player)
        {
            for (byte page = 0; page < PlayerInventory.PAGES; page++)
            {
                byte itemCount = player.Player.inventory.getItemCount(page);
                for (byte index = 0; index < itemCount; index++)
                {
                    ushort id = player.Player.inventory.getItem(page, index).item.id;
                    if (blackList.Contains(id))
                    {
                        try
                        {
                            player.Player.inventory.removeItem(page, index);
                            index--;
                            itemCount--;
                        }
                        catch (Exception e)
                        {
                            if (showWarnings)
                            {
                                Logger.LogWarning(@"[DropManager] Warning: Cant clean BlackListed item for player " + player.CharacterName);
                                Logger.LogWarning(@"[DropManager] Warning: Full problem description: " + e.Message);
                            }
                        }
                    }
                }
            }
        }

        private void ClearAllClothes(UnturnedPlayer player)
        {
            try
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
                if (showWarnings)
                {
                    Logger.LogWarning(@"[DropManager] Warning: Cant clean clothes for player " + player.CharacterName);
                    Logger.LogWarning(@"[DropManager] Warning: Full problem description: " + e.Message);
                }
            }
        }

        private void ClearBlackListedClothes(UnturnedPlayer player)
        {
            player.Player.Clothing.askWearBackpack(0, 0, new byte[0]);
            ClearBlackListedItems(player);
            DropAllItems(player);

            player.Player.Clothing.askWearGlasses(0, 0, new byte[0]);
            ClearBlackListedItems(player);
            DropAllItems(player);

            player.Player.Clothing.askWearHat(0, 0, new byte[0]);
            ClearBlackListedItems(player);
            DropAllItems(player);

            player.Player.Clothing.askWearMask(0, 0, new byte[0]);
            ClearBlackListedItems(player);
            DropAllItems(player);

            player.Player.Clothing.askWearPants(0, 0, new byte[0]);
            ClearBlackListedItems(player);
            DropAllItems(player);

            player.Player.Clothing.askWearShirt(0, 0, new byte[0]);
            ClearBlackListedItems(player);
            DropAllItems(player);

            player.Player.Clothing.askWearVest(0, 0, new byte[0]);
            ClearBlackListedItems(player);
            DropAllItems(player);
        }

        private void DropAllItems(UnturnedPlayer player)
        {
            for (byte page = 0; page < PlayerInventory.PAGES; page++)
            {
                byte itemCount = player.Player.inventory.getItemCount(page);
                for (byte index = 0; index < itemCount; index++)
                {
                    byte posX = player.Player.inventory.getItem(page, index).PositionX;
                    byte posY = player.Player.inventory.getItem(page, index).PositionY;
                    try
                    {
                        player.Player.inventory.askDropItem(player.CSteamID, page, posX, posY);
                        index--;
                        itemCount--;
                    }
                    catch (Exception e)
                    {
                        if (showWarnings)
                        {
                            Logger.LogWarning(@"[DropManager] Warning: Cant drop item for player " + player.CharacterName);
                            Logger.LogWarning(@"[DropManager] Warning: Full problem description: " + e.Message);
                        }
                    }
                }
            }
        }

        private void AddItems(UnturnedPlayer player)
        {
            foreach (Item item in Configuration.Instance.Items) // maybe hard to understand, sorry
            {
                System.Random rand = new System.Random();
                int random = rand.Next(item.min, item.max + 1);

                try
                {
                    SDG.Unturned.Item itemToAdd = new SDG.Unturned.Item(item.id, true);

                    for (int i = random; i > 0; i--)
                    {
                        try
                        {
                            player.Player.inventory.tryAddItem(itemToAdd, true);
                        }
                        catch (Exception e)
                        {
                            if (showWarnings)
                            {
                                Logger.LogWarning(@"[DropManager] Warning: Cant add item to player " + player.CharacterName);
                                Logger.LogWarning(@"[DropManager] Warning: Full problem description: " + e.Message);
                            }
                        }

                        DropAllItems(player);
                    }
                }
                catch (Exception e)
                {
                    if (showWarnings)
                    {
                        Logger.LogWarning(@"[DropManager] Warning: Cant find item by this id: " + item.id);
                        Logger.LogWarning(@"[DropManager] Warning: Check config and site, where you get Unturned items id list");
                        Logger.LogWarning(@"[DropManager] Warning: Full problem description: " + e.Message);
                    }
                }
            }
        }
    }
}
