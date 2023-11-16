using API.Other;
using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using ARRAPI = API.API;

namespace AutoReconnectRemastered;

public class EventHandlers
{
    public EventHandlers()
    {
        Exiled.Events.Handlers.Player.Verified += OnVerified;
        Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
        Exiled.Events.Handlers.Player.Left += OnLeft;
        Exiled.Events.Handlers.Player.Dying += OnDying;
        if (!AutoReconnect.Instance.Config.SpawnRagdoll) return;

        Exiled.Events.Handlers.Player.SpawningRagdoll += OnSpawningRagdolls;
    }
    ~EventHandlers()
    {
        Exiled.Events.Handlers.Player.Left -= OnLeft;
        Exiled.Events.Handlers.Player.Dying -= OnDying;
        Exiled.Events.Handlers.Player.Verified -= OnVerified;
        Exiled.Events.Handlers.Player.SpawningRagdoll -= OnSpawningRagdolls;
        Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;
    }
    public void OnWaitingForPlayers() => ARRAPI.ClearPlayerData();

    public void OnSpawningRagdolls(SpawningRagdollEventArgs ev) => ev.IsAllowed = ARRAPI.DisconnectedPlayers.ContainsKey(ev.Player.UserId) ? false : true;
    public void OnLeft(LeftEventArgs ev)
    {
        if (!ev.Player.Role.IsAlive) return;

        ARRAPI.AddPlayer(ev.Player);
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
        ARRAPI.DisconnectedPlayers.Remove(ev.Player.UserId);
        PlayerHandlers playerData = ARRAPI.GetPlayerData(ev.Player);
        if (playerData == null) return;

        if (ARRAPI.ResurrectPlayer(ev.Player, playerData))
            ev.Player.Broadcast(5, AutoReconnect.Instance.Config.ReconnectText, Broadcast.BroadcastFlags.Normal, true);

    }
}