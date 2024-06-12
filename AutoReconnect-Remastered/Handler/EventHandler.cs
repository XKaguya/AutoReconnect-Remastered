#pragma warning disable CS1591

using System.Collections.Generic;
using API;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp096;
using PlayerInfo;
// using Exiled.Events.EventArgs.Scp173;
using PlayerRoles;
using Plugin;
using Warhead = Exiled.API.Features.Warhead;

namespace Event
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
            Exiled.Events.Handlers.Player.Hurt += OnHurt;
            Exiled.Events.Handlers.Scp096.AddingTarget += OnAddingTarget;
            /*Exiled.Events.Handlers.Scp173.AddingObserver += OnAddingObserver;*/
            
            if (PluginBase.Instance!.Config.SpawnRagdoll)
            {
                return;
            }

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
            Exiled.Events.Handlers.Player.Hurt -= OnHurt;
            Exiled.Events.Handlers.Scp096.AddingTarget -= OnAddingTarget;
            /*Exiled.Events.Handlers.Scp173.AddingObserver -= OnAddingObserver;*/
            
            if (PluginBase.Instance!.Config.SpawnRagdoll)
            {
                return;
            }

            Exiled.Events.Handlers.Player.SpawningRagdoll -= OnSpawningRagdolls;
        }

        private void OnWaitingForPlayers()
        {
            PlayerApi.ClearPlayerData();
            Features.StopCoroutine();
            Features.ClearBlockTime();
        }

        private void OnSpawningRagdolls(SpawningRagdollEventArgs ev) => ev.IsAllowed = PlayerApi.DisconnectedPlayers.ContainsKey(ev.Player.UserId) ? false : true;

        private void OnRoundstarted()
        {
            PlayerApi.GetAcceptPlayers();
            Log.Debug("Player list initialized.");
            
            if (PluginBase.Instance!.Config.ReviveBlock)
            {
                Features.StartCoroutine();
            }
        }

        private void OnLeft(LeftEventArgs ev)
        { 
            if (AcceptPlayers.Contains(ev.Player.UserId))
            {
                if (ev.Player.Role.Type != RoleTypeId.Spectator && ev.Player.Role.Type != RoleTypeId.None)
                {
                    if (Features.GetPlayerBlockTime(ev.Player) == 0 && PluginBase.Instance!.Config.ReviveBlock)
                    {
                        PlayerApi.AddPlayer(ev.Player);
                        Log.Debug($"Player {ev.Player.Nickname} data stored.");

                        if (PluginBase.Instance.Config.RandomSpec)
                        {
                            if (ev.Player.Role.Team == Team.SCPs)
                            {
                                Features.RandomSpec(ev.Player);
                                Log.Debug("Random Spectator method executed.");
                            }
                        }
                        ev.Player.ClearInventory();
                    }
                }
            }
        }

        private void OnSpawned(SpawnedEventArgs ev)
        {
            if (!AcceptPlayers.Contains(ev.Player.UserId))
            {
                if (ev.Player.DoNotTrack)
                {
                    Log.Debug($"Player {ev.Player.Nickname} has DNT on.");
                    ev.Player.Broadcast(PluginBase.Instance?.Config.JoinedBroadcast);
                }
            }
        }

        private void OnDying(DyingEventArgs ev)
        {
            if (ev.DamageHandler.Type == DamageType.Decontamination || ev.DamageHandler.Type == DamageType.Custom || ev.DamageHandler.Type == DamageType.Tesla || ev.DamageHandler.Type == DamageType.Marshmallow || ev.DamageHandler.Type == DamageType.Crushed || ev.DamageHandler.Type == DamageType.Warhead)
            {
                PlayerApi.RemovePlayerData(ev.Player);
            }
            else
            {
                if (ev.Attacker == null) return;
                PlayerApi.RemovePlayerData(ev.Player);
            }
        }

        private void OnHurt(HurtEventArgs ev)
        {
            if (ev.Attacker != null && ev.DamageHandler.Type != DamageType.Tesla && ev.DamageHandler.Type != DamageType.Marshmallow && ev.DamageHandler.Type != DamageType.Crushed && ev.DamageHandler.Type != DamageType.Warhead && ev.Attacker.Role.Side != ev.Player.Role.Side && PluginBase.Instance!.Config.ReviveBlock)
            {
                Features.BlockRevive(ev.Player);
            }
        }

        private void OnAddingTarget(AddingTargetEventArgs ev)
        {
            if (PluginBase.Instance!.Config.ReviveBlock)
            {
                if (AcceptPlayers.Contains(ev.Target.UserId))
                {
                    Features.BlockRevive(ev.Target);
                }
            }
        }

        /*
        private void OnAddingObserver(AddingObserverEventArgs ev)
        {
            if (AutoReconnect.Instance.Config.ReviveBlock)
            {
                if (AcceptPlayers.Contains(ev.Target.UserId))
                {
                    Features.BlockRevive(ev.Target);
                }
            }
        }
        */
        
        private void OnVerified(VerifiedEventArgs ev)
        {
            PlayerData? playerData = PlayerApi.GetPlayerData(ev.Player);
            if (playerData == null) return;
    
            PlayerApi.GetAcceptPlayers();

            if (PluginBase.Instance!.Config.RespawnEnabled)
            {
                if (PluginBase.Instance.Config.RetentionTime != 0 && Features.IsReachedTimeLimit(ev.Player))
                {
                    PlayerApi.RemovePlayerData(ev.Player);
                    return;
                }

                if (Warhead.IsDetonated && ev.Player.Zone == ZoneType.Surface)
                {
                    if (PlayerApi.ResurrectPlayer(ev.Player, playerData))
                    {
                        ev.Player.Broadcast(5, PluginBase.Instance!.Config.ReconnectText, Broadcast.BroadcastFlags.Normal, true);
                    }
                }
                else
                {
                    if (PlayerApi.ResurrectPlayer(ev.Player, playerData))
                    {
                        ev.Player.Broadcast(5, PluginBase.Instance!.Config.ReconnectText, Broadcast.BroadcastFlags.Normal, true);
                    }
                }

                PlayerApi.DisconnectedPlayers?.Remove(ev.Player.UserId);
            }
        }
    }
}