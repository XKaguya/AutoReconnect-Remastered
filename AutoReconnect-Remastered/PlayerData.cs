using System;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using PlayerRoles;
using System.Collections.Generic;
/*using Exiled.CustomModules.API.Features.CustomItems;
using Exiled.CustomModules.API.Features.CustomRoles;*/
using UnityEngine;

namespace AutoReconnect_Remastered
{
    public class PlayerData
    {
        public PlayerData(Player player)
        {
            Player = player;
        }
        public string Name { get; set; }

        public Vector3 Position { get; set; }

        public RoleTypeId Class { get; set; }

        public float Health { get; set; }

        public HashSet<Item> Inventory { get; set; }

        public Player Player { get; set; }

        public Dictionary<ItemType, ushort> Ammo { get; set; }

        public HashSet<EffectList> Effects { get; set; } = new();
        
        public DateTime Time { get; set; }
        
        public int Exp { get; set; }
        
        public int Level { get; set; }
        
        public float Energy { get; set; }
        
        /*public CustomRole? CustomRole { get; set; }

        public List<CustomItem> CustomItems { get; set; } = new();*/
    }
    
    public class EffectList
    {
        public EffectList(EffectType effectType, byte intensity, float duration)
        {
            this.EffectType = effectType;
            Intensity = intensity;
            Duration = duration;
        }
        public EffectType EffectType { get; set; }
        public byte Intensity { get; set; }
        public float Duration { get; set; }
    }

    public static class ReviveBlock
    {
        public static Dictionary<string, int> BlockTime { get; set; } = new();
    }
}