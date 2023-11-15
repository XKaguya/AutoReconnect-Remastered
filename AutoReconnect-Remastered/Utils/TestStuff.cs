using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Features.Items;
using MEC;
using PluginAPI.Core;
using Player = Exiled.API.Features.Player;

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
    }
}