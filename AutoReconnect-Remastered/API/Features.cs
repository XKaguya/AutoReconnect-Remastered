using System;
using System.Collections.Generic;
using System.Linq;
using AutoReconnectRemastered;
using Exiled.API.Features;
using MEC;
using PlayerRoles;

namespace AutoReconnect_Remastered.API
{
    public class Features
    {
        private static CoroutineHandle BlockTimer { get; set; }

        public static void BlockRevive(Player player)
        {
            if (GetPlayerBlockTime(player) != -1)
            {
                SetBlockTime(player, AutoReconnect.Instance.Config.ReviveBlockTime);
            }
        }

        public static void ClearBlockTime()
        {
            ReviveBlock.BlockTime.Clear();
        }

        private static void DecreaseBlockTime(Player player)
        {
            if (GetPlayerBlockTime(player) != 0)
            {
                ReviveBlock.BlockTime[player.UserId] -= 1;
            }
        }

        public static bool IsReachedTimeLimit(Player player)
        {
            PlayerData? PlayerData = PlayerApi.GetPlayerData(player);

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
            PlayerData? PlayerData = PlayerApi.GetPlayerData(player);

            if (PlayerData == null)
            {
                return;
            }

            var players = Player.List.Where(p => p.Role.Type == RoleTypeId.Spectator);
            Player[] playerArray = players.ToArray();

            Random random = new Random();
            int randomNumber = random.Next(0, playerArray.Length);
            Player selectedPlayer = playerArray[randomNumber];

            Log.Debug($"Choosen player: {selectedPlayer}");

            PlayerApi.ResurrectPlayer(selectedPlayer, PlayerData);

            var message = string.Format(AutoReconnect.Instance.Config.DisconnectedMessage, PlayerData.Class,
                player.Nickname);

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

        public static bool ReplacePlayer(Player player)
        {
            PlayerData? pd = null;
            foreach (var pair in PlayerApi.DisconnectedPlayers)
            {
                string key = pair.Key;
                PlayerData value = pair.Value;

                if (value != null && value.Player.IsScp)
                {
                    pd = value;
                    break;
                }
            }

            if (player.Role == RoleTypeId.Spectator && AutoReconnect.Instance.Config.ReplacePlayer)
            {
                PlayerApi.ResurrectPlayer(player, pd);
            }

            var message = string.Format(AutoReconnect.Instance.Config.DisconnectedMessage, pd.Class, player.Nickname);

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

            return true;
        }

        public static void StartCoroutine()
        {
            BlockTimer = Timing.RunCoroutine(ReviveCoroutine());
        }

        public static void StopCoroutine()
        {
            Timing.KillCoroutines(BlockTimer);
        }

        private static IEnumerator<float> ReviveCoroutine()
        {
            while (true)
            {
                try
                {
                    if (PlayerApi.AllPlayers != null)
                    {
                        foreach (Player player in PlayerApi.AllPlayers)
                        {
                            if (EventHandlers.AcceptPlayers.Contains(player.UserId))
                            {
                                if (GetPlayerBlockTime(player) != 0)
                                {
                                    DecreaseBlockTime(player);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"An exception occurred in ReviveCoroutine: {ex.Message} {ex.StackTrace}");
                }

                yield return Timing.WaitForSeconds(AutoReconnect.Instance.Config.ReviveBlockDelay);
            }
        }

        private static void SetBlockTime(Player player, int value)
        {
            ReviveBlock.BlockTime[player.UserId] = value;
        }

        public static int GetPlayerBlockTime(Player player)
        {
            return ReviveBlock.BlockTime.TryGetValue(player.UserId, out int time) ? time : 0;
        }
    }
}
