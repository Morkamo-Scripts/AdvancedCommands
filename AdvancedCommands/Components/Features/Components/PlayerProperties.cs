using System;
using System.Collections.Generic;
using AdvancedCommands.Components.Features.Components.Interfaces;
using Exiled.API.Enums;
using PlayerRoles;
using UnityEngine;

namespace AdvancedCommands.Components.Features.Components;

public class PlayerProperties(PlayerAdvancedCommands playerAdvancedCommands) : IPropertyModule
{
    public PlayerAdvancedCommands PlayerAdvancedCommands { get; } = playerAdvancedCommands;
    
    public bool HasBeenSpawned { get; set; } = false;
    public bool IsOwnerModeEnabled { get; set; } = false;
    public bool IsUnlimitedAmmo { get; set; } = false;
    public RoleTypeId? ReservedRole { get; set; } = null;
    
    public bool IsInfinityJailbird { get; set; } = false;
    
    private bool _isInfinityMagazines = false;
    public bool IsInfinityMagazines
    {
        get => _isInfinityMagazines;
        set
        {
            if (_isInfinityMagazines == value)
                return;

            _isInfinityMagazines = value;

            if (!value)
            {
                PlayerAdvancedCommands.Player.ClearAmmo();
                return;
            }

            PlayerAdvancedCommands.Player.ClearAmmo();

            foreach (AmmoType type in Enum.GetValues(typeof(AmmoType)))
                PlayerAdvancedCommands.Player.AddAmmo(type, 1000);
        }
    }

    public DateTime? LastCall { get; set; }
    public bool ImSleeping { get; set; } = false;
}