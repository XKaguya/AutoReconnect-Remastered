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
    }
}