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
                player.ClearInventory();
                DisconnectedPlayers.Add(player.UserId, player);
                Log.Info($"Player {player.Nickname} has joined the list.");
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
                Log.Info($"Player {player.Nickname}'s data has been restored.");
                if (player.Role.Side == Side.Scp)
                {
                    player.Broadcast(5, "你已重连，你会以数据存储时的状态重生。", Broadcast.BroadcastFlags.Normal, true);
                }
                else
                {
                    player.Broadcast(5, "你已重连，你会以数据存储时的状态重生。\n请记得低头捡起你的物品。", Broadcast.BroadcastFlags.Normal, true);
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
    }
}