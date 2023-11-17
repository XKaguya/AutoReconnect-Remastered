using System;
using Exiled.API.Features;

namespace AutoReconnectRemastered;

public class AutoReconnect : Plugin<Config>
{
    public static AutoReconnect? Instance;
    
    public override string Author => "RedLeaves & Rysik5318";
    public override string Name => "AutoReconnect-Remastered";
    public override Version Version => new(1, 0, 2);
    public override Version RequiredExiledVersion => new(8, 4, 0);
    
    public EventHandlers EventHandler { get; private set; }
    
    public override void OnEnabled()
    {
        Instance = this;
        EventHandler = new EventHandlers();
        EventHandlers.OnInitAcceptPlayers();
        
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
        Instance = this;
        EventHandler = new EventHandlers();
        EventHandlers.OnInitAcceptPlayers();
        
        base.OnReloaded();
    }
}