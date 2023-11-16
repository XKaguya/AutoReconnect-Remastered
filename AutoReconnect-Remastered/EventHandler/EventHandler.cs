using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables;
using MEC;
using PlayerRoles;
using PlayerRoles.Ragdolls;

namespace AutoReconnectRemastered
{
    public class EventHandlers
    {
        private readonly AutoReconnect _instance;

        internal EventHandlers(AutoReconnect instance) => this._instance = instance;
        
        public Dictionary<string, Player> DisconnectedPlayers = new();
        
        public void OnDisconnected(LeftEventArgs ev)
        {
            Player player = ev.Player;
            
            if (player.Role.Type != RoleTypeId.Spectator && player.Role.Type != RoleTypeId.None)
            {
                try
                {
                    player.ClearInventory();
                    DisconnectedPlayers.Add(player.UserId, player);
                    Log.Info($"Player {player.Nickname} has joined the list.");
                }
                catch (Exception e)
                {
                    var text = $"Event OnDisconnected raising error. Reason: {e}";
                    Log.Error(text);
                    throw;
                }
            }
        }

        public void OnDying(DyingEventArgs ev)
        {
            Player player = ev.Player;
            if (ev.DamageHandler.Type == DamageType.Tesla || ev.DamageHandler.Type == DamageType.Marshmallow ||
                ev.DamageHandler.Type == DamageType.Crushed || ev.DamageHandler.Type == DamageType.Warhead)
            {
                Log.Info($"Dying reason: {ev.DamageHandler.Type}");
                AutoReconnect.Instance.PlayerData.RemovePlayerData(player);
            }
            else
            {
                if (ev.Attacker != null)
                {
                    AutoReconnect.Instance.PlayerData.RemovePlayerData(player);
                }
            }
        }
        
        public void OnVerified(VerifiedEventArgs ev)
        {
            Player player = ev.Player;
            
            DisconnectedPlayers.Remove(player.UserId);
            PlayerHandlers playerData = AutoReconnect.Instance.PlayerData.GetPlayerData(player);
            if (playerData != null)
            {
                AutoReconnect.Instance.PlayerData.ResurrectPlayer(player, playerData);
                {
                    if (AutoReconnect.Instance.Config.Language == "zh_CN")
                    {
                        player.Broadcast(5, "你已重连，你会以数据存储时的状态重生。", Broadcast.BroadcastFlags.Normal, true);
                    }
                    else
                    {
                        player.Broadcast(5, "You have been reconnected. You will be respawn to stored status.", Broadcast.BroadcastFlags.Normal, true);
                    }
                }
            }
        }

        public void OnWaitingForPlayers()
        {
            DisconnectedPlayers.Clear();
            AutoReconnect.Instance.PlayerData.ClearPlayerData();
            Log.Info("Data Cleared.");
        }

        public void OnRoundstarted()
        {
            AutoReconnect.Instance.Timer.RunTimer();
        }

        private bool isAllowed = false;

        public void OnSpawningRagdolls(SpawningRagdollEventArgs ev)
        {
            Player player = ev.Player;
            var disconnectedPlayers = AutoReconnect.Instance.EventHandlers.DisconnectedPlayers;
            RagdollData Info = ev.Info;

            if (AutoReconnect.Instance.Config.SpawnRagdoll)
            {
                if (disconnectedPlayers.ContainsKey(player.UserId))
                {
                    ev.IsAllowed = isAllowed;
                    Log.Info($"Spawn ragdoll event rejected.");
                }
                else
                {
                    isAllowed = true;
                    ev.IsAllowed = isAllowed;
                }
            }
        }
    }
}