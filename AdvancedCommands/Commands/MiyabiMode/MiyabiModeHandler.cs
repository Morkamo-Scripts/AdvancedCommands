using System;
using AdvancedCommands.Components.Extensions;
using Exiled.Events.EventArgs.Player;
using Object = UnityEngine.Object;

namespace AdvancedCommands.Commands.MiyabiMode;

public class MiyabiModeHandler
{
    public void OnPlayerLeft(LeftEventArgs ev)
    {
        if (ev.Player == null || ev.Player.IsNPC)
            return;

        try
        {
            Object.Destroy(ev.Player.AdvancedCommand().PlayerProperties.MiyabiModel);
        }
        catch { /**/ }
    }
}