using System;
using System.IO;
using System.Security;
using System.Security.Permissions;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.Commands.Reload;
using Exiled.Events.EventArgs.Player;
using Exiled.Loader;
using Exiled.Permissions.Features;
using Interactables.Interobjects.DoorUtils;
using LabApi.Features.Permissions;
using LabApi.Features.Permissions.Providers;
using Query;
using RemoteAdmin;

namespace AdvancedCommands.Commands.ReservedSlotsManagement;

public class RsmHandler
{
    public void OnCheckReservedSlot(ReservedSlotsCheckEventArgs ev)
    {
        if (ev.HasReservedSlot)
        {
            ev.Result = ReservedSlotEventResult.CanUseReservedSlots;
            ev.IsAllowed = true;
        }

        string group = Components.Extensions.Utils
            .GetRoleFromRemoteAdminConfig(Plugin.Instance.RsmHeader.RaConfigPatch, ev.UserId);
        
        if (group == null)
            return;
        
        if (Plugin.Instance.RsmHeader.ReservedGroups.Contains(group))
        {
            ev.Result = ReservedSlotEventResult.CanUseReservedSlots;
            ev.IsAllowed = true;
        }
    }
}