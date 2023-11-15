using System;
using System.Collections.Generic;
using System.Linq;
using CustomPlayerEffects;
using Discord;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using InventorySystem.Items.Autosync;
using InventorySystem.Items.ThrowableProjectiles;
using InventorySystem.Items.Usables;
using PlayerRoles;
using MEC;

namespace AutoReconnectRemastered
{
    public class PlayerData
    {
        private readonly AutoReconnect _instance;

        internal PlayerData(AutoReconnect instance) => this._instance = instance;
        
        public Dictionary<string, PlayerHandlers> Players = new();
        
        public void AddPlayer(Player player)
        {
            if (player != null && player.IsAlive)
            {
                PlayerHandlers Player = new PlayerHandlers(_instance)
                {
                    Ammo = new Dictionary<ItemType, ushort>(),
                    Ammo_Clone = new Dictionary<ItemType, ushort>(),
                    Class = player.Role.Type,
                    Effects = player.ActiveEffects,
                    Effects_Repertory = new Dictionary<EffectType, (byte, float)>(),
                    Health = player.Health,
                    Inventory = player.Items,
                    Inventory_Clone = new List<Item>(),
                    Name = player.Nickname,
                    Position_X = player.Position.x,
                    Position_Y = player.Position.y,
                    Position_Z = player.Position.z,
                    Player = player,
                };
                Players[player.UserId] = Player;
                
                CloneInventory(player);
                AutoReconnect.Instance.AmmoData.StoreAmmo(player);
                AutoReconnect.Instance.EffectData.StoreEffects(player);
            }
        }
        
        public void ClearPlayerData()
        {
            Players.Clear();
        }

        public void CloneInventory(Player player)
        {
            PlayerHandlers playerData = GetPlayerData(player);
            try
            {
                foreach (Item item in playerData.Inventory)
                {
                    Item clonedItem = item.Clone();
                    playerData.Inventory_Clone.Add(clonedItem);
                }
            }
            catch (Exception e)
            {
                var text = $"Clone failed. Reason: {e}";
                Log.Error(text);
                throw;
            }
        }

        public PlayerHandlers GetPlayerData(Player player)
        {
            if (Players.ContainsKey(player.UserId))
            {
                return Players[player.UserId];
            }
            return null;
        }
        
        public void RemovePlayerData(Player player)
        {
            if (player != null && Players.ContainsKey(player.UserId))
            {
                Players.Remove(player.UserId);
                Log.Info($"Removed data for player {player.Nickname} with SteamID: {player.UserId}");
            }
            else
            {
                Log.Info($"Player {player.Nickname} with SteamID {player.UserId} not found in the data dictionary.");
            }
        }
        
        public void ResurrectPlayer(Player player, PlayerHandlers playerData)
        {
            if (player != null && playerData != null)
            {
                Log.Info("------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                Log.Info($"Entering Method: ResurrectPlayer. Target player: {player.Nickname}, ID: {player.UserId}");
                
                player.Role.Set(playerData.Class, RoleSpawnFlags.None);
                Log.Info($"Player's Role is now {playerData.Class}.");
                player.Position = new UnityEngine.Vector3(playerData.Position_X, playerData.Position_Y, playerData.Position_Z);
                Log.Info($"Player's pos is now {playerData.Position_X}, {playerData.Position_Y}, {playerData.Position_Z}.");
                player.Health = playerData.Health;
                Log.Info($"Player's health is now {playerData.Health}.");

                if (playerData.Inventory_Clone.Count == 0)
                {
                    Log.Info("playerData.Inventory_Clone is empty.");
                }
                try
                {
                    Log.Info($"Trying restoring player's inventory...");
                    
                    var itemsToRemove = new List<Item>();
                    foreach (Item item in playerData.Inventory_Clone)
                    {
                        item.ChangeItemOwner(playerData.Player, player);
                        Log.Info($"Changed Item {item.Type} owner from previous into new.");
                        item.Give(player);
                        Log.Info($"Item {item.Type} has been successfully given to {player.Nickname}.");
                        itemsToRemove.Add(item);
                    }
                    
                    Log.Info("Player's Inventory has been successfully restored.");
                    
                    AutoReconnect.Instance.AmmoData.RestoreAmmo(player);

                    AutoReconnect.Instance.EffectData.RestoreEffects(player);
                    
                    
                    foreach (var itemToRemove in itemsToRemove)
                    {
                        playerData.Inventory_Clone.Remove(itemToRemove);
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    throw;
                }
                
                Log.Info("------------------------------------------------------------------------------------------------------------------------------------------------------------------");
            }
        }
        
        public void DisplayPlayersInfo()
        {
            Log.Info("------------------------------------------------------------------------------------------------------------------------------------------------------------------");
            var disconnectedPlayers = AutoReconnect.Instance.EventHandlers.DisconnectedPlayers;
            var Tps = Server.Tps;
            if (Tps <= 50)
            {
                AutoReconnect.Instance.Timer.DelayedTime = 5f;
                Log.Info($"Detected performence issue.Lowered running fquency on AutoReconnect plugin now.");
                Log.Info($"Current TPS: {Tps}");
            }
            else
            {
                AutoReconnect.Instance.Timer.DelayedTime = 1f;
            }
            foreach (var steamID in Players.Keys)
            {
                PlayerHandlers playerData = Players[steamID];
                if (disconnectedPlayers.ContainsKey(steamID))
                {
                    Log.Info($"Name: {playerData.Name}, SteamID: {steamID}, Health: {playerData.Health}, Class: {playerData.Class}, POS: {playerData.Position_X}, {playerData.Position_Y}, {playerData.Position_Z}, Room: {playerData.Player.CurrentRoom.Name} Status: Disconnected.");
                }
                else
                {
                    Log.Info($"Name: {playerData.Name}, SteamID: {steamID}, Health: {playerData.Health}, Class: {playerData.Class}, POS: {playerData.Position_X}, {playerData.Position_Y}, {playerData.Position_Z}, Room: {playerData.Player.CurrentRoom.Name} Status: Connected.");
                }
            }
            Log.Info("------------------------------------------------------------------------------------------------------------------------------------------------------------------");
        }
    }
}