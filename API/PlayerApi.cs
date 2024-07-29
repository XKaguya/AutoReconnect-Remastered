#pragma warning disable CS1591

using System;
using System.Collections.Generic;
using System.Linq;
using Event;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using Exiled.CustomItems.API.Features;
using Exiled.CustomRoles.API;
using Exiled.CustomRoles.API.Features;
using PlayerInfo;
using PlayerRoles;
using Plugin;

namespace API
{
    public class PlayerApi
    {
        public static Dictionary<string, PlayerData> DisconnectedPlayers = new();

        public static void AddPlayer(Player player)
        {
            if (player == null && !player.IsAlive) return;

            PlayerData PlayerHandler = new PlayerData(player)
            {
                Ammo = new Dictionary<ItemType, ushort>(),
                Class = player.Role.Type,
                MaxHealth = player.MaxHealth,
                Health = player.Health,
                Inventory = new HashSet<Item>(),
                Name = player.Nickname,
                Position = player.Position,
                Effects = new(),
                Player = player,
                Time = DateTime.Now,
                Stamina = player.Stamina,
                Scale = player.Scale,
            };

            if (player.Role.Type == RoleTypeId.Scp079)
            {
                PlayerHandler.Exp = player.Role.As<Scp079Role>().Experience;
                PlayerHandler.Level = player.Role.As<Scp079Role>().Level;
                PlayerHandler.Energy = player.Role.As<Scp079Role>().Energy;
            }

            var customRoles = player.GetCustomRoles();
            if (customRoles.Any())
            {
                foreach (CustomRole customRole in CustomRole.Registered)
                {
                    if (customRoles.Contains(customRole))
                    {
                        // Only accept one custom role. Won't consider as a bug if multiple.
                        PlayerHandler.CustomRole = customRole;
                        PlayerHandler.CustomRoleType = customRole.GetType();
                        break;
                    }
                }
            }
            else
            {
                PlayerHandler.CustomRole = null;
            }

            DisconnectedPlayers.Add(player.UserId, PlayerHandler);

            CallSto(player, PluginBase.Instance.Config.RecoveryInventory, PluginBase.Instance.Config.RecoveryAmmo, PluginBase.Instance.Config.RecoveryEffect);
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
                try
                {
                    if (PluginBase.Instance.Config.CustomModuleSupport && CustomItem.TryGet(item, out CustomItem customItem))
                    {
                        Log.Debug($"CustomItem {customItem.Name} added.");
                        PlayerData.CustomItems.Add(customItem.Id);
                    }
                    else
                    {
                        PlayerData.Inventory.Add(item.Clone());
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e.Message + e.StackTrace);
                }
            }
        }

        public static void GetAcceptPlayers()
        {
            var players = Player.List.ToList();
            foreach (var player in players)
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

        public static PlayerData? GetPlayerData(Player player) => DisconnectedPlayers.ContainsKey(player.UserId)
            ? DisconnectedPlayers[player.UserId]
            : null;

        public static bool ResurrectPlayer(Player player, PlayerData playerData)
        {
            if (player == null && playerData == null) return false;
        
            player.Role.Set(playerData.Class, (SpawnReason)PluginBase.Instance.Config.SpawnReason, RoleSpawnFlags.None);

            if (playerData.CustomRole == null)
            {
                player.Role.Set(playerData.Class, (SpawnReason)PluginBase.Instance.Config.SpawnReason, RoleSpawnFlags.None);
            }
            else if (playerData.CustomRole != null && playerData.CustomRoleType != null && PluginBase.Instance.Config.CustomModuleSupport)
            {
                CustomRole.Get(playerData.CustomRoleType)?.AddRole(player);
            }
            
            player.Position = playerData.Position;
            player.MaxHealth = playerData.MaxHealth;
            player.Health = playerData.Health;
            player.Scale = playerData.Scale;
            player.Stamina = playerData.Stamina;

            CallRec(player, PluginBase.Instance.Config.RecoveryInventory, PluginBase.Instance.Config.RecoveryAmmo, PluginBase.Instance.Config.RecoveryEffect);

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
            if (PlayerData == null || PlayerData.Inventory.Count == 0 && PlayerData.CustomItems.Count == 0)
            {
                return;
            }

            foreach (Item item in PlayerData.Inventory)
            {
                item.Give(player);
            }

            foreach (uint customItem in PlayerData.CustomItems)
            {
                Log.Debug($"Trying to give player custom item id: {customItem}");
                CustomItem.TryGive(player, customItem);
            }

            PlayerData.Inventory.Clear();
            PlayerData.CustomItems.Clear();
        }
        
        public static bool IsPlayerAlive(Player player)
        {
            return player.Role.IsAlive;
        }

        public static bool IsPlayerDead(Player player)
        {
            return !player.Role.IsAlive;
        }
    }
}
