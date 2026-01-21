using System;
using AdvancedCommands.Components.Extensions;
using AdvancedCommands.Components.Features;
using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Handlers;
using LabApi.Events.Arguments.PlayerEvents;
using Player = Exiled.API.Features.Player;

namespace AdvancedCommands.Commands.InfinityMagazines;

public class InfinityMagazinesHandler
{
    public void OnDroppingAmmo(DroppingAmmoEventArgs ev)
    {
        if (ev.Player.IsNPC)
            return;
        
        if (ev.Player.AdvancedCommand().PlayerProperties.IsInfinityMagazines)
            ev.IsAllowed = false;
    }
    
    public void OnDying(DyingEventArgs ev)
    {
        if (ev.Player.IsNPC)
            return;
        
        if (ev.Player.AdvancedCommand().PlayerProperties.IsInfinityMagazines && 
            !ev.Player.IsGodModeEnabled && 
            !ev.Player.IsSpawnProtected && 
            !ev.Player.IsEffectActive<AntiScp207>())
        {
            ev.Player.ClearAmmo();
        }
    }

    public void OnReloadedWeapon(ReloadedWeaponEventArgs ev)
    {
        if (ev.Player.IsNPC)
            return;
        
        if (ev.Player.AdvancedCommand().PlayerProperties.IsInfinityMagazines)
        {
            ev.Player.ClearAmmo();
            foreach (AmmoType type in Enum.GetValues(typeof(AmmoType)))
                ev.Player.AddAmmo(type, 1000);
        }
    }

    public void OnChangedRole(PlayerChangedRoleEventArgs ev)
    {
        if (Player.Get(ev.Player).IsNPC)
            return;
        
        try
        {
            var exiledPlayer = Player.Get(ev.Player);
            if (exiledPlayer.AdvancedCommand().PlayerProperties.IsInfinityMagazines &&
                exiledPlayer is { IsAlive: true, IsScp: false })
            {
                exiledPlayer.ClearAmmo();
                foreach (AmmoType type in Enum.GetValues(typeof(AmmoType)))
                    exiledPlayer.AddAmmo(type, 1000);
            }
        }
        catch { /*ignored*/ }
    }
}