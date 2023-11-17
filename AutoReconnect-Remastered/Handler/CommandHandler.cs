using System;
using CommandSystem;
using Exiled.API.Features;

namespace AutoReconnectRemastered
{
    public class CommandHandler
    {
        [CommandHandler(typeof(ClientCommandHandler))]
        public class AutoReconnectAccept : ICommand
        {
            public string Command { get; } = "accept";
            public string[] Aliases { get; } = Array.Empty<string>();
            public string Description { get; } = "By inputing .accept , you're allowed server to storing your data for plugin use.";

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
    
        public class AutoReconnectDeny : ICommand
        {
            public string Command { get; } = "deny";
            public string[] Aliases { get; } = Array.Empty<string>();
            public string Description { get; } = "Deny the server to store your data.";
            
            public bool Execute(ArraySegment<string> arguments, ICommandSender? sender, out string response)
            {
                response = "You are now denied.";
            
                Player player = Player.Get(sender);
                Log.Debug($"Player {player.Nickname} denied.");
            
                return true;
            }
        }
    }
}