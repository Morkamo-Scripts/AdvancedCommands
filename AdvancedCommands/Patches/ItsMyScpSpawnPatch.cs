using System.Collections.Generic;
using AdvancedCommands.Commands.ItsMyScp;
using HarmonyLib;
using PlayerRoles;
using PlayerRoles.RoleAssign;

namespace AdvancedCommands.Patches;

[HarmonyPatch(typeof(ScpSpawner), nameof(ScpSpawner.SpawnScps))]
public class ItsMyScpSpawnPatch
{
    private static void Prefix(List<RoleTypeId> ___EnqueuedScps) => ItsMyScpHandler.BeginAssignment(___EnqueuedScps);

    private static void Finalizer() => ItsMyScpHandler.ClearReservations();
}
