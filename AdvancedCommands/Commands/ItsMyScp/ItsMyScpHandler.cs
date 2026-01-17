using System.Collections.Generic;
using AdvancedCommands.Components.Extensions;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using UnityEngine;

namespace AdvancedCommands.Commands.ItsMyScp;

public class ItsMyScpHandler
{
    public static Dictionary<RoleTypeId, Player> ReservedRoles { get; set; } = new();
    
    public void OnSpawning(SpawningEventArgs ev)
    {
        if (ReservedRoles.IsEmpty())
            return;

        Round.IsLocked = true;
        Timing.CallDelayed(0.2f, () => Round.IsLocked = false);

        var props = ev.Player.AdvancedCommand()?.PlayerProperties;
        
        if (props == null || props.ReservedRole == null)
        {
            foreach (var role in ReservedRoles)
            {
                if (ev.NewRole == role.Key)
                {
                    switch (Random.Range(0, 3))
                    {
                        case 0:
                        {
                            ev.Player.Role.Set(RoleTypeId.ClassD);
                            break;
                        }
                        case 1:
                        {
                            ev.Player.Role.Set(RoleTypeId.Scientist);
                            break;
                        }
                        case 2:
                        {
                            ev.Player.Role.Set(RoleTypeId.FacilityGuard);
                            break;
                        }
                    }
                }
            }
            
            return;
        }
        
        Timing.CallDelayed(0.1f, () =>
        {
            var reserverRole = (RoleTypeId)props.ReservedRole!;
            ReservedRoles.Remove(reserverRole);
            props.ReservedRole = null;
            ev.Player.Role.Set(reserverRole, RoleSpawnFlags.All);
        });
    }

    public void OnRoundEnded(RoundEndedEventArgs ev) => ReservedRoles.Clear();
}