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
using CommandSystem;

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

        public static Dictionary<string, PlayerData> DisconnectedPlayers = new();
        public static void AddPlayer(Player player)
        {
            if (player == null && !player.IsAlive) return;

            PlayerData PlayerHandler = new PlayerData(player)
            {
                Ammo = new Dictionary<ItemType, ushort>(),
                Class = player.Role.Type,
                Health = player.Health,
                Inventory = player.Items.ToHashSet(),
                Inventory_Clone = new List<Item>(),
                Name = player.Nickname,
                Position = player.Position,
                Effects = new(),
                Player = player,
            };
            DisconnectedPlayers.Add(player.UserId, PlayerHandler);
            CloneInventory(player);
            StoreAmmo(player);
            StoreEffects(player);
        }

        public static void ClearPlayerData() => DisconnectedPlayers.Clear();

        public static PlayerData GetPlayerData(Player player) => DisconnectedPlayers.ContainsKey(player.UserId) ? DisconnectedPlayers[player.UserId] : null;

        public static void RemovePlayerData(Player player)
        {
            if (player == null && !DisconnectedPlayers.ContainsKey(player.UserId)) return;
            DisconnectedPlayers.Remove(player.UserId);
        }

        public static bool ResurrectPlayer(Player player, PlayerData playerData)
        {
            if (player == null && playerData == null) return false;

            player.Role.Set(playerData.Class, RoleSpawnFlags.None);
            player.Position = playerData.Position;
            player.Health = playerData.Health;

            if (AutoReconnect.Instance.Config.RecoveryAmmo) RestoreAmmo(player);
            if (AutoReconnect.Instance.Config.RecoveryInventory) RestoreInventory(player);
            if (AutoReconnect.Instance.Config.RecoveryEffect) RestoreEffects(player);
            DisconnectedPlayers.Remove(player.UserId);
            return true;
        }

        public static void CloneInventory(Player player)
        {
            PlayerData PlayerData = GetPlayerData(player);
            if (PlayerData == null) return;

            foreach (Item item in PlayerData.Inventory)
                PlayerData.Inventory_Clone.Add(item.Clone());
        }

        public static void RestoreInventory(Player player)
        {
            PlayerData PlayerData = GetPlayerData(player);
            if (PlayerData == null || PlayerData.Inventory.Count() == 0) return;

            foreach (Item item in PlayerData.Inventory_Clone)
            {
                item.ChangeItemOwner(PlayerData.Player, player);
                item.Give(player);
            }

            PlayerData.Inventory_Clone.Clear();
        }

        public static void StoreEffects(Player player)
        {
            PlayerData PlayerData = GetPlayerData(player);
            if (PlayerData == null || player.ActiveEffects.Count() == 0) return;

            foreach (StatusEffectBase effectBase in player.ActiveEffects)
                PlayerData.Effects.Add(new EffectList(effectBase.GetEffectType(), effectBase.Intensity,
                    effectBase.Duration));
        }

        public static void RestoreEffects(Player player)
        {
            PlayerData PlayerData = GetPlayerData(player);
            if (PlayerData == null || PlayerData.Effects.Count() == 0) return;

            foreach (var effectType in PlayerData.Effects)
            {
                player.EnableEffect(effectType.effectType, effectType.Intensity, effectType.Duration);
                PlayerData.Effects.Remove(effectType);
            }
        }

        public static void StoreAmmo(Player player)
        {
            PlayerData PlayerData = GetPlayerData(player);
            if (PlayerData == null || player.Ammo.Count == 0) return;

            foreach (ItemType ammoType in ammoTypes)
                if (player.Ammo.TryGetValue(ammoType, out ushort ammoAmount))
                    PlayerData.Ammo[ammoType] = ammoAmount;
        }

        public static void RestoreAmmo(Player player)
        {
            PlayerData PlayerData = GetPlayerData(player);
            if (PlayerData == null || PlayerData.Ammo.Count == 0) return;
            foreach (var ammoType in PlayerData.Ammo.Keys)
            {
                player.AddAmmo(ammoType.GetAmmoType(), PlayerData.Ammo[ammoType]);
            }
        }
    }
}
