#pragma warning disable CS1591

using System.ComponentModel;
using Exiled.API.Interfaces;
using PlayerRoles;

namespace Configurations
{
    public class Config : IConfig
    {
        [Description("Whether or not this plugin is enabled.")]
        public bool IsEnabled { get; set; } = true;

        [Description("Whether or not to display debug messages in the server console.")]
        public bool Debug { get; set; } = false;
        
        [Description("How long will the server hold the player's data. Set to 0 to disable time limit.")]
        public int RetentionTime { get; set; } = 0;
        
        [Description("Whether or not to respawn player. Set to false and it WILL NOT respawn player.")]
        public bool RespawnEnabled { get; set; } = true;
    
        [Description("Whether or not to restore player's active effect. Player will respawn with same effect strengh and remain time.")]
        public bool RecoveryEffect { get; set; } = true;
    
        [Description("Whether or not to restore player's inventory. Player will respawn with same inventory.")]
        public bool RecoveryInventory { get; set; } = true;
    
        [Description("Whether or not to restore player's ammo. Player will respawn with same ammo.")]
        public bool RecoveryAmmo { get; set; } = true;
        
        [Description("Whether or not to spawn disconnected player's ragdoll.")]
        public bool SpawnRagdoll { get; set; } = false;
        
        [Description("Whether or not to randomly choose a spectator to be the disconnected SCP. While RespawnEnabled is false, this config will take the place of the respawn.")]
        public bool RandomSpec { get; set; } = false;
    
        [Description("Text that will be shown to the player at reconnect.")]
        public string ReconnectText { get; set; } = "You have been reconnected. You will be respawn to stored status.";
        
        [Description("Whether or not to enable revive block. If true then player won't be revive in the revive block time.")]
        public bool ReviveBlock { get; set; } = true;
        
        [Description("How long the revive will be block.")]
        public int ReviveBlockTime { get; set; } = 10;
        
        [Description("How long the revive timer will delay.")]
        public float ReviveBlockDelay { get; set; } = 1;

        [Description("Text that will be shown to the player who enabled DNT.")]
        public Exiled.API.Features.Broadcast JoinedBroadcast { get; private set; } = new("Plugin requires your auth for storing data. Do you want to accept that ?\nIf accepted you''re able to enjoy reconnect respawn features.\nType .accept in console for accept .deny for deny.", 15);

        [Description("The message that sent to all players while scp is replaced by spectator.")]
        public string DisconnectedMessage { get; set; } = "{0} has been replaced by player {1}";
        
        [Description("The way plugin to show for spectator players. Set 1 for Hint, 2 for Broadcast.")]
        public int DisconnectedMessageType { get; set; } = 1;
        
        [Description("The information used for other plugins to avoid conflicts. Set to default if you have no conflict.")]
        public RoleChangeReason SpawnReason { get; set; } = RoleChangeReason.RemoteAdmin;
        
        [Description("Whether or not to enable .rp command. This command allows player to replace a disconnected scp.")]
        public bool ReplacePlayer { get; set; } = true;
        
        [Description("Whether or not to enable custom moudle support. Default is true.")]
        public bool CustomModuleSupport { get; set; } = true;
    }
}