using System;
using System.Linq;
using AdvancedCommands.Components.Extensions;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using RemoteAdmin;

namespace AdvancedCommands.Commands.CallAdmin.Subcommands;

[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(GameConsoleCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class ImSleepingCommand : ICommand
{
    public string Command { get; } = "isleep";
    public string[] Aliases { get; } = [];
    public string Description { get; } = "Переводит вас в спящий режим! " +
                                         "Вы не будите получать сообщения о вызовах администратора!";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (sender is not PlayerCommandSender)
        {
            response = "<color=orange>Команда доступна только игрокам!</color>";
            return false;
        }
        
        if (!sender.CheckPermission("ac.isleep"))
        {
            string requestPermission = "Требуется разрешение - 'ac.isleep'";
            
            if (InstanceConfig().Debug)
                response = $"<color=red>Вы не имеете права использовать данную команду!</color>\n" +
                           $"<color=orange>[{requestPermission}]</color>";
            else
                response = "<color=red>Вы не имеете права использовать данную команду!</color>";
            
            return false;
        }

        var props = sender.AsPlayer().AdvancedCommand().PlayerProperties;
        var newValue = !props.ImSleeping;
        props.ImSleeping = newValue;
        
        response = $"<color=green>Спящий режим {(newValue ? "включен" : "выключен")}!</color>";
        return true;
    }

    private static Config InstanceConfig()
    {
        return Plugin.Instance.Config;
    }
}