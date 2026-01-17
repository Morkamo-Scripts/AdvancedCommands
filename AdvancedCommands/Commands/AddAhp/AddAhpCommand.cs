using System;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;

namespace AdvancedCommands.Commands.AddAhp;

[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class AddAhpCommand : ICommand
{
    public string Command { get; } = "addahp";
    public string[] Aliases { get; } = [];
    public string Description { get; } = "Добавляет игроку AHP защиту!";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission("ac.addahp"))
        {
            string requestPermission = "Требуется разрешение - 'ac.addahp'";
            
            if (InstanceConfig().Debug)
                response = $"<color=red>Вы не имеете права использовать данную команду!</color>\n" +
                           $"<color=orange>[{requestPermission}]</color>";
            else
                response = "<color=red>Вы не имеете права использовать данную команду!</color>";
            
            return false;
        }
        
        if (arguments.Count < 2)
        {
            response = "<color=orange>Формат ввода: addahp [playerId] [value]</color>";
            return false;
        }

        if (!short.TryParse(arguments.At(0), out var id) ||
            !float.TryParse(arguments.At(1), out var value))
        {
            response = "<color=orange>Формат ввода: addahp [playerId] [value]</color>";
            return false;
        }
        
        var target = Player.Get((ushort)id);

        if (target == null)
        {
            response = "<color=orange>Игрок не найден!</color>";
            return false;
        }

        target.AddAhp(value, 75f, 0);
        response = $"<color=green>Успешно!</color>";
        return true;
    }
    
    private static Config InstanceConfig()
    {
        return Plugin.Instance.Config;
    }
}