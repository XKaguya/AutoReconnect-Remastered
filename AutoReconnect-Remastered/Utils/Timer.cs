using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using MEC;

namespace AutoReconnectRemastered
{
    public class Timer
    {
        private readonly AutoReconnect _instance;

        internal Timer(AutoReconnect instance) => this._instance = instance;

        public float DelayedTime = 5;
        
        public CoroutineHandle TimerHandle;
        
        public IEnumerator<float> ATimer()
        {
            for (;;)
            {
                yield return Timing.WaitForSeconds(DelayedTime);
                var AllPlayers = Player.List.Where(p => p.IsAlive).ToList();
                foreach (var player in AllPlayers)
                {
                    AutoReconnect.Instance.PlayerData.AddPlayer(player);
                }
                //AutoReconnect.Instance.PlayerData.DisplayPlayersInfo();
            }
        }

        public void RunTimer()
        {
            TimerHandle = Timing.RunCoroutine(ATimer());
            Log.Info("Timer started.");
        }

        public void KillTimer()
        {
            Timing.KillCoroutines(TimerHandle);
            Log.Info("Timer destroyered.");
        }
    }
}