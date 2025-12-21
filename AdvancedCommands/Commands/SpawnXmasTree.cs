using System;
using AdvancedCommands.Components.Extensions;
using Christmas.Scp2536;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using InventorySystem.Items;
using Mirror;
using UnityEngine;

namespace AdvancedCommands.Commands;

/*[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class SpawnXmasTree : ICommand
{
    public string Command { get; } = "spawnxmastree";
    public string[] Aliases { get; } = { "sxt", "spawnxtree", "xmasstree", "xtree" };
    public string Description { get; } = "Призывает рядом с игроком рожденственнскую ёлку.";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission("ac.spawnxmastree"))
        {
            string requestPermission = "Требуется разрешение - 'ac.spawnxmastree'";
            
            if (InstanceConfig().Debug)
                response = $"<color=red>Вы не имеете права использовать данную команду!</color>\n" +
                           $"<color=orange>[{requestPermission}]</color>";
            else
                response = "<color=red>Вы не имеете права использовать данную команду!</color>";
            
            return false;
        }
        
        if (arguments.Count < 1)
        {
            response = "<color=orange>Некорректный ввод! Формат ввода: xmasstree [playerId]</color>";
            return false;
        }
        
        if (!ushort.TryParse(arguments.At(0), out var id) || !Player.TryGet(id, out var target))
        {
            response = "<color=orange>Не удалось найти игрока с таким ID!</color>";
            return false;
        }
            
        response = $"<color=green>Рождественская ёлка успешно призвана к игроку {target.Nickname}!</color>";
        return true;
    }
    
    private static Config InstanceConfig()
    {
        return Plugin.Instance.Config;
    }
}*/