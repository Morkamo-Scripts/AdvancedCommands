using System;
using Exiled.Events.EventArgs.Player;

namespace AdvancedCommands.Commands.GlobalInfinityRadio;

public class GlobalInfinityRadioHandler
{
    public static bool IsEnabled { get; set; } = false;
    public static bool IsFirstUsing { get; set; } = true;

    public void OnUsingRadioBattery(UsingRadioBatteryEventArgs ev)
    {
        if (ev.Player.IsNPC)
            return;
        
        if (IsEnabled)
        {
            ev.Radio.BatteryLevel = 100;
            ev.IsAllowed = IsFirstUsing;
            if (IsFirstUsing)
                IsFirstUsing = false;
        }
    }
}