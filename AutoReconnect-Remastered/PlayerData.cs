#pragma warning disable CS1591

using System;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using PlayerRoles;
using System.Collections.Generic;
using Exiled.CustomRoles.API.Features;
using UnityEngine;

namespace PlayerInfo
{
    public class PlayerData
    {
        public PlayerData(Player player)
        {
            Player = player;
        }
        
        public float? ArtificialHealth { get; set; }
        public string Name { get; set; }
        public string? CustomName { get; set; }
        public string? CustomInfo { get; set; }
        public Vector3 Position { get; set; }
        public RoleTypeId Class { get; set; }
        public float Stamina { get; set; }
        public bool IsUsingStamina { get; set; }
        public Vector3 Scale { get; set; }
        public float Health { get; set; }
        public float? HumeShield { get; set; }
        public float? MaxArtificialHealth { get; set; }
        public float MaxHealth { get; set; }
        public HashSet<Item> Inventory { get; set; }
        public Player Player { get; set; }
        public Dictionary<ItemType, ushort> Ammo { get; set; }
        public HashSet<EffectList> Effects { get; set; } = new();
        public DateTime Time { get; set; }
        public int Exp { get; set; }
        public int Level { get; set; }
        public float Energy { get; set; }
        public CustomRole? CustomRole { get; set; }
        public Type? CustomRoleType { get; set; }
        public List<uint> CustomItems { get; set; } = new();
    }

    public class EffectList
    {
        public EffectList(EffectType effectType, byte intensity, float duration)
        {
            EffectType = effectType;
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