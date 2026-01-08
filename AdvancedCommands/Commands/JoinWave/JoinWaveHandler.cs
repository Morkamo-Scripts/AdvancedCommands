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

    public HashSet<RoleTypeId> AllowedChaosInsurgencyRoles { get; set; } = new()
    {
        RoleTypeId.ChaosConscript,
        RoleTypeId.ChaosRepressor,
        RoleTypeId.ChaosRifleman,
        RoleTypeId.ChaosMarauder
    };
    
    public HashSet<RoleTypeId> AllowedNtfRoles { get; set; } = new()
    {
        RoleTypeId.NtfSergeant,
        RoleTypeId.NtfPrivate,
    };

    public void OnRespawnedTeam(RespawnedTeamEventArgs ev)
    {
        if (Plugin.Instance.IsWaveBlockedAnotherTeam)
            return;
        
        Plugin.Instance.LastSpawnTime = DateTime.UtcNow;
        
        switch (ev.Wave.TargetFaction)
        {
            case Faction.FoundationStaff:
                Plugin.Instance.LastSpawnedSquad = SquadTypes.Ntf;
                break;
            case Faction.FoundationEnemy:
                Plugin.Instance.LastSpawnedSquad = SquadTypes.ChaosInsurgency;
                break;
        }

        foreach (var player in Player.List)
        {
            player.AdvancedCommand()?.PlayerProperties.HasBeenSpawned = false;
        }
    }
}