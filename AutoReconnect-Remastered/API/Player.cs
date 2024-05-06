﻿using System;
using System.Collections.Generic;
using System.Linq;
using AutoReconnectRemastered;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
// using Exiled.CustomModules.API.Features;
// using Exiled.CustomModules.API.Features.CustomItems;
// using Exiled.CustomModules.API.Features.CustomRoles;
using PlayerRoles;

namespace AutoReconnect_Remastered.API
{
    public class PlayerApi
    {
        public static List<Player>? AllPlayers = new();
        public static Dictionary<string, PlayerData> DisconnectedPlayers = new();

        public static void AddPlayer(Player player)
        {
            if (player == null && !player.IsAlive) return;

            PlayerData PlayerHandler = new PlayerData(player)
            {
                Ammo = new Dictionary<ItemType, ushort>(),
                Class = player.Role.Type,
                Health = player.Health,
                Inventory = new HashSet<Item>(),
                Name = player.Nickname,
                Position = player.Position,
                Effects = new(),
                Player = player,
                Time = DateTime.Now,
            };

            if (player.Role.Type == RoleTypeId.Scp079)
            {
                PlayerHandler.Exp = player.Role.As<Scp079Role>().Experience;
                PlayerHandler.Level = player.Role.As<Scp079Role>().Level;
                PlayerHandler.Energy = player.Role.As<Scp079Role>().Energy;
            }
            
            /*var customRole = CustomRole.Get(player);
            if (customRole != null)
            {
                PlayerHandler.CustomRole = customRole;
            }
            else
            {
                PlayerHandler.CustomRole = null;
            }*/

            DisconnectedPlayers.Add(player.UserId, PlayerHandler);

            CallSto(player, AutoReconnect.Instance.Config.RecoveryInventory, AutoReconnect.Instance.Config.RecoveryAmmo, AutoReconnect.Instance.Config.RecoveryEffect);
        }

        private static void CallRec(Player player, bool recInv, bool recAmmo, bool recEff)
        {
            if (recInv)
            {
                RestoreInventory(player);
            }

            if (recAmmo)
            {
                Ammo.RestoreAmmo(player);
            }

            if (recEff)
            {
                Effects.RestoreEffects(player);
            }
        }

        private static void CallSto(Player player, bool recInv, bool recAmmo, bool recEff)
        {
            if (recInv)
            {
                CloneInventory(player);
            }

            if (recAmmo)
            {
                Ammo.StoreAmmo(player);
            }

            if (recEff)
            {
                Effects.StoreEffects(player);
            }
        }

        public static void ClearPlayerData() => DisconnectedPlayers?.Clear();

        private static void CloneInventory(Player player)
        {
            PlayerData? PlayerData = GetPlayerData(player);
            if (PlayerData == null)
            {
                return;
            }
        
            foreach (Item item in player.Items.ToHashSet())
            {
                PlayerData.Inventory.Add(item.Clone());
            }
            
            /*foreach (Item item in player.Items.ToHashSet())
            {
                CustomItem customItem;
    
                if (CustomItem.TryGet(item, out customItem) && AutoReconnect.Instance.Config.CustomModuleSupport)
                {
                    PlayerData.CustomItems.Add(customItem);
                }
                else
                {
                    PlayerData.Inventory.Add(item.Clone());
                }
            }*/
        }

        public static void GetAcceptPlayers()
        {
            var AllPlayers = Player.List.ToList();
            foreach (var player in AllPlayers)
            {
                if (!player.DoNotTrack)
                {
                    if (!EventHandlers.AcceptPlayers.Contains(player.UserId))
                    {
                        EventHandlers.AcceptPlayers.Add(player.UserId);
                    }

                    ReviveBlock.BlockTime[player.UserId] = 0;
                }
            }
        }

        private static void GetAllPlayer()
        {
            if (AllPlayers.IsEmpty())
            {
                AllPlayers = Player.List.ToList();
            }
            else
            {
                if (!Player.List.ToList().Equals(AllPlayers))
                {
                    AllPlayers = Player.List.ToList();
                }
            }
        }

        public static PlayerData? GetPlayerData(Player player) => DisconnectedPlayers.ContainsKey(player.UserId)
            ? DisconnectedPlayers[player.UserId]
            : null;

        public static bool ResurrectPlayer(Player player, PlayerData playerData)
        {
            if (player == null && playerData == null) return false;
        
            player.Role.Set(playerData.Class, (SpawnReason)AutoReconnect.Instance.Config.SpawnReason, RoleSpawnFlags.None);

            /*if (playerData.CustomRole == null)
            {
                player.Role.Set(playerData.Class, (SpawnReason)AutoReconnect.Instance.Config.SpawnReason, RoleSpawnFlags.None);
            }
            else if (AutoReconnect.Instance.Config.CustomModuleSupport)
            {
                Pawn pawn = new Pawn(player.ReferenceHub);
                CustomRole.Spawn(pawn, playerData.CustomRole, true);
            }*/
            
            player.Position = playerData.Position;
            player.Health = playerData.Health;

            CallRec(player, AutoReconnect.Instance.Config.RecoveryInventory, AutoReconnect.Instance.Config.RecoveryAmmo, AutoReconnect.Instance.Config.RecoveryEffect);

            if (playerData.Class == RoleTypeId.Scp079)
            {
                player.Role.As<Scp079Role>().Experience = playerData.Exp;
                player.Role.As<Scp079Role>().Energy = playerData.Energy;
                player.Role.As<Scp079Role>().Level = playerData.Level;
            }

            DisconnectedPlayers?.Remove(player.UserId);
            return true;
        }

        public static void RemovePlayerData(Player player)
        {
            if (player == null && !DisconnectedPlayers.ContainsKey(player.UserId)) return;
            DisconnectedPlayers?.Remove(player.UserId);
        }

        private static void RestoreInventory(Player player)
        {
            PlayerData? PlayerData = GetPlayerData(player);
            if (PlayerData == null || PlayerData.Inventory.Count == 0)
            {
                return;
            }

            foreach (Item item in PlayerData.Inventory)
            {
                item.Give(player);
            }

            /*foreach (CustomItem item in PlayerData.CustomItems)
            {
                item.Give(player);
            }*/

            PlayerData.Inventory.Clear();
        }
    }
}
