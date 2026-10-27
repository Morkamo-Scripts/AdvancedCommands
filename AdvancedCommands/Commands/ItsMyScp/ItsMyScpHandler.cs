using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using HarmonyLib;
using PlayerRoles;
using PlayerRoles.PlayableScps;
using PlayerRoles.RoleAssign;
using Random = UnityEngine.Random;

namespace AdvancedCommands.Commands.ItsMyScp;

public class ItsMyScpHandler
{
    public static List<RoleTypeId> AvailableRoles { get; } = new()
    {
        RoleTypeId.Scp049,
        RoleTypeId.Scp079,
        RoleTypeId.Scp096,
        RoleTypeId.Scp106,
        RoleTypeId.Scp173,
        RoleTypeId.Scp939,
        RoleTypeId.Scp3114,
    };

    public static Dictionary<Player, RoleTypeId> ReservedRoles { get; } = new();

    internal static bool IsAssigning { get; private set; }

    private static readonly Dictionary<ReferenceHub, RoleTypeId> AssignedRoles = new();
    private static readonly Func<RoleTypeId> GetNextScp = AccessTools.MethodDelegate<Func<RoleTypeId>>(
        AccessTools.PropertyGetter(typeof(ScpSpawner), "NextScp"));

    private static List<RoleTypeId> _enqueuedScps;

    internal static void BeginAssignment(List<RoleTypeId> enqueuedScps)
    {
        AssignedRoles.Clear();
        _enqueuedScps = enqueuedScps;
        IsAssigning = Plugin.Instance?.Config.CommandProperties.CpItsMyScp.IsActive ?? false;
    }

    internal static void ApplyReservations(int targetScpNumber, List<ReferenceHub> players)
    {
        if (!IsAssigning || targetScpNumber <= 0 || ReservedRoles.Count == 0)
            return;

        var roles = _enqueuedScps;

        if (players.Count != targetScpNumber || roles.Count != targetScpNumber)
            return;

        var reservations = ReservedRoles
            .Where(p => AvailableRoles.Contains(p.Value) && CanParticipate(p.Key) &&
                        RoleAssigner.CheckPlayer(p.Key.ReferenceHub))
            .ToList();

        // A shuffled prefix gives every reservation the same chance when there are fewer SCP slots.
        for (int i = reservations.Count - 1; i > 0; i--)
        {
            int index = Random.Range(0, i + 1);
            (reservations[i], reservations[index]) = (reservations[index], reservations[i]);
        }

        reservations = reservations.Take(targetScpNumber).ToList();

        if (reservations.Count == 0)
            return;

        var originalPlayers = players.ToArray();
        var originalRoles = roles.ToArray();

        try
        {
            roles.Clear();

            while (reservations.Count > 0)
            {
                int index = reservations.FindIndex(p => !roles.Contains(p.Value) && CanSpawn(p.Value, true));

                if (index >= 0)
                {
                    var reservation = reservations[index];
                    roles.Add(reservation.Value);
                    AssignedRoles.Add(reservation.Key.ReferenceHub, reservation.Value);
                    reservations.RemoveAt(index);
                    continue;
                }

                var fallback = NextAllowedScp();
                roles.Add(fallback);

                // Use an unreserved slot as an anchor for 079/096, if one is available.
                if (roles.Count + reservations.Count <= targetScpNumber)
                    continue;

                AssignedRoles.Add(reservations[0].Key.ReferenceHub, fallback);
                reservations.RemoveAt(0);
            }

            while (roles.Count < targetScpNumber)
                roles.Add(NextAllowedScp());

            foreach (var hub in AssignedRoles.Keys)
            {
                if (players.Contains(hub))
                    continue;

                int index = players.FindLastIndex(p => !AssignedRoles.ContainsKey(p));
                players[index] = hub;
            }
        }
        catch (Exception ex)
        {
            // Leave the original allocation intact if the game's role pool cannot satisfy the plan.
            players.Clear();
            players.AddRange(originalPlayers);
            roles.Clear();
            roles.AddRange(originalRoles);
            AssignedRoles.Clear();
            Log.Error($"ItsMyScp: не удалось применить бронирование SCP: {ex}");
        }
    }

    internal static List<ReferenceHub> FilterPlayers(List<ReferenceHub> players, RoleTypeId role)
    {
        if (!IsAssigning || AssignedRoles.Count == 0)
            return null;

        var reservedPlayer = players.FirstOrDefault(p => AssignedRoles.TryGetValue(p, out var reserved) && reserved == role);
        var excludedPlayers = players
            .Where(p => reservedPlayer != null ? p != reservedPlayer : AssignedRoles.ContainsKey(p))
            .ToList();

        foreach (var hub in excludedPlayers)
            players.Remove(hub);

        return excludedPlayers;
    }

    private static bool CanParticipate(Player player)
    {
        return player != null && player.IsConnected && !player.IsNPC && !player.IsOverwatchEnabled &&
               !ScpPlayerPicker.IsOptedOutOfScp(player.ReferenceHub);
    }

    private static bool CanSpawn(RoleTypeId role, bool isReserved)
    {
        if (!PlayerRoleLoader.AllRoles.TryGetValue(role, out var prefab) || prefab is not ISpawnableScp spawnable)
            return false;

        // A reservation bypasses only 3114's seasonal restriction.
        return (isReserved && role == RoleTypeId.Scp3114) || spawnable.GetSpawnChance(_enqueuedScps) > 0;
    }

    private static RoleTypeId NextAllowedScp()
    {
        var role = GetNextScp();

        if (CanSpawn(role, false))
            return role;

        // The vanilla overflow fallback ignores GetSpawnChance; keep composition restrictions here too.
        var candidates = PlayerRoleLoader.AllRoles.Values
            .Where(p => p is ISpawnableScp { AllowFallback: true } && CanSpawn(p.RoleTypeId, false))
            .Select(p => p.RoleTypeId)
            .ToList();

        int minCount = candidates.Min(r => _enqueuedScps.Count(p => p == r));
        candidates = candidates.Where(r => _enqueuedScps.Count(p => p == r) == minCount).ToList();
        return candidates[Random.Range(0, candidates.Count)];
    }

    public static void ClearReservations()
    {
        ReservedRoles.Clear();
        AssignedRoles.Clear();
        _enqueuedScps = null;
        IsAssigning = false;
    }

    public void OnPlayerLeft(LeftEventArgs ev) => ReservedRoles.Remove(ev.Player);

    public void OnRoundEnded(RoundEndedEventArgs ev) => ClearReservations();

    public void OnRestartingRound() => ClearReservations();

    public void OnWaitingForPlayers() => ClearReservations();
}
