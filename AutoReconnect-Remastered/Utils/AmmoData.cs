using System;
using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;

namespace AutoReconnectRemastered
{
    public class AmmoData
    {
        private readonly AutoReconnect _instance;

        internal AmmoData(AutoReconnect instance) => this._instance = instance;
        
        List<ItemType> ammoTypes = new List<ItemType>
        {
            ItemType.Ammo9x19,
            ItemType.Ammo556x45,
            ItemType.Ammo12gauge,
            ItemType.Ammo762x39,
            ItemType.Ammo44cal
        };

        public void StoreAmmo(Player player)
        {
            PlayerHandlers PlayerData = AutoReconnect.Instance.PlayerData.GetPlayerData(player);

            foreach (ItemType ammoType in ammoTypes)
            {
                if (player.Ammo.TryGetValue(ammoType, out ushort ammoAmount))
                {
                    PlayerData.Ammo[ammoType] = ammoAmount;
                }
                else
                {
                    continue;
                }
            }
        }

        public void RestoreAmmo(Player player)
        {
            PlayerHandlers PlayerData = AutoReconnect.Instance.PlayerData.GetPlayerData(player);

            try
            {
                CloneAmmo(player);
                foreach (var ammoType in PlayerData.Ammo_Clone.Keys)
                {
                    ushort ammoAmount = PlayerData.Ammo_Clone[ammoType];
                    player.AddAmmo(ammoType.GetAmmoType(), ammoAmount);
                }
                Log.Info($"Players {player.Nickname}'s ammo restored successfully.");
            }
            catch (Exception e)
            {
                var text = $"Restore Ammo failed. Reason: {e}";
                Log.Error(text);
                throw;
            }
        }
        
        public void CloneAmmo(Player player)
        {
            PlayerHandlers PlayerData = AutoReconnect.Instance.PlayerData.GetPlayerData(player);

            try
            {
                PlayerData.Ammo_Clone.Clear();
                foreach (var ammoType in PlayerData.Ammo.Keys)
                {
                    ushort ammoAmount = PlayerData.Ammo[ammoType];
                    PlayerData.Ammo_Clone.Add(ammoType, ammoAmount);
                }

                Log.Info($"Cloned {player.Nickname}'s ammo successfully.");
            }
            catch (Exception e)
            {
                var text = $"Clone failed. Reason: {e}";
                Log.Error(text);
                throw;
            }
        }
    }
}

