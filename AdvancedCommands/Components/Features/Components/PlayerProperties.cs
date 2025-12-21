using System.Collections.Generic;
using AdvancedCommands.Components.Features.Components.Interfaces;
using UnityEngine;

namespace AdvancedCommands.Components.Features.Components;

public class PlayerProperties(PlayerAdvancedCommands playerAdvancedCommands) : IPropertyModule
{
    public PlayerAdvancedCommands PlayerAdvancedCommands { get; } = playerAdvancedCommands;
    public bool HasBeenSpawned { get; set; } = false;
}