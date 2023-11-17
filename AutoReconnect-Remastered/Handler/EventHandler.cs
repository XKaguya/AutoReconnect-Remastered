using System.Collections.Generic;
using System.Linq;
using API.Other;
using Exiled.API.Enums;
using Exiled.Events.Commands.Reload;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using PluginAPI.Core;
using ARRAPI = API.API;
using Player = Exiled.API.Features.Player;

namespace AutoReconnectRemastered
{
    public class EventHandlers
{
    public static Dictionary<string, Player> DisconnectedPlayers = new();
    
    public static List<string> AcceptPlayers = new();
    public EventHandlers()
    {
        Exiled.Events.Handlers.Player.Verified += OnVerified;
        Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
        Exiled.Events.Handlers.Player.Left += OnLeft;
        Exiled.Events.Handlers.Player.Dying += OnDying;
        Exiled.Events.Handlers.Player.Spawned += OnSpawned;
        if (!AutoReconnect.Instance.Config.SpawnRagdoll) return;

        Exiled.Events.Handlers.Player.SpawningRagdoll += OnSpawningRagdolls;
    }
    ~EventHandlers()
    {
        Exiled.Events.Handlers.Player.Left -= OnLeft;
        Exiled.Events.Handlers.Player.Dying -= OnDying;
        Exiled.Events.Handlers.Player.Spawned -= OnSpawned;
        Exiled.Events.Handlers.Player.Verified -= OnVerified;
        Exiled.Events.Handlers.Player.SpawningRagdoll -= OnSpawningRagdolls;
        Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;
    }
    public void OnWaitingForPlayers() => ARRAPI.ClearPlayerData();

    public static void OnInitAcceptPlayers()
    {
        var AllPlayers = Player.List.ToList();
        foreach (var player in AllPlayers)
        {
            if (!player.ReferenceHub.authManager.DoNotTrack)
            {
                AcceptPlayers.Add(player.UserId);
            }
        }
    }

    public void OnSpawningRagdolls(SpawningRagdollEventArgs ev) => ev.IsAllowed = DisconnectedPlayers.ContainsKey(ev.Player.UserId) ? false : true;
    public void OnLeft(LeftEventArgs ev)
    {
        if (AcceptPlayers.Contains(ev.Player.UserId))
        {
            if (ev.Player.Role.Type != RoleTypeId.Spectator && ev.Player.Role.Type != RoleTypeId.None)
            {
                ARRAPI.AddPlayer(ev.Player);
                DisconnectedPlayers.Add(ev.Player.UserId, ev.Player);
                Log.Info($"Player {ev.Player.Nickname} data stored.");
            }
        }
    }

    public void OnSpawned(SpawnedEventArgs ev)
    {
        if (!AcceptPlayers.Contains(ev.Player.UserId))
        {
            if (ev.Player.ReferenceHub.authManager.DoNotTrack)
            {
                Log.Info($"Player {ev.Player.Nickname} has DNT on.");
                ev.Player.Broadcast(60, AutoReconnect.Instance.Config.DNT_Hint, Broadcast.BroadcastFlags.Normal);
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
        DisconnectedPlayers.Remove(ev.Player.UserId);
        PlayerData playerData = ARRAPI.GetPlayerData(ev.Player);
        if (playerData == null) return;

        if (ARRAPI.ResurrectPlayer(ev.Player, playerData))
            ev.Player.Broadcast(5, AutoReconnect.Instance.Config.ReconnectText, Broadcast.BroadcastFlags.Normal, true);
        ARRAPI.Players.Remove(ev.Player.UserId);
    }
}
}