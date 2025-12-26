using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using PlayerRoles;

namespace AdvancedCommands.Commands.RandomPlayerSelection;

[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class RandomPlayerSelectionCommand : ICommand
{
    public string Command { get; } = "RandomPlayerSelection";
    public string[] Aliases { get; } = ["rps", "rpsel"];
    public string Description { get; } =
        "<color=orange>Случайно выбирает игроков по заданным параметрам.</color>\n" +
        "<color=red>Пример: <b>rps 3 alive</b></color>";

    private static readonly Dictionary<string, string> SortTypeInfo =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["all"] = "Все игроки",
            ["alive"] = "Только живые",
            ["notAlive"] = "Только мёртвые (наблюдатели и Overwatch)",
            ["human"] = "Только люди",
            ["scp"] = "Только SCP",
            ["specOnly"] = "Только наблюдатели",
            ["overOnly"] = "Только Overwatch"
        };

    private static readonly HashSet<string> SortTypes =
        new(SortTypeInfo.Keys, StringComparer.OrdinalIgnoreCase);

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission("ac.randomPlayerSelection"))
        {
            string requestPermission = "Требуется разрешение - 'ac.randomPlayerSelection'";
            
            if (Plugin.Instance.Config.Debug)
                response = $"<color=red>Вы не имеете права использовать данную команду!</color>\n" +
                           $"<color=orange>[{requestPermission}]</color>";
            else
                response = "<color=red>Вы не имеете права использовать данную команду!</color>";
            
            return false;
        }

        if (arguments.Count < 1 || !ushort.TryParse(arguments.At(0), out var requestedCount) || arguments.Count > 2)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<color=orange>Формат: rps [кол-во] [тип]");
            sb.AppendLine("(по умолчанию: <color=red>all</color>)</color>");
            sb.AppendLine("Доступные типы сортировки игроков:");

            foreach (var (key, info) in SortTypeInfo)
                sb.AppendLine($"- {key} — {info}");

            response = sb.ToString();
            return false;
        }

        var senderPlayer = Player.Get(sender);
        var sortType = arguments.Count == 2
            ? arguments.At(1).ToLowerInvariant()
            : "all";

        if (!SortTypes.Contains(sortType))
        {
            var sb = new StringBuilder();
            sb.AppendLine("<color=orange>Некорректный тип сортировки.</color>\nДоступные типы сортировки игроков:");

            foreach (var (key, info) in SortTypeInfo)
                sb.AppendLine($"- {key} — {info}");

            response = sb.ToString();
            return false;
        }

        var pool = Player.List
            .Where(p => !p.IsNPC && p != senderPlayer)
            .Where(p => sortType switch
            {
                "alive"    => p.IsAlive,
                "notalive" => !p.IsAlive,
                "human"    => p.IsHuman,
                "scp"      => p.IsScp,
                "speconly" => p.Role == RoleTypeId.Spectator,
                "overonly" => p.IsOverwatchEnabled,
                _          => true
            }).ToList();

        if (requestedCount < 1)
        {
            response = "<color=orange>Количество игроков должно быть больше 0.</color>";
            return false;
        }

        if (pool.Count == 0)
        {
            response = "<color=orange>Нет доступных игроков для выбора.</color>";
            return false;
        }

        if (requestedCount > pool.Count)
        {
            response = $"<color=orange>Недостаточно доступных игроков. Доступно: {pool.Count}.</color>";
            return false;
        }
        
        var selected = new HashSet<Player>();

        while (selected.Count < requestedCount)
            selected.Add(pool[new Random().Next(pool.Count)]);

        var result = new StringBuilder();
        
        result.AppendLine("<color=green>Набор успешно составлен!</color>");
        result.AppendLine($"Тип: {sortType} — {SortTypeInfo[sortType]}");
        result.AppendLine("Игроки:");

        foreach (var player in selected)
            result.AppendLine($"- [ID: {player.Id}] {player.Nickname}");

        response = result.ToString();
        return true;
    }
}