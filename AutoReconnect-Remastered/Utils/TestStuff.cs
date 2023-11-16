using System;
using System.Collections;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using MEC;
using Log = PluginAPI.Core.Log;
using Player = Exiled.API.Features.Player;
using Server = PluginAPI.Core.Server;

namespace AutoReconnectRemastered
{
    public class TestStuff
    {
        private readonly AutoReconnect _instance;

        internal TestStuff(AutoReconnect instance) => this._instance = instance;
        
        [CommandHandler(typeof(ClientCommandHandler))]
        public class AutoReconnectTryDestroyerItems : ICommand
        {
            public string Command => "destitem";
            public string Description => "Try destroyer items";
            public string[] Aliases => Array.Empty<string>();

            public bool Execute(ArraySegment<string> arguments, ICommandSender? sender, out string response)
            {
                response = "Items Destroyerd.";

                var AllPlayers = Player.List.Where(p => p.IsAlive).ToList();
                Log.Info("------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                foreach (var player in AllPlayers)
                {
                    PlayerHandlers PlayerData = AutoReconnect.Instance.PlayerData.GetPlayerData(player);
                    foreach (Item item in PlayerData.Inventory_Clone)
                    {
                        item.Destroy();
                    }
                }
                Log.Info("------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                
                AutoReconnect.Instance.PlayerData.DisplayPlayersInfo();
                
                return true;
            }
        }
        
        [CommandHandler(typeof(ClientCommandHandler))]
        public class AutoReconnectShowPlayerData : ICommand
        {
            public string Command => "showpd";
            public string Description => "Show PlayerData.";
            public string[] Aliases => Array.Empty<string>();

            public bool Execute(ArraySegment<string> arguments, ICommandSender? sender, out string response)
            {
                response = "PlayerData printed.";

                var AllPlayers = Player.List.Where(p => p.IsAlive).ToList();
                Log.Info("------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                foreach (var player in AllPlayers)
                {
                    PlayerHandlers PlayerData = AutoReconnect.Instance.PlayerData.GetPlayerData(player);
                    Log.Info($"Player {player.Nickname}, ID: {player.Id}, UserId: {player.UserId}");
                    foreach (Item item in PlayerData.Inventory_Clone)
                    {
                        Log.Info($"Item {item.Type}, Owner: {item.Owner}");
                    }
                }
                Log.Info("------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                
                AutoReconnect.Instance.PlayerData.DisplayPlayersInfo();
                
                return true;
            }
        }
        
        [CommandHandler(typeof(ClientCommandHandler))]
        public class AutoReconnectStartTimer : ICommand
        {
            public string Command => "starttimer";
            public string Description => "Start AutoReconnect's timer.";
            public string[] Aliases => Array.Empty<string>();

            public bool Execute(ArraySegment<string> arguments, ICommandSender? sender, out string response)
            {
                response = "Timer started.";
                
                AutoReconnect.Instance.Timer.RunTimer();
                
                return true;
            }
        }

        [CommandHandler(typeof(ClientCommandHandler))]
        public class AutoReconnectRagdoll : ICommand
        {
            public string Command => "showragdoll";
            public string Description => "Show all the ragdolls.";
            public string[] Aliases => Array.Empty<string>();

            public bool Execute(ArraySegment<string> arguments, ICommandSender? sender, out string response)
            {

                response = "Ragdolls information:\n";

                foreach (Ragdoll ragdoll in Ragdoll.List)
                {
                    if (ragdoll?.Owner != null && !string.IsNullOrEmpty(ragdoll.Owner.UserId))
                    {
                        Log.Info(
                            $"Ragdoll ID: {ragdoll}, Owner: {ragdoll.Owner.Nickname}, Owner UserId: {ragdoll.Owner.UserId}");
                    }
                    else
                    {
                        Log.Info($"Ragdoll ID: {ragdoll}, Owner: None");
                    }
                }

                return true;
            }
        }

        [CommandHandler(typeof(ClientCommandHandler))]
        public class AutoReconnectShowTPS : ICommand
        {
            public string Command => "tps";
            public string Description => "Show server's TPS.";
            public string[] Aliases => Array.Empty<string>();

            public bool Execute(ArraySegment<string> arguments, ICommandSender? sender, out string response)
            {
                response = "Timer started.";
                
                Log.Info($"{Server.TPS}");
                
                return true;
            }
        }
        
        [CommandHandler(typeof(ClientCommandHandler))]
        public class AutoReconnectCheckNull : ICommand
        {
            public string Command => "checkpd";
            public string Description => "Check playerdata to see if it was null.";
            public string[] Aliases => Array.Empty<string>();

            public bool Execute(ArraySegment<string> arguments, ICommandSender? sender, out string response)
            {
                response = "Result: ";

                Player player = Player.Get(sender);
                
                PlayerHandlers PlayerData = AutoReconnect.Instance.PlayerData.GetPlayerData(player);
                
                if (PlayerData.Inventory == null || !PlayerData.Inventory.Any())
                {
                    Log.Info($"PlayerData.Inventory is null");
                }
                
                if (PlayerData.Inventory_Clone == null || PlayerData.Inventory_Clone.Count == 0)
                {
                    Log.Info($"PlayerData.Inventory_Clone is null");
                }
                
                if (PlayerData.Ammo == null || !PlayerData.Ammo.Any())
                {
                    Log.Info($"PlayerData.Ammo is null");
                }
                
                if (PlayerData.Ammo_Clone == null || PlayerData.Ammo_Clone.Count == 0)
                {
                    Log.Info($"PlayerData.Ammo_Clone is null");
                }
                
                if (PlayerData.Effects == null || !PlayerData.Effects.Any())
                {
                    Log.Info($"PlayerData.Effects is null");
                }
                
                if (PlayerData.Effects_Repertory == null || PlayerData.Effects_Repertory.Count == 0)
                {
                    Log.Info($"PlayerData.Effects_Repertory is null");
                }
                
                return true;
            }
        }
    }
}