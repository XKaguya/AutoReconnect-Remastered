using System.Linq;
using CustomPlayerEffects;
using Exiled.API.Extensions;
using Exiled.API.Features;

namespace AutoReconnect_Remastered.API
{
    public class Effects
    {
        public static void StoreEffects(Player player)
        {
            PlayerData? PlayerData = PlayerApi.GetPlayerData(player);
            if (PlayerData == null || player.ActiveEffects.Any())
            {
                return;
            }

            foreach (StatusEffectBase effectBase in player.ActiveEffects)
            {
                PlayerData.Effects.Add(new EffectList(effectBase.GetEffectType(), effectBase.Intensity,
                    effectBase.Duration));
            }
        }

        public static void RestoreEffects(Player player)
        {
            PlayerData? PlayerData = PlayerApi.GetPlayerData(player);
            if (PlayerData == null || PlayerData.Effects.Count == 0)
            {
                return;
            }

            foreach (var effectType in PlayerData.Effects)
            {
                player.EnableEffect(effectType.EffectType, effectType.Intensity, effectType.Duration);
            }

            PlayerData.Effects.Clear();
        }
    }
}