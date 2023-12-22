using System.Collections.Generic;
using API.Other;
using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using PluginAPI.Core;
using ARRAPI = API.API;

namespace AutoReconnectRemastered
{
    public class EventHandlers
    {
        public static List<string> AcceptPlayers = new();

        public EventHandlers()
        {
            Exiled.Events.Handlers.Player.Verified += OnVerified;
            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
            Exiled.Events.Handlers.Player.Left += OnLeft;
            Exiled.Events.Handlers.Player.Dying += OnDying;
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundstarted;
            if (AutoReconnect.Instance.Config.SpawnRagdoll) return;

            Exiled.Events.Handlers.Player.SpawningRagdoll += OnSpawningRagdolls;
        }

        ~EventHandlers()
        {
            Exiled.Events.Handlers.Player.Left -= OnLeft;
            Exiled.Events.Handlers.Player.Dying -= OnDying;
            Exiled.Events.Handlers.Player.Spawned -= OnSpawned;
            Exiled.Events.Handlers.Player.Verified -= OnVerified;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundstarted;
            if (AutoReconnect.Instance.Config.SpawnRagdoll) return;

            Exiled.Events.Handlers.Player.SpawningRagdoll -= OnSpawningRagdolls;
        }

        public void OnWaitingForPlayers() => ARRAPI.ClearPlayerData();

        public void OnSpawningRagdolls(SpawningRagdollEventArgs ev) =>
            ev.IsAllowed = ARRAPI.DisconnectedPlayers.ContainsKey(ev.Player.UserId) ? false : true;

        public void OnRoundstarted()
        {
            ARRAPI.GetAcceptPlayers();
            Log.Debug("Player list initialized.");
        }

        public void OnLeft(LeftEventArgs ev)
        {
            if (AcceptPlayers.Contains(ev.Player.UserId))
            {
                if (ev.Player.Role.Type != RoleTypeId.Spectator && ev.Player.Role.Type != RoleTypeId.None)
                {
                    ARRAPI.AddPlayer(ev.Player);
                    if (AutoReconnect.Instance.Config.RandomSpec)
                    {
                        if (ev.Player.Role.Team == Team.SCPs)
                        {
                            ARRAPI.RandomSpec(ev.Player);
                            Log.Debug("Random Spectator method executed.");
                        }
                    }
                    ev.Player.ClearInventory();
                    Log.Debug($"Player {ev.Player.Nickname} data stored.");
                }
            }
        }

        public void OnSpawned(SpawnedEventArgs ev)
        {
            if (!AcceptPlayers.Contains(ev.Player.UserId))
            {
                if (ev.Player.DoNotTrack)
                {
                    Log.Debug($"Player {ev.Player.Nickname} has DNT on.");
                    ev.Player.Broadcast(AutoReconnect.Instance?.Config.JoinedBroadcast);
                }
            }
        }

        public void OnDying(DyingEventArgs ev)
        {
            if (ev.DamageHandler.Type == DamageType.Tesla || ev.DamageHandler.Type == DamageType.Marshmallow || ev.DamageHandler.Type == DamageType.Crushed || ev.DamageHandler.Type == DamageType.Warhead)
                ARRAPI.RemovePlayerData(ev.Player);
            else
            {
                if (ev.Attacker == null) return;
                ARRAPI.RemovePlayerData(ev.Player);
            }
        }

        public void OnVerified(VerifiedEventArgs ev)
        {
            PlayerData playerData = ARRAPI.GetPlayerData(ev.Player);
            if (playerData == null) return;
            
            ARRAPI.GetAcceptPlayers();

            if (AutoReconnect.Instance.Config.RespawnEnabled)
            {
                if (AutoReconnect.Instance.Config.RetentionTime == 0)
                {
                    if (Warhead.IsDetonated && ev.Player.Zone == ZoneType.Surface)
                    {
                        if (ARRAPI.ResurrectPlayer(ev.Player, playerData))
                        {
                            ev.Player.Broadcast(5, AutoReconnect.Instance?.Config.ReconnectText, Broadcast.BroadcastFlags.Normal, true);
                        }
                        ARRAPI.DisconnectedPlayers?.Remove(ev.Player.UserId);
                    }
                    else
                    {
                        if (ARRAPI.ResurrectPlayer(ev.Player, playerData))
                        {
                            ev.Player.Broadcast(5, AutoReconnect.Instance?.Config.ReconnectText, Broadcast.BroadcastFlags.Normal, true);
                        }
                        ARRAPI.DisconnectedPlayers?.Remove(ev.Player.UserId);
                    }
                }
                else
                {
                    if (!ARRAPI.IsReachedTimeLimit(ev.Player))
                    {
                        if (Warhead.IsDetonated && ev.Player.Zone == ZoneType.Surface)
                        {
                            if (ARRAPI.ResurrectPlayer(ev.Player, playerData))
                            {
                                ev.Player.Broadcast(5, AutoReconnect.Instance?.Config.ReconnectText, Broadcast.BroadcastFlags.Normal, true);
                            }
                            ARRAPI.DisconnectedPlayers?.Remove(ev.Player.UserId);
                        }
                        else
                        {
                            if (ARRAPI.ResurrectPlayer(ev.Player, playerData))
                            {
                                ev.Player.Broadcast(5, AutoReconnect.Instance?.Config.ReconnectText, Broadcast.BroadcastFlags.Normal, true);
                            }
                            ARRAPI.DisconnectedPlayers?.Remove(ev.Player.UserId);
                        }
                    }
                    else
                    {
                        ARRAPI.RemovePlayerData(ev.Player);
                    }
                }
            }
        }
    }
}