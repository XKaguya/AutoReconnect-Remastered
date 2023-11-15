using System.Collections.Generic;
using AutoReconnectRemastered;
using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using PlayerRoles;

namespace AutoReconnectRemastered
{
    public class PlayerHandlers
    {
        private readonly AutoReconnect _instance;
    
        internal PlayerHandlers(AutoReconnect instance) => this._instance = instance;
        
        public RoleTypeId Class { get; set; }
        
        public float Health { get; set; }
        
        public IEnumerable<Item> Inventory { get; set; }
        
        public List<Item> Inventory_Clone { get; set; }
        
        public string Name { get; set; }
        
        public float Position_X { get; set; }
        
        public float Position_Y { get; set; }
        
        public float Position_Z { get; set; }
        
        public Player Player { get; set; }
        
        public Dictionary<ItemType, ushort> Ammo { get; set; }
        
        public Dictionary<ItemType, ushort> Ammo_Clone { get; set; }
        
        public IEnumerable<StatusEffectBase> Effects { get; set; }
        
        public Dictionary<EffectType, (byte, float)> Effects_Repertory  { get; set; }
    }
}