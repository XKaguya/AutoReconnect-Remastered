using System;
using Exiled.API.Features;

namespace AutoReconnectRemastered
{
    public class AutoReconnect : Plugin<Config>
    {
        public static AutoReconnect? Instance;
    
        public override string Author => "Decline & Rysik5318";
        public override string Name => "AutoReconnect-Remastered";
        public override Version Version => new(1, 1, 7);
        public override Version RequiredExiledVersion => new(8, 7, 3);
    
        public EventHandlers? EventHandler { get; private set; }
    
        public override void OnEnabled()
        {
            Instance = this;
            EventHandler = new EventHandlers();
        
            base.OnEnabled();
        }
    
        public override void OnDisabled()
        {
            Instance = null;
            EventHandler = null;
            
            base.OnDisabled();
        }

        public override void OnReloaded()
        {
            Instance = null;
            EventHandler = null;
        
            base.OnReloaded();
        }
    }
}