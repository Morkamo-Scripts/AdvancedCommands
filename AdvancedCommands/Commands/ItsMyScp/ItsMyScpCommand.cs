using System;
using System.Linq;
using AdvancedCommands.Components.Extensions;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using PlayerRoles;

namespace AdvancedCommands.Commands.ItsMyScp;

[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class ItsMyScpCommand : ICommand
{
    public string Command { get; } = "itsMyScp";
    public string[] Aliases { get; } = { "ims" };
    public string Description { get; } = "Бронирует указанную роль SCP для выбранного игрока!";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
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

        if (Round.IsStarted)
        {
            response = "<color=orange>Команда не может быть использована после начала раунда!</color>";
            return false;
        }
        
        if (arguments.Count < 2)
        {
            response = "<color=orange>Формат ввода: ims [playerId] [roleNum]</color>";
            return false;
        }
        
        if (!short.TryParse(arguments.At(0), out var id))
        {
            response = "<color=orange>Некорректное значение у ID игрока! (Пример: 1, 2, 3...)</color>";
            return false;
        }
        
        if (!short.TryParse(arguments.At(1), out var roleNum))
        {
            response = "<color=orange>Некорректное значение у номера роли SCP! (Пример: 173, 939, 079...)</color>";
            return false;
        }
        
        var target = Player.Get((ushort)id);

        if (target == null)
        {
            response = "<color=orange>Игрок не найден!</color>";
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

        if (role == null)
        {
            response = $"<color=orange>Роль не найдена!</color>";
            return false;
        }

        if (ItsMyScpHandler.ReservedRoles.TryGetValue((RoleTypeId)role, out var player))
        {
            if (player == target)
            {
                response = $"<color=orange>Указанный игрок уже занял эту роль!</color>";
                return false;
            }
            
            response = $"<color=orange>Роль уже занята другим игроком!</color>";
            return false;
        }
        
        var keysToRemove = ItsMyScpHandler.ReservedRoles
            .Where(p => p.Value == target)
            .Select(p => p.Key)
            .ToList();
        
        foreach (var key in keysToRemove)
        {
            ItsMyScpHandler.ReservedRoles.Remove(key);
            target.AdvancedCommand().PlayerProperties.ReservedRole = null;
        }
        
        ItsMyScpHandler.ReservedRoles.Add((RoleTypeId)role, target);
        target.AdvancedCommand().PlayerProperties.ReservedRole = role;
        response = $"<color=green>Успешно!</color>";
        return true;
    }
    
    private static Config InstanceConfig()
    {
        return Plugin.Instance.Config;
    }
}