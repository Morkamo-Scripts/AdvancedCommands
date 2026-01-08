using System;
using AdvancedCommands.Components.Extensions;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using UnityEngine;

namespace AdvancedCommands.Commands.DisableScp2536;

[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class DisableScp2536Command : ICommand
{
    public string Command { get; } = "disable2536";
    public string[] Aliases { get; } = [];
    public string Description { get; } = "Включает и выключает возможность появления SCP-2536 в комплексе.";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission("ac.disable2536"))
        {
            string requestPermission = "Требуется разрешение - 'ac.disable2536'";
            
            if (InstanceConfig().Debug)
                response = $"<color=red>Вы не имеете права использовать данную команду!</color>\n" +
                           $"<color=orange>[{requestPermission}]</color>";
            else
                response = "<color=red>Вы не имеете права использовать данную команду!</color>";
            
            return false;
        }
        
        Disable2536Handler.IsEnabledSpawn = !Disable2536Handler.IsEnabledSpawn;
        string converted = Disable2536Handler.IsEnabledSpawn ? "включен" : "отключен";
        response = $"<color=green>SCP-2536 {converted}!</color>";
        return true;
    }
    
    private static Config InstanceConfig()
    {
        return Plugin.Instance.Config;
    }
}