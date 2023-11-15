using AutoReconnectRemastered;
using System;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;

namespace AutoReconnectRemastered
{
    public class AutoReconnect : Plugin<Config>
    {
        public static AutoReconnect? Instance;
        
        public override string Author => "RedLeaves";
        public override string Name => "AutoReconnect-Remastered";

        public override Version Version { get; } = new(1, 0, 0);
        public override Version RequiredExiledVersion { get; } = new(8, 3, 9);
        
        public EventHandlers? EventHandlers { get; private set; }
        public PlayerHandlers? PlayerHandlers { get; private set; }
        
        public PlayerData? PlayerData { get; private set; }
        
        public AmmoData? AmmoData { get; private set; }
        
        public Timer? Timer { get; private set; }
        
        public TestStuff? TestStuff { get; private set; }

        public void InstanceController(bool controller)
        {
            try
            {
                if (controller)
                {
                    AmmoData = new AmmoData(this);
                    EventHandlers = new EventHandlers(this);
                    PlayerHandlers = new PlayerHandlers(this);
                    PlayerData = new PlayerData(this);
                    Timer = new Timer(this);
                    TestStuff = new TestStuff(this);
                    
                    Log.Info("Instance regist successful.");
                }
                else
                {
                    AmmoData = null;
                    EventHandlers = null;
                    PlayerHandlers = null;
                    PlayerData = null;
                    Timer = null;
                    TestStuff = null;
                    
                    Log.Info("Instance unregist successful.");
                }
            }
            catch (Exception e)
            {
                var text = $"Instance regist failed. Reason: {e}";
                Log.Error(text);
                throw;
            }
        }

        public void EventController(bool controller)
        {
            try
            {
                if (controller)
                {
                    Exiled.Events.Handlers.Player.Verified += EventHandlers.OnVerified;
                    Exiled.Events.Handlers.Server.WaitingForPlayers += EventHandlers.OnWaitingForPlayers;
                    Exiled.Events.Handlers.Player.Left += EventHandlers.OnDisconnected;
                    Exiled.Events.Handlers.Server.RoundStarted += EventHandlers.OnRoundstarted;
                    Exiled.Events.Handlers.Player.Dying += EventHandlers.OnDying;
                    
                    Log.Info("Event subscribe successful.");
                }
                else
                {
                    Exiled.Events.Handlers.Player.Verified -= EventHandlers.OnVerified;
                    Exiled.Events.Handlers.Server.WaitingForPlayers -= EventHandlers.OnWaitingForPlayers;
                    Exiled.Events.Handlers.Player.Left -= EventHandlers.OnDisconnected;
                    Exiled.Events.Handlers.Server.RoundStarted -= EventHandlers.OnRoundstarted;
                    Exiled.Events.Handlers.Player.Dying -= EventHandlers.OnDying;
                    
                    Log.Info("Event unsubscribe successful.");
                }
            }
            catch (Exception e)
            {
                var text = $"Event subscribe failed. Reason: {e}";
                Log.Error(text);
                throw;
            }
        }
        
        public override void OnEnabled()
        {
            Instance = this;
            InstanceController(true);
            EventController(true);
            
            base.OnEnabled();
        }
        
        public override void OnDisabled()
        {
            AutoReconnect.Instance.Timer.KillTimer();
            
            EventController(false);
            Instance = null;
            InstanceController(false);
            base.OnDisabled();
        }

        public override void OnReloaded()
        {
            Instance = this;
            InstanceController(true);
            EventController(true);
            
            AutoReconnect.Instance.Timer.RunTimer();
            base.OnReloaded();
        }
    }
}