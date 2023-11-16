using System;
using System.Linq;
using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;

namespace AutoReconnectRemastered
{
    public class EffectData
    {
        private readonly AutoReconnect _instance;

        internal EffectData(AutoReconnect instance) => this._instance = instance;

        public void StoreEffects(Player player)
        {
            PlayerHandlers PlayerData = AutoReconnect.Instance.PlayerData.GetPlayerData(player);

            try
            {
                foreach (StatusEffectBase effectBase in PlayerData.Effects)
                {
                    EffectType Effect = effectBase.GetEffectType();
                    byte Intensity = effectBase.Intensity;
                    float Duration = effectBase.Duration;
                
                    PlayerData.Effects_Repertory[Effect] = (Intensity, Duration);
                }
            }
            catch (Exception e)
            {
                var text = $"Store effects. Reason: {e}";
                Log.Error(text);
                throw;
            }
        }

        public void RestoreEffects(Player player)
        {
            PlayerHandlers PlayerData = AutoReconnect.Instance.PlayerData.GetPlayerData(player);

            try
            {
                if (PlayerData.Effects_Repertory.Count != 0)
                {
                    foreach (EffectType effectType in PlayerData.Effects_Repertory.Keys)
                    {
                        byte intensity;
                        float duration;

                        if (PlayerData.Effects_Repertory.TryGetValue(effectType, out var tuple))
                        {
                            intensity = tuple.Item1;
                            duration = tuple.Item2;
                        

                            player.EnableEffect(effectType, intensity, duration);
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                var text = $"Restore effects. Reason: {e}";
                Log.Error(text);
                throw;
            }
        }
    }
}