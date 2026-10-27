using System.Collections.Generic;
using AdvancedCommands.Commands.ItsMyScp;
using HarmonyLib;
using PlayerRoles.RoleAssign;

namespace AdvancedCommands.Patches;

[HarmonyPatch(typeof(ScpPlayerPicker), "GenerateList")]
public class ItsMyScpPlayerPickerPatch
{
    private static void Prefix(int scpsToAssign, out int __state) => __state = scpsToAssign;
    
    private static void Postfix(int __state, List<ReferenceHub> ___ScpsToSpawn)
        => ItsMyScpHandler.ApplyReservations(__state, ___ScpsToSpawn);
}
