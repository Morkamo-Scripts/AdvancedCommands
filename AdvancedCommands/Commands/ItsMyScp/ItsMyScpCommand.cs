using System;
using AdvancedCommands.Components;
using AdvancedCommands.Components.Extensions;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using PlayerRoles;
using PlayerRoles.RoleAssign;

namespace AdvancedCommands.Commands.ItsMyScp;

[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class ItsMyScpCommand : ICommand, IAdvancedCommand
{
    public bool IsActive => Plugin.Instance?.Config.CommandProperties.CpItsMyScp.IsActive ?? false;

    public string Command { get; } = "itsMyScp";
    public string[] Aliases { get; } = { "ims" };
    public string Description { get; } = "Бронирует указанную роль SCP для выбранного игрока!";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!IsActive)
        {
            response = "Команда отключена сервером";
            return false;
        }

        if (!sender.CheckPermission("ac.itsMyScp"))
        {
            string requestPermission = "Требуется разрешение - 'ac.itsMyScp'";
            
            if (InstanceConfig().Debug)
                response = $"<color=red>Вы не имеете права использовать данную команду!</color>\n" +
                           $"<color=orange>[{requestPermission}]</color>";
            else
                response = "<color=red>Вы не имеете права использовать данную команду!</color>";
            
            return false;
        }

        if (Round.IsStarted || ItsMyScpHandler.IsAssigning)
        {
            response = "<color=orange>Команда не может быть использована после начала раунда!</color>";
            return false;
        }
        
        if (arguments.Count == 1 && arguments.At(0) == "0")
        {
            var player = sender.AsPlayer();

            if (player == null)
            {
                response = "<color=orange>Для отмены брони из консоли используйте: ims [playerId] 0</color>";
                return false;
            }

            return CancelReservation(player, out response);
        }

        if (arguments.Count != 2)
        {
            response = "<color=orange>Формат ввода: ims [playerId] [roleNum]\n" +
                       "Отмена своей брони: ims 0\nОтмена брони игрока: ims [playerId] 0</color>";
            return false;
        }
        
        if (!ushort.TryParse(arguments.At(0), out var id) || id == 0)
        {
            response = "<color=orange>Некорректное значение у ID игрока! (Пример: 1, 2, 3...)</color>";
            return false;
        }
        
        if (!short.TryParse(arguments.At(1), out var roleNum))
        {
            response = "<color=orange>Некорректное значение у номера роли SCP! (Пример: 173, 939, 079...)</color>";
            return false;
        }
        
        var target = Player.Get(id);

        if (target == null || !target.IsConnected)
        {
            response = "<color=orange>Игрок не найден!</color>";
            return false;
        }

        if (roleNum == 0)
            return CancelReservation(target, out response);

        if (target.IsNPC || target.IsOverwatchEnabled)
        {
            response = "<color=orange>Нельзя забронировать SCP для NPC или игрока в режиме надзирателя!</color>";
            return false;
        }

        if (ScpPlayerPicker.IsOptedOutOfScp(target.ReferenceHub))
        {
            response = "<color=orange>Игрок отключил появление за SCP в настройках игры!</color>";
            return false;
        }
        
        RoleTypeId? role = null;

        switch (roleNum)
        {
            case 173:
            {
                role = RoleTypeId.Scp173;
                break;
            }
            case 106:
            {
                role = RoleTypeId.Scp106;
                break;
            }
            case 939:
            {
                role = RoleTypeId.Scp939;
                break;
            }
            case 049:
            {
                role = RoleTypeId.Scp049;
                break;
            }
            case 096:
            {
                role = RoleTypeId.Scp096;
                break;
            }
            case 079:
            {
                role = RoleTypeId.Scp079;
                break;
            }
            case 3114:
            {
                role = RoleTypeId.Scp3114;
                break;
            }
        }

        if (role == null || !ItsMyScpHandler.AvailableRoles.Contains(role.Value))
        {
            response = "<color=orange>Роль не найдена или недоступна для бронирования!</color>";
            return false;
        }

        if (ItsMyScpHandler.ReservedRoles.TryGetValue(target, out var reservedRole) && reservedRole == role.Value)
        {
            response = "<color=orange>Указанный игрок уже занял эту роль!</color>";
            return false;
        }

        if (ItsMyScpHandler.ReservedRoles.ContainsValue(role.Value))
        {
            response = "<color=orange>Роль уже занята другим игроком!</color>";
            return false;
        }

        ItsMyScpHandler.ReservedRoles[target] = role.Value;
        response = $"<color=green>Роль SCP-{roleNum} забронирована для игрока {target.Nickname}! " +
                   "При нехватке мест SCP проводится случайный выбор среди участников бронирования.</color>";
        return true;
    }

    private static bool CancelReservation(Player player, out string response)
    {
        if (!ItsMyScpHandler.ReservedRoles.Remove(player))
        {
            response = "<color=orange>Указанный игрок не бронировал роль SCP!</color>";
            return false;
        }

        response = "<color=green>Бронирование роли SCP отменено!</color>";
        return true;
    }
    
    private static Config InstanceConfig()
    {
        return Plugin.Instance.Config;
    }
}
