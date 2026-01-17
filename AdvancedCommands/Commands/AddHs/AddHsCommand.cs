using System;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;

namespace AdvancedCommands.Commands.AddHS;

[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class AddHsCommand : ICommand
{
    public string Command { get; } = "addhs";
    public string[] Aliases { get; } = [];
    public string Description { get; } = "Добавляет игроку HS защиту!";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission("ac.addhs"))
        {
            string requestPermission = "Требуется разрешение - 'ac.addhs'";
            
            if (InstanceConfig().Debug)
                response = $"<color=red>Вы не имеете права использовать данную команду!</color>\n" +
                           $"<color=orange>[{requestPermission}]</color>";
            else
                response = "<color=red>Вы не имеете права использовать данную команду!</color>";
            
            return false;
        }
        
        if (arguments.Count < 2)
        {
            response = "<color=orange>Формат ввода: addhs [playerId] [value]</color>";
            return false;
        }

        if (!short.TryParse(arguments.At(0), out var id) ||
            !float.TryParse(arguments.At(1), out var value))
        {
            response = "<color=orange>Формат ввода: addhs [playerId] [value]</color>";
            return false;
        }
        
        var target = Player.Get((ushort)id);

        if (target == null)
        {
            response = "<color=orange>Игрок не найден!</color>";
            return false;
        }

        target.HumeShield += value;
        response = $"<color=green>Успешно!</color>";
        return true;
    }
    
    private static Config InstanceConfig()
    {
        return Plugin.Instance.Config;
    }
}