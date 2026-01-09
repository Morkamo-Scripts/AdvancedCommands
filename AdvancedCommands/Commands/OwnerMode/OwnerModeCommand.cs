using System;
using AdvancedCommands.Components.Extensions;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using RemoteAdmin;
using UnityEngine;

namespace AdvancedCommands.Commands.OwnerMode;

[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class OwnerModeCommand : ICommand
{
    public string Command { get; } = "OwnerMode";
    public string[] Aliases { get; } = { "ownmode" };
    public string Description { get; } = "Включает и выключает игроку 'OwnerMode'.";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (sender is not PlayerCommandSender)
        {
            response = "<color=red>Команда доступна только игрокам!</color>";
            return false;
        }
        
        if (!sender.CheckPermission("ac.ownerMode"))
        {
            string requestPermission = "Требуется разрешение - 'ac.ownerMode'";
            
            if (InstanceConfig().Debug)
                response = $"<color=red>Вы не имеете права использовать данную команду!</color>\n" +
                           $"<color=orange>[{requestPermission}]</color>";
            else
                response = "<color=red>Вы не имеете права использовать данную команду!</color>";
            
            return false;
        }
        
        var props = sender.AsPlayer().AdvancedCommand().PlayerProperties;
        bool result = !props.IsOwnerModeEnabled;;
        props.IsOwnerModeEnabled = result;
        
        Plugin.Instance.OwnerModeHandler.OnOwnerModeStatusChanged(sender.AsPlayer(), result);

        response = $"OwnerMode {(result ? "включен" : "выключен")}";
        return true;
    }
    
    private static Config InstanceConfig()
    {
        return Plugin.Instance.Config;
    }
}