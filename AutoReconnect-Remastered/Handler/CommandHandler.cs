#pragma warning disable CS1591

using System;
using API;
using CommandSystem;
using Event;
using Exiled.API.Features;
using UnityEngine;

namespace Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Accept : ICommand
    {
        public string Command { get; } = "accept";
        public string[] Aliases { get; } = { "acc" };
        public string Description { get; } = "By inputting .accept , you're allowed server to storing your data for plugin use.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender? sender, out string response)
        {
            response = "You are now accepted.";

            Player player = Player.Get(sender);
            EventHandlers.AcceptPlayers.Add(player.UserId);
            player.ClearBroadcasts();
            Log.Debug($"Player {player.Nickname} accepted.");
            
            return true;
        }
    }
    
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Deny : ICommand
    {
        public string Command { get; } = "deny";
        public string[] Aliases { get; } = { "d" };
        public string Description { get; } = "Deny the server to store your data.";
        
        public bool Execute(ArraySegment<string> arguments, ICommandSender? sender, out string response)
        {
            response = "You are now denied.";
        
            Player player = Player.Get(sender);
            Log.Debug($"Player {player.Nickname} denied.");
        
            return true;
        }
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class Replace : ICommand
    {
        public string Command { get; } = "rp";
        public string[] Aliases { get; } = { "replace", "replaceplayer" };
        public string Description { get; } = "Replace departing player.";
        
        public bool Execute(ArraySegment<string> arguments, ICommandSender? sender, out string response)
        {
            try
            {
                if (PlayerApi.DisconnectedPlayers.IsEmpty())
                {
                    response = "There's no disconnected player.";
                    
                    return false;
                }

                Player player = Player.Get(sender);
                Features.ReplacePlayer(player);
                
                response = "You have replaced a departing player.";
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + ex.StackTrace);
                throw;
            }
        
            return true;
        }
    }
}