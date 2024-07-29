#pragma warning disable CS1591

using System;
using System.Collections.Generic;
using System.Linq;
using Event;
using Exiled.API.Features;
using MEC;
using PlayerInfo;
using PlayerRoles;
using Plugin;

namespace API
{
    public class Features
    {
        private static CoroutineHandle BlockTimer { get; set; }

        public static void BlockRevive(Player player)
        {
            if (GetPlayerBlockTime(player) != -1)
            {
                SetBlockTime(player, PluginBase.Instance.Config.ReviveBlockTime);
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
            PlayerData? playerData = PlayerApi.GetPlayerData(player);

            var time = DateTime.Now;
            TimeSpan retentionTimeSpan = TimeSpan.FromSeconds(PluginBase.Instance!.Config.RetentionTime);
            if ((time - playerData!.Time) > retentionTimeSpan)
            {
                return true;
            }

            return false;
        }

        public static void RandomSpec(Player player)
        {
            PlayerData? playerData = PlayerApi.GetPlayerData(player);

            if (playerData == null)
            {
                return;
            }

            var players = Player.List.Where(p => p.Role.Type == RoleTypeId.Spectator);
            Player[] playerArray = players.ToArray();

            Random random = new Random();
            int randomNumber = random.Next(0, playerArray.Length);
            Player selectedPlayer = playerArray[randomNumber];

            Log.Debug($"Choosen player: {selectedPlayer}");

            PlayerApi.ResurrectPlayer(selectedPlayer, playerData);

            var message = string.Format(PluginBase.Instance!.Config.DisconnectedMessage, playerData.Class, player.Nickname);

            if (PluginBase.Instance.Config.DisconnectedMessageType == 1)
            {
                foreach (var player1 in Player.List)
                {
                    player1.ShowHint(message);
                }
            }

            if (PluginBase.Instance.Config.DisconnectedMessageType == 2)
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

            if (player.Role == RoleTypeId.Spectator && PluginBase.Instance!.Config.ReplacePlayer)
            {
                PlayerApi.ResurrectPlayer(player, pd!);
            }

            var message = string.Format(PluginBase.Instance!.Config.DisconnectedMessage, pd!.Class, player.Nickname);

            if (PluginBase.Instance.Config.DisconnectedMessageType == 1)
            {
                foreach (var player1 in Player.List)
                {
                    player1.ShowHint(message);
                }
            }

            if (PluginBase.Instance.Config.DisconnectedMessageType == 2)
            {
                var players = Player.List;

                var alivePlayers = players.Where(PlayerApi.IsPlayerAlive);
                var deadPlayers = players.Where(PlayerApi.IsPlayerDead);

                foreach (var alivePlayer in alivePlayers)
                {
                    alivePlayer.ShowHint(message);
                }

                foreach (var deadPlayer in deadPlayers)
                {
                    deadPlayer.Broadcast(10, message, Broadcast.BroadcastFlags.Normal, true);
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
                    foreach (Player player in Player.List)
                    {
                        if (EventHandlers.AcceptPlayers.Contains(player.UserId))
                        {
                            if (GetPlayerBlockTime(player) != 0)
                            {
                                Log.Debug($"Player {player.Nickname} with block time: {GetPlayerBlockTime(player)}");
                                DecreaseBlockTime(player);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"An exception occurred in ReviveCoroutine: {ex.Message} {ex.StackTrace}");
                }

                yield return Timing.WaitForSeconds(PluginBase.Instance.Config.ReviveBlockDelay);
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
