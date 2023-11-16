using System.ComponentModel;
using Exiled.API.Interfaces;

namespace AutoReconnectRemastered
{
    /// <inheritdoc />
    public class Config : IConfig
    {
        /// <inheritdoc />
        [Description("Whether or not this plugin is enabled.")]
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether debug messages will be shown.
        /// </summary>
        [Description("Whether or not to display debug messages in the server console.")]
        public bool Debug { get; set; } = false;
        
        [Description("Whether or not to restore player's active effect. Player will respawn with same effect strengh and remain time.")]
        public bool RecoveryEffect { get; set; } = true;
        
        [Description("Whether or not to restore player's inventory. Player will respawn with same inventory.")]
        public bool RecoveryInventory { get; set; } = true;
        
        [Description("Whether or not to restore player's ammo. Player will respawn with same ammo.")]
        public bool RecoveryAmmo { get; set; } = true;
        
        [Description("Whether or not to spawn disconnected player's ragdoll.")]
        public bool SpawnRagdoll { get; set; } = true;
        
        [Description("Language (en_US, zh_CN)")]
        public string Language { get; set; } = "zh_CN";
    }
}