using System;
using AdvancedCommands.Components.Extensions;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using UnityEngine;

namespace AdvancedCommands.Commands.PlayerSize;

[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class PlayerSizeCommand : ICommand
{
    public string Command { get; } = "playerSize";
    public string[] Aliases { get; } = { "pls", "plsize" };
    public string Description { get; } = "Меняет размер игрока.";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission("ac.playerSize"))
        {
            string requestPermission = "Требуется разрешение - 'ac.playerSize'";
            
            if (InstanceConfig().Debug)
                response = $"<color=red>Вы не имеете права использовать данную команду!</color>\n" +
                           $"<color=orange>[{requestPermission}]</color>";
            else
                response = "<color=red>Вы не имеете права использовать данную команду!</color>";
            
            return false;
        }
        
        if (arguments.Count < 3)
        {
            response = "<color=orange>Некорректный воод!\n" +
                       "Формат ввода для другого игрока: ps [id] [X, Y, Z].\n" +
                       "Формат ввода для себя: ps [X, Y, Z].</color>";
            return false;
        }
        
        if (arguments.Count == 3)
        {
            if (!float.TryParse(arguments.At(0), out var x) ||
                !float.TryParse(arguments.At(1), out var y) ||
                !float.TryParse(arguments.At(2), out var z))
            {
                response = "<color=orange>Параметры масштаба введены неверно. Пример: 1.2 1.3 1.4</color>";
                return false;
            }
            
            sender.AsPlayer().Scale = new Vector3(x,y,z);
            response = $"<color=green>Ваш размер успешно изменён! [X: {x}, Y: {y}, Z: {z}]</color>";
            return true;
        }
        else
        {
            if (!ushort.TryParse(arguments.At(0), out var id) || !Player.TryGet(id, out var target))
            {
                response = "<color=orange>Не удалось найти игрока с таким ID!</color>";
                return false;
            }
            
            if (!float.TryParse(arguments.At(1), out var x) ||
                !float.TryParse(arguments.At(2), out var y) ||
                !float.TryParse(arguments.At(3), out var z))
            {
                response = "<color=orange>Параметры масштаба введены неверно. Пример: 1.2 1.3 1.4</color>";
                return false;
            }
            
            target.Scale = new Vector3(x,y,z);
            response = $"<color=green>Размер игрока {target.Nickname} успешно изменён! [X: {x}, Y: {y}, Z: {z}]</color>";
            return true;
        }
    }
    
    private static Config InstanceConfig()
    {
        return Plugin.Instance.Config;
    }
}