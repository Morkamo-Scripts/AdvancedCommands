/*using System;
using AdvancedCommands.Components.Extensions;
using CommandSystem.Commands.RemoteAdmin;
using Exiled.API.Enums;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.MicroHID.Modules;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using RueI.API;
using RueI.API.Elements;
using Player = Exiled.API.Features.Player;

namespace AdvancedCommands.Commands.OwnerMode;

public class OwnerModeHandler
{
    public void OnHandcuffing(HandcuffingEventArgs ev)
    {
        if (ev.Player.IsNPC)
            return;
        
        if (ev.Target.AdvancedCommand().PlayerProperties.IsOwnerModeEnabled)
        {
            ev.IsAllowed = false;
            RueDisplay.Get(ev.Player).Show(
                new Tag(),
                new BasicElement(200, "<b><color=red>Нельзя связать игрока со включенным</color>\n" +
                                      "<color=#F79100>OwnerMode!</color>"), 3);
            
            Timing.CallDelayed(3.2f, () => RueDisplay.Get(ev.Player).Update());
        }
    }

    public void OnOwnerModeStatusChanged(Player player, bool status)
    {
        if (player.IsNPC)
            return;
        
        if (status)
        {
            player.Role.Set(RoleTypeId.Tutorial, RoleSpawnFlags.None);
            
            player.IsGodModeEnabled = true;
            player.IsUsingStamina = false;
            player.IsBypassModeEnabled = true;
            player.IsNoclipPermitted = true;
            
            player.EnableEffect(EffectType.MovementBoost, 75);
            player.EnableEffect(EffectType.Ghostly);
            player.EnableEffect(EffectType.SilentWalk, 10);
            player.EnableEffect(EffectType.NightVision, 20);

            player.AdvancedCommand().PlayerProperties.IsUnlimitedAmmo = true;
        }
        else
        {
            player.IsGodModeEnabled = false;
            player.IsUsingStamina = true;
            player.IsBypassModeEnabled = false;
            player.IsNoclipPermitted = false;
            
            player.DisableEffect(EffectType.MovementBoost);
            player.DisableEffect(EffectType.Ghostly);
            player.DisableEffect(EffectType.SilentWalk);
            player.DisableEffect(EffectType.NightVision);

            player.AdvancedCommand().PlayerProperties.IsUnlimitedAmmo = false;
        }
    }

    public void OnDying(DyingEventArgs ev)
    {
        if (ev.Player.IsNPC)
            return;
        
        try
        {
            if (ev.Player.AdvancedCommand().PlayerProperties.IsOwnerModeEnabled)
                ev.IsAllowed = false;
        }
        catch { /*ignored#1# }
    }

    public void OnShooting(PlayerShootingWeaponEventArgs ev)
    {
        if (Player.Get(ev.Player).IsNPC)
            return;
        
        if (Player.Get(ev.Player).AdvancedCommand().PlayerProperties.IsOwnerModeEnabled)
        {
            ev.FirearmItem.StoredAmmo = ev.FirearmItem.MaxAmmo;
            ev.IsAllowed = true;
        }
    }

    public void OnUsingMicroHID(UsingMicroHIDEnergyEventArgs ev)
    {
        if (ev.Player.AdvancedCommand().PlayerProperties.IsOwnerModeEnabled)
        {
            ev.Drain = 0f;
            ev.IsAllowed = true;
        }
    }

    public void OnMicroHidExploding(ExplodingMicroHIDEventArgs ev)
    {
        if (ev.Player.AdvancedCommand().PlayerProperties.IsOwnerModeEnabled)
            ev.IsAllowed = false;
    }

    public void OnChangingMicroHIDState(ChangingMicroHIDStateEventArgs ev)
    {
        if (ev.Player.AdvancedCommand().PlayerProperties.IsOwnerModeEnabled && ev.NewPhase == MicroHidPhase.WindingUp)
            ev.NewPhase = MicroHidPhase.Firing;
    }

    public void OnHurting(HurtingEventArgs ev)
    {
        if (ev.Attacker != null && ev.Attacker.AdvancedCommand().PlayerProperties.IsOwnerModeEnabled &&
            ev.DamageHandler.Type == DamageType.MicroHid)
        {
            ev.Amount = 100;
        }
    }

    public void OnItemAdded(ItemAddedEventArgs ev)
    {
        try
        {
            if (ev.Player.AdvancedCommand().PlayerProperties.IsOwnerModeEnabled && ev.Item.IsFirearm)
            {
                var firearm = ev.Item.As<Firearm>();
                firearm.MagazineAmmo = firearm.MaxMagazineAmmo;
            }
        }
        catch { /*ignored#1# }
    }

    public void OnChangingRole(ChangingRoleEventArgs ev)
    {
        if (ev.Player.AdvancedCommand() != null && ev.Player.AdvancedCommand().PlayerProperties.IsOwnerModeEnabled)
        {
            ev.IsAllowed = false;
            RueDisplay.Get(ev.Player).Show(
                new Tag(),
                new BasicElement(200, "<b><color=red>Нельзя сменить роль со включенным</color>\n" +
                                      "<color=#F79100>OwnerMode!<color>"), 3);
            
            Timing.CallDelayed(3.2f, () => RueDisplay.Get(ev.Player).Update());
        }
    }
}*/