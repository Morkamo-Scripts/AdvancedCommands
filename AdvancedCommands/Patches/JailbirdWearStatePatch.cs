using System.Reflection;
using AdvancedCommands.Components.Extensions;
using Exiled.API.Features;
using HarmonyLib;
using InventorySystem.Items.Jailbird;

namespace AdvancedCommands.Patches;

[HarmonyPatch(typeof(JailbirdDeteriorationTracker), "RecheckUsage")]
public class JailbirdWearStatePatch
{
    // ReSharper disable once InconsistentNaming
    private static bool Prefix(JailbirdDeteriorationTracker __instance)
    {
        var field = typeof(JailbirdDeteriorationTracker).GetField("_jailbird", BindingFlags.NonPublic | BindingFlags.Instance);
        var jailbird = field?.GetValue(__instance) as JailbirdItem;

        if (jailbird)
        {
            var owner = Player.Get(jailbird.Owner.networkIdentity);
            if (owner != null && owner.AdvancedCommand().PlayerProperties.IsInfinityJailbird)
                return false;
        }
        return true;
    }
}