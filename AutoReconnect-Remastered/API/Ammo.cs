using System.Collections.Generic;
using Exiled.API.Extensions;
#pragma warning disable CS1591

using PlayerInfo;

namespace API
{
    public class Ammo
    {
        private static HashSet<ItemType> _ammoTypes = new()
        {
            ItemType.Ammo9x19,
            ItemType.Ammo556x45,
            ItemType.Ammo12gauge,
            ItemType.Ammo762x39,
            ItemType.Ammo44cal
        };
        
        public static void RestoreAmmo(Exiled.API.Features.Player player)
        {
            PlayerData? playerData = PlayerApi.GetPlayerData(player);
            if (playerData == null || playerData.Ammo.Count == 0)
            {
                return;
            }
    
            foreach (var ammoType in playerData.Ammo.Keys)
            {
                player.AddAmmo(ammoType.GetAmmoType(), playerData.Ammo[ammoType]);
            }
    
            playerData.Ammo.Clear();
        }
        
        public static void StoreAmmo(Exiled.API.Features.Player player)
        {
            PlayerData? playerData = PlayerApi.GetPlayerData(player);
            if (playerData == null || player.Ammo.Count == 0)
            {
                return;
            }
    
            foreach (ItemType ammoType in _ammoTypes)
            {
                if (player.Ammo.TryGetValue(ammoType, out ushort ammoAmount))
                {
                    playerData.Ammo[ammoType] = ammoAmount;
                }
            }
        }
    }
}