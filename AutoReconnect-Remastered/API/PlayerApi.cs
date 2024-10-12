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

        public static void AddPlayer(Player? player)
        {
            if (player == null)
            {
                return;
            }

            PlayerData playerHandler = new PlayerData(player)
            {
                ArtificialHealth = player.ArtificialHealth,
                Ammo = new Dictionary<ItemType, ushort>(),
                Class = player.Role.Type,
                CustomInfo = player.CustomInfo,
                CustomName = player.CustomName,
                Effects = new(),
                Health = player.Health,
                HumeShield = player.HumeShield,
                Inventory = new HashSet<Item>(),
                IsUsingStamina = player.IsUsingStamina,
                MaxArtificialHealth = player.MaxArtificialHealth,
                MaxHealth = player.MaxHealth,
                Name = player.Nickname,
                Player = player,
                Position = player.Position,
                Scale = player.Scale,
                Stamina = player.Stamina,
                Time = DateTime.Now,
            };

            if (player.Role.Type == RoleTypeId.Scp079)
            {
                playerHandler.Exp = player.Role.As<Scp079Role>().Experience;
                playerHandler.Level = player.Role.As<Scp079Role>().Level;
                playerHandler.Energy = player.Role.As<Scp079Role>().Energy;
            }

            var customRoles = player.GetCustomRoles();
            if (customRoles.Any())
            {
                foreach (CustomRole customRole in CustomRole.Registered)
                {
                    if (customRoles.Contains(customRole))
                    {
                        // Only accept one custom role. Won't consider as a bug if multiple.
                        playerHandler.CustomRole = customRole;
                        playerHandler.CustomRoleType = customRole.GetType();
                        break;
                    }
                }
            }
            else
            {
                playerHandler.CustomRole = null;
            }

            DisconnectedPlayers.Add(player.UserId, playerHandler);

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
            PlayerData? playerData = GetPlayerData(player);
            if (playerData == null)
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
                        playerData.CustomItems.Add(customItem.Id);
                    }
                    else
                    {
                        playerData.Inventory.Add(item.Clone());
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

        public static PlayerData? GetPlayerData(Player player)
        {
            return DisconnectedPlayers.TryGetValue(player.UserId, out var playerData) ? playerData : null;
        }

        public static bool ResurrectPlayer(Player? player, PlayerData? playerData)
        {
            if (player == null || playerData == null)
            {
                return false;
            }

            player.Role.Set(playerData.Class, (SpawnReason)PluginBase.Instance.Config.SpawnReason, RoleSpawnFlags.None);

            if (playerData.CustomRoleType != null && PluginBase.Instance.Config.CustomModuleSupport)
            {
                CustomRole.Get(playerData.CustomRoleType)?.AddRole(player);
            }
            
            if (playerData.ArtificialHealth.HasValue)
            {
                player.ArtificialHealth = playerData.ArtificialHealth.Value;
            }
            
            if (playerData.MaxArtificialHealth.HasValue)
            {
                player.MaxArtificialHealth = playerData.MaxArtificialHealth.Value;
            }

            player.Position = playerData.Position;
            player.MaxHealth = playerData.MaxHealth;
            player.Health = playerData.Health;
            player.Scale = playerData.Scale;
            player.Stamina = playerData.Stamina;
            player.IsUsingStamina = playerData.IsUsingStamina;

            if (playerData.HumeShield.HasValue)
            {
                player.HumeShield = playerData.HumeShield.Value;
            }
            
            if (playerData.CustomInfo != null)
            {
                player.CustomInfo = playerData.CustomInfo;
            }
            
            if (playerData.CustomName != null)
            {
                player.CustomName = playerData.CustomName;
            }

            CallRec(
                player, 
                PluginBase.Instance.Config.RecoveryInventory, 
                PluginBase.Instance.Config.RecoveryAmmo, 
                PluginBase.Instance.Config.RecoveryEffect
            );

            if (playerData.Class == RoleTypeId.Scp079)
            {
                var scp079Role = player.Role.As<Scp079Role>();
                scp079Role.Experience = playerData.Exp;
                scp079Role.Energy = playerData.Energy;
                scp079Role.Level = playerData.Level;
            }

            DisconnectedPlayers?.Remove(player.UserId);
            return true;
        }

        public static void RemovePlayerData(Player? player)
        {
            if (player != null && DisconnectedPlayers.ContainsKey(player.UserId))
            {
                DisconnectedPlayers.Remove(player.UserId);
            }
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
    }
}
