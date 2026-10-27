using System.Collections.Generic;
using AdvancedCommands.Commands.ItsMyScp;
using HarmonyLib;
using PlayerRoles;
using PlayerRoles.RoleAssign;

namespace AdvancedCommands.Patches;

[HarmonyPatch(typeof(ScpSpawner), "AssignScp")]
public class ItsMyScpAssignPatch
{
    private static void Prefix(List<ReferenceHub> chosenPlayers, RoleTypeId scp, out List<ReferenceHub> __state)
        => __state = ItsMyScpHandler.FilterPlayers(chosenPlayers, scp);
    
    private static void Finalizer(List<ReferenceHub> chosenPlayers, List<ReferenceHub> __state)
    {
        if (__state != null)
            chosenPlayers.AddRange(__state);
    }
}
