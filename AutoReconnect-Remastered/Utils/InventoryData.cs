using System;
using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.API.Features.Items;

namespace AutoReconnectRemastered
{
    public class InventoryData
    {
        private readonly AutoReconnect _instance;

        internal InventoryData(AutoReconnect instance) => this._instance = instance;
        
        public void CloneInventory(Player player)
        {
            PlayerHandlers PlayerData = AutoReconnect.Instance.PlayerData.GetPlayerData(player);
            try
            {
                foreach (Item item in PlayerData.Inventory)
                {
                    Item clonedItem = item.Clone();
                    PlayerData.Inventory_Clone.Add(clonedItem);
                }
            }
            catch (Exception e)
            {
                var text = $"Clone failed. Reason: {e}";
                Log.Error(text);
                throw;
            }
        }

        public void RestoreInventory(Player player)
        {
            PlayerHandlers PlayerData = AutoReconnect.Instance.PlayerData.GetPlayerData(player);
            
            if (PlayerData.Inventory_Clone.Count == 0)
            {
                Log.Info("PlayerData.Inventory_Clone is empty.");
            }
            try
            {
                Log.Info($"Trying restoring player's inventory...");
                    
                var itemsToRemove = new List<Item>();
                foreach (Item item in PlayerData.Inventory_Clone)
                {
                    item.ChangeItemOwner(PlayerData.Player, player);
                    item.Give(player);
                    itemsToRemove.Add(item);
                }
                Log.Info("Player's Inventory has been successfully restored.");
                    
                foreach (var itemToRemove in itemsToRemove)
                {
                    PlayerData.Inventory_Clone.Remove(itemToRemove);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }
        }
    }
}

