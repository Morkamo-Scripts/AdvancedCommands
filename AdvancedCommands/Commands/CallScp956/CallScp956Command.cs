using System;
using AdvancedCommands.Components.Extensions;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using UnityEngine;

namespace AdvancedCommands.Commands.CallScp956;

[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class CallScp956Command : ICommand
{
    public string Command { get; } = "call956";
    public string[] Aliases { get; } = [];
    public string Description { get; } = "Призывает SCP-956 за спину игрока!";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission("ac.call956"))
        {
            string requestPermission = "Требуется разрешение - 'ac.call956'";
            
            if (InstanceConfig().Debug)
                response = $"<color=red>Вы не имеете права использовать данную команду!</color>\n" +
                           $"<color=orange>[{requestPermission}]</color>";
            else
                response = "<color=red>Вы не имеете права использовать данную команду!</color>";
            
            return false;
        }
        
        if (arguments.Count < 1)
        {
            response = "<color=orange>Формат ввода: call956 [playerId]</color>";
            return false;
        }

        if (!short.TryParse(arguments.At(0), out var id))
        {
            response = "<color=orange>Некорректное значение ID игрока!</color>";
            return false;
        }
        
        if (id == -1)
        {
            Scp956.SpawnBehindTarget();
            response = $"<color=green>SCP-956 успешно призван к случайному игроку!</color>";
            return true;
        }
        
        var target = Player.Get((ushort)id);

        if (target == null)
        {
            response = "<color=orange>Игрок не найден!</color>";
            return false;
        }
        
        Scp956.SpawnBehindTarget(target);
        response = $"<color=green>SCP-956 успешно призван к игроку {target.Nickname}!</color>";
        return true;
    }
    
    private static Config InstanceConfig()
    {
        return Plugin.Instance.Config;
    }
}