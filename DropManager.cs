using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using Rocket.Core.Logging;
using Rocket.Unturned.Events;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using Rocket.API;

namespace DropManager
{
    public class DropManager : RocketPlugin<DropManagerConfiguration>
    {
        public static DropManager Instance;
        private int invPages = PlayerInventory.PAGES - 1;
        private System.Random rand = new System.Random();

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
            bool showWarnings = Configuration.Instance.ShowWarnings;
            string leftOtherDrop = Configuration.Instance.LeftOtherDrop;
            string blackListIds = Configuration.Instance.BlackListIds;

            if (player.IsAdmin || player.HasPermission("dropmanager.alwaysclear")) // if we want to clear players inventory before death
            {
                ClearAllItems(player, showWarnings);
                ClearAllClothes(player, showWarnings);
            }
            else
            {
                List<ushort> blackList = new List<ushort>();
                
                BlackListController(blackListIds, blackList, showWarnings); // convert string to ushort list
                ClearBlackListedItems(player, blackList, showWarnings); // remove only items, which contains in blacklist
                Logger.Log(blackList.Count.ToString());
                int allItemsAmount = 0;
                int amountOfItemsToClear = 0;
                for (byte page = 0; page < invPages; page++)
                {
                    allItemsAmount += player.Player.inventory.getItemCount(page); // set allItemsAmount
                }
                amountOfItemsToClear = CalculateAmountOfRandomItemsToClear(player, allItemsAmount, leftOtherDrop, showWarnings);
                if (amountOfItemsToClear == allItemsAmount && allItemsAmount != 0)
                {
                    ClearAllItems(player, showWarnings);
                    ClearAllClothes(player, showWarnings);
                }
                else
                {
                    ClearRandomItems(player, amountOfItemsToClear, showWarnings);
                }
                
                DropAllItems(player, showWarnings); // we will drop all items for get a free space to new items
                ClearBlackListedClothes(player, blackList, showWarnings);
            }
            
            if (!player.IsAdmin && !player.HasPermission("dropmanager.alwaysclear"))
            {
                AddItems(player, showWarnings); // add to drop new items, if player is not an admin or dont have special permission
            }
            
        }

        private void BlackListController(string configBlackList, List<ushort> blackList, bool showWarnings)
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

        private void ClearAllItems(UnturnedPlayer player, bool showWarnings)
        {
            for (byte page = 0; page < invPages; page++)
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

        private void ClearBlackListedItems(UnturnedPlayer player, List<ushort> blackList, bool showWarnings)
        {
            for (byte page = 0; page < invPages; page++)
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

        // Thanks to ZaupClearInventoryLib
        private void ClearAllClothes(UnturnedPlayer player, bool showWarnings)
        {
            try
            {
                player.Player.clothing.askWearBackpack(0, 0, new byte[0], true);
                ClearAllItems(player, showWarnings);

                player.Player.clothing.askWearGlasses(0, 0, new byte[0], true);
                ClearAllItems(player, showWarnings);

                player.Player.clothing.askWearHat(0, 0, new byte[0], true);
                ClearAllItems(player, showWarnings);

                player.Player.clothing.askWearMask(0, 0, new byte[0], true);
                ClearAllItems(player, showWarnings);

                player.Player.clothing.askWearPants(0, 0, new byte[0], true);
                ClearAllItems(player, showWarnings);

                player.Player.clothing.askWearShirt(0, 0, new byte[0], true);
                ClearAllItems(player, showWarnings);

                player.Player.clothing.askWearVest(0, 0, new byte[0], true);
                ClearAllItems(player, showWarnings);
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

        private void ClearBlackListedClothes(UnturnedPlayer player, List<ushort> blackList, bool showWarnings)
        {
            player.Player.clothing.askWearBackpack(0, 0, new byte[0], true);
            ClearBlackListedItems(player, blackList, showWarnings);
            DropAllItems(player, showWarnings);

            player.Player.clothing.askWearGlasses(0, 0, new byte[0], true);
            ClearBlackListedItems(player, blackList, showWarnings);
            DropAllItems(player, showWarnings);

            player.Player.clothing.askWearHat(0, 0, new byte[0], true);
            ClearBlackListedItems(player, blackList, showWarnings);
            DropAllItems(player, showWarnings);

            player.Player.clothing.askWearMask(0, 0, new byte[0], true);
            ClearBlackListedItems(player, blackList, showWarnings);
            DropAllItems(player, showWarnings);

            player.Player.clothing.askWearPants(0, 0, new byte[0], true);
            ClearBlackListedItems(player, blackList, showWarnings);
            DropAllItems(player, showWarnings);

            player.Player.clothing.askWearShirt(0, 0, new byte[0], true);
            ClearBlackListedItems(player, blackList, showWarnings);
            DropAllItems(player, showWarnings);

            player.Player.clothing.askWearVest(0, 0, new byte[0], true);
            ClearBlackListedItems(player, blackList, showWarnings);
            DropAllItems(player, showWarnings);
        }

        private void ClearRandomItems(UnturnedPlayer player, int amount, bool showWarnings)
        {
            while (amount > 0)
            {
                byte page = Convert.ToByte(rand.Next(0, invPages));
                byte itemsCountOnPage = player.Player.inventory.getItemCount(page);

                if (itemsCountOnPage > 0)
                {
                    byte index = Convert.ToByte(rand.Next(0, itemsCountOnPage));
                    try
                    {
                        player.Player.inventory.removeItem(page, index);
                        amount--;
                    }
                    catch (Exception e)
                    {
                        if (showWarnings)
                        {
                            Logger.LogWarning(@"[DropManager] Warning: Cant ClearRandomItem for player " + player.CharacterName);
                            Logger.LogWarning(@"[DropManager] Warning: Full problem description: " + e.Message);
                        }
                    }
                }
            }
        }

        private int CalculateAmountOfRandomItemsToClear(UnturnedPlayer player, int allItemsAmount, string percentage, bool showWarnings)
        {
            string sourcePercentage = percentage;
            percentage = percentage.Trim();
            percentage = percentage.Trim('%');

            int finalPercentage;
            
            if (percentage == "true")
            {
                if (showWarnings)
                {
                    Logger.LogWarning(@"[DropManager] Warning: Seems, like you have updated this plugin, but dont remove config before this.");
                    Logger.LogWarning(@"[DropManager] Warning: Cant CalculateAmountOfRandomItemsToClear.");
                    Logger.LogWarning(@"[DropManager] Warning: You set DropPercentage to: " + sourcePercentage);
                    Logger.LogWarning(@"[DropManager] Warning: DropPercentage can be, for example: 0% (clear all drop), 15%, 50%, 100% (left all drop)");
                    Logger.LogWarning(@"[DropManager] Warning: We will set DropPercentage to: 100%");
                }
                finalPercentage = 100;
            }

            if (percentage == "false")
            {
                if (showWarnings)
                {
                    Logger.LogWarning(@"[DropManager] Warning: Seems, like you have updated this plugin, but dont remove config before this.");
                    Logger.LogWarning(@"[DropManager] Warning: Cant CalculateAmountOfRandomItemsToClear.");
                    Logger.LogWarning(@"[DropManager] Warning: You set DropPercentage to: " + sourcePercentage);
                    Logger.LogWarning(@"[DropManager] Warning: DropPercentage can be, for example: 0% (clear all drop), 15%, 50%, 100% (left all drop)");
                    Logger.LogWarning(@"[DropManager] Warning: We will set DropPercentage to: 0%");
                }
                finalPercentage = 0;
            }

            try
            {
                finalPercentage = Convert.ToInt32(percentage);
            }
            catch (Exception e)
            {
                Logger.LogWarning(@"[DropManager] Warning: Cant CalculateAmountOfRandomItemsToClear: cant convert your value to integer");
                Logger.LogWarning(@"[DropManager] Warning: You set DropPercentage to: " + sourcePercentage);
                Logger.LogWarning(@"[DropManager] Warning: DropPercentage can be, for example: 0% (clear all drop), 15%, 50%, 100% (left all drop)");
                Logger.LogWarning(@"[DropManager] Warning: We will set DropPercentage to: 100%");
                Logger.LogWarning(@"[DropManager] Warning: Full problem description: " + e.Message);
                finalPercentage = 100;
            }

            if (finalPercentage < 0 || finalPercentage > 100)
            {
                Logger.LogWarning(@"[DropManager] Warning: Cant CalculateAmountOfRandomItemsToClear: your value is less than 0% or bigger than 100%");
                Logger.LogWarning(@"[DropManager] Warning: You set DropPercentage to: " + sourcePercentage);
                Logger.LogWarning(@"[DropManager] Warning: DropPercentage can be, for example: 0% (clear all drop), 15%, 50%, 100% (left all drop)");
                Logger.LogWarning(@"[DropManager] Warning: We will set DropPercentage to: 100%");
                finalPercentage = 100;
            }

            if (finalPercentage == 100)
            {
                return 0;
            }

            if (finalPercentage == 0)
            {
                return allItemsAmount;
            }

            return Convert.ToInt32(allItemsAmount - Math.Ceiling(allItemsAmount * finalPercentage / (double)100));
        }
        
        private void DropAllItems(UnturnedPlayer player, bool showWarnings)
        {
            for (byte page = 0; page < invPages; page++)
            {
                byte itemCount = player.Player.inventory.getItemCount(page);
                for (byte index = 0; index < itemCount; index++)
                {
                    byte posX = player.Player.inventory.getItem(page, index).x;
                    byte posY = player.Player.inventory.getItem(page, index).y;
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

        private void AddItems(UnturnedPlayer player, bool showWarnings)
        {
            foreach (Item item in Configuration.Instance.Items) // maybe hard to understand, sorry
            {
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

                        DropAllItems(player, showWarnings);
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
