using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp2536;
using LabApi.Events.Arguments.ServerEvents;
using MapGeneration.Holidays;

namespace AdvancedCommands.Commands.DisableScp2536;

public class Disable2536Handler
{
    public static bool IsEnabledSpawn { get; set; } = true;
    
    public void OnFindingSpawnPoint(FindingPositionEventArgs ev)
        => ev.IsAllowed = IsEnabledSpawn;
}