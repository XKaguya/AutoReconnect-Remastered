#pragma warning disable CS1591

using System.Linq;
using CustomPlayerEffects;
using Exiled.API.Extensions;
using Exiled.API.Features;
using PlayerInfo;

namespace API
{
    public class Effects
    {
        public static void StoreEffects(Player player)
        {
            PlayerData? playerData = PlayerApi.GetPlayerData(player);
            if (playerData == null || player.ActiveEffects.Any())
            {
                return;
            }

            foreach (StatusEffectBase effectBase in player.ActiveEffects)
            {
                playerData.Effects.Add(new EffectList(effectBase.GetEffectType(), effectBase.Intensity, effectBase.Duration));
            }
        }

        public static void RestoreEffects(Player player)
        {
            PlayerData? playerData = PlayerApi.GetPlayerData(player);
            if (playerData == null || playerData.Effects.Count == 0)
            {
                return;
            }

            foreach (EffectList effect in playerData.Effects)
            {
                player.EnableEffect(effect.EffectType, effect.Intensity, effect.Duration);
                Log.Debug($"Recovered {effect.EffectType} {effect.Duration} {effect.Intensity}.");
            }

            playerData.Effects.Clear();
        }
    }
}