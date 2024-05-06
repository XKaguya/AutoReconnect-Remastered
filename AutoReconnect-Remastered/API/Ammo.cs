using System.Collections.Generic;
using Exiled.API.Extensions;

namespace AutoReconnect_Remastered.API
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
            PlayerData? PlayerData = PlayerApi.GetPlayerData(player);
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
        
        public static void StoreAmmo(Exiled.API.Features.Player player)
        {
            PlayerData? PlayerData = PlayerApi.GetPlayerData(player);
            if (PlayerData == null || player.Ammo.Count == 0)
            {
                return;
            }
    
            foreach (ItemType ammoType in _ammoTypes)
            {
                if (player.Ammo.TryGetValue(ammoType, out ushort ammoAmount))
                {
                    PlayerData.Ammo[ammoType] = ammoAmount;
                }
            }
        }
    }
}