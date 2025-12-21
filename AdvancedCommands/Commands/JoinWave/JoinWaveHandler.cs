using System;
using System.Collections.Generic;
using System.ComponentModel;
using AdvancedCommands.Components.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Server;
using LabApi.Events.Arguments.ServerEvents;
using PlayerRoles;
using UnityEngine;

namespace AdvancedCommands.Commands.JoinWave;

public class JoinWaveHandler
{
    public ushort SpawnOvertime { get; set; } = 30;
    public DateTime? LastSpawnTime;
    public Team? LastSpawnTeam;

    public HashSet<RoleTypeId> AllowedChaosInsurgencyRoles { get; set; } = new()
    {
        RoleTypeId.ChaosConscript,
        RoleTypeId.ChaosMarauder,
        RoleTypeId.ChaosRepressor,
        RoleTypeId.ChaosRifleman
    };
    
    public HashSet<RoleTypeId> AllowedNtfRoles { get; set; } = new()
    {
        RoleTypeId.NtfCaptain,
        RoleTypeId.NtfSergeant,
        RoleTypeId.NtfPrivate,
        RoleTypeId.NtfSpecialist
    };

    public void OnChaosEntrance(AnnouncingChaosEntranceEventArgs ev)
    {
        LastSpawnTime = DateTime.UtcNow;
        LastSpawnTeam = ev.Wave.Team;

        foreach (var player in Player.List)
        {
            player.AdvancedCommand().PlayerProperties.HasBeenSpawned = false;
        }
    }
    
    public void OnNtfEntrance(AnnouncingNtfEntranceEventArgs ev)
    {
        LastSpawnTime = DateTime.UtcNow;
        LastSpawnTeam = ev.Wave.Team;
        
        foreach (var player in Player.List)
        {
            player.AdvancedCommand().PlayerProperties.HasBeenSpawned = false;
        }
    }
}