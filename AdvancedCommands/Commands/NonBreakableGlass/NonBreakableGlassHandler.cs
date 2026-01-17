using System;
using AdvancedCommands.Components.Extensions;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Permissions.Extensions;

namespace AdvancedCommands.Commands.NonBreakableGlass;

public class NonBreakableGlassHandler
{
    public static bool IsEnabled { get; set; } = false;
    public void OnDamagingWindow(DamagingWindowEventArgs ev) => ev.IsAllowed = !IsEnabled;
}