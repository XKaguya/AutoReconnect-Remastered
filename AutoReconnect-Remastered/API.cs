using System;
using API.Other;
using AutoReconnectRemastered;
using CustomPlayerEffects;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features.Roles;
using Log = PluginAPI.Core.Log;
using Random = System.Random;

namespace API
{
    public static class API
    {
        private static HashSet<ItemType> ammoTypes = new()
        {
            ItemType.Ammo9x19,
            ItemType.Ammo556x45,
            ItemType.Ammo12gauge,
            ItemType.Ammo762x39,
            ItemType.Ammo44cal
        };

        public static Dictionary<string, PlayerData>? DisconnectedPlayers = new();

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
                }
            }
        }

        public static bool IsReachedTimeLimit(Player player)
        {
            PlayerData PlayerData = GetPlayerData(player);

            var time = DateTime.Now;
            TimeSpan RetentionTimeSpan = TimeSpan.FromSeconds(AutoReconnect.Instance.Config.RetentionTime);
            if ((time - PlayerData.Time) > RetentionTimeSpan)
            {
                return true;
            }

            return false;
        }

        public static void RandomSpec(Player player)
        {
            PlayerData PlayerData = GetPlayerData(player);
            
            var players = Player.List.Where(p => p.Role.Type == RoleTypeId.Spectator);
            Player[] playerArray = players.ToArray();
            
            Random random = new Random();
            int randomNumber = random.Next(0, playerArray.Length);
            Player selectedPlayer = playerArray[randomNumber];
            
            Log.Debug($"Choosen player: {selectedPlayer}");

            ResurrectPlayer(selectedPlayer, PlayerData);
            
            var message = string.Format(AutoReconnect.Instance.Config.DisconnectedMessage, PlayerData.Class, player.Nickname);

            if (AutoReconnect.Instance.Config.DisconnectedMessageType == 1)
            {
                foreach (var player1 in Player.List)
                {
                    player1.ShowHint(message);
                }
            }

            if (AutoReconnect.Instance.Config.DisconnectedMessageType == 2)
            {
                var alivePlayers = Player.List.Where(player => player.Role.IsAlive);
                foreach (var player1 in alivePlayers)
                {
                    player1.ShowHint(message);
                }

                var deadPlayers = Player.List.Where(player => !player.Role.IsAlive);
                foreach (var player2 in deadPlayers)
                {
                    player2.Broadcast(10, message, Broadcast.BroadcastFlags.Normal, true);
                }
            }
        }
        
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
            
            DisconnectedPlayers.Add(player.UserId, PlayerHandler);
            
            if (AutoReconnect.Instance.Config.RecoveryInventory) CloneInventory(player);
            if (AutoReconnect.Instance.Config.RecoveryAmmo) StoreAmmo(player);
            if (AutoReconnect.Instance.Config.RecoveryEffect) StoreEffects(player);
        }

        public static void ClearPlayerData() => DisconnectedPlayers?.Clear();

        public static PlayerData GetPlayerData(Player player) => DisconnectedPlayers.ContainsKey(player.UserId) ? DisconnectedPlayers[player.UserId] : null;

        public static void RemovePlayerData(Player player)
        {
            if (player == null && !DisconnectedPlayers.ContainsKey(player.UserId)) return;
            DisconnectedPlayers?.Remove(player.UserId);
        }

        public static bool ResurrectPlayer(Player player, PlayerData playerData)
        {
            if (player == null && playerData == null) return false;

            player.Role.Set(playerData.Class, (SpawnReason)AutoReconnect.Instance.Config.SpawnReason, RoleSpawnFlags.None);
            player.Position = playerData.Position;
            player.Health = playerData.Health;

            if (AutoReconnect.Instance.Config.RecoveryAmmo)
            {
                RestoreAmmo(player);
            }

            if (AutoReconnect.Instance.Config.RecoveryInventory)
            {
                RestoreInventory(player);
            }

            if (AutoReconnect.Instance.Config.RecoveryEffect)
            {
                RestoreEffects(player);
            }

            if (playerData.Class == RoleTypeId.Scp079)
            {
                player.Role.As<Scp079Role>().Experience = playerData.Exp;
                player.Role.As<Scp079Role>().Energy = playerData.Energy;
                player.Role.As<Scp079Role>().Level = playerData.Level;
            }
            
            DisconnectedPlayers?.Remove(player.UserId);
            return true;
        }

        public static void CloneInventory(Player player)
        {
            PlayerData PlayerData = GetPlayerData(player);
            if (PlayerData == null)
            {
                return;
            }

            foreach (Item item in player.Items.ToHashSet())
            {
                PlayerData.Inventory.Add(item.Clone());
            }
        }
        
        public static void RestoreInventory(Player player)
        {
            PlayerData PlayerData = GetPlayerData(player);
            if (PlayerData == null || PlayerData.Inventory.Count == 0)
            {
                return;
            }

            foreach (Item item in PlayerData.Inventory)
            {
                item.Give(player);
            }
            
            PlayerData.Inventory.Clear();
        }
        
        public static void StoreEffects(Player player)
        {
            PlayerData PlayerData = GetPlayerData(player);
            if (PlayerData == null || player.ActiveEffects.Count() == 0)
            {
                return;
            }

            foreach (StatusEffectBase effectBase in player.ActiveEffects)
            {
                PlayerData.Effects.Add(new EffectList(effectBase.GetEffectType(), effectBase.Intensity, effectBase.Duration));
            }
        }

        public static void RestoreEffects(Player player)
        {
            PlayerData PlayerData = GetPlayerData(player);
            if (PlayerData == null || PlayerData.Effects.Count == 0)
            {
                return;
            }

            foreach (var effectType in PlayerData.Effects)
            {
                player.EnableEffect(effectType.effectType, effectType.Intensity, effectType.Duration);
            }
            PlayerData.Effects.Clear();
        }

        public static void StoreAmmo(Player player)
        {
            PlayerData PlayerData = GetPlayerData(player);
            if (PlayerData == null || player.Ammo.Count == 0)
            {
                return;
            }

            foreach (ItemType ammoType in ammoTypes)
            {
                if (player.Ammo.TryGetValue(ammoType, out ushort ammoAmount))
                {
                    PlayerData.Ammo[ammoType] = ammoAmount;
                }
            }
        }

        public static void RestoreAmmo(Player player)
        {
            PlayerData PlayerData = GetPlayerData(player);
            if (PlayerData == null || PlayerData.Ammo.Count == 0)
            {
                return;
            }
            foreach (var ammoType in PlayerData.Ammo.Keys)
            {
                player.AddAmmo(ammoType.GetAmmoType(), PlayerData.Ammo[ammoType]);
            }
            PlayerData.Ammo.Clear();
        }
    }
}
