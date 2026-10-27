using AdvancedCommands.Components.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using UnityEngine;

namespace AdvancedCommands.Commands.CapybaraMode;

public class CapybaraModeHandler
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