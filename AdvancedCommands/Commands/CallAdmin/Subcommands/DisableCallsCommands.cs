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
public class DisableCallsCommand : ICommand
{
    public string Command { get; } = "DisableCalls";
    public string[] Aliases { get; } = { "disCalls", "callsOf" };
    public string Description { get; } = "Отключает и включает возможность вызова администрации! (.calls)";

    public static bool IsCallsEnabled = true;
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (sender is not PlayerCommandSender)
        {
            response = "<color=orange>Команда доступна только игрокам!</color>";
            return false;
        }
        
        if (!sender.CheckPermission("ac.disableCalls"))
        {
            string requestPermission = "Требуется разрешение - 'ac.disableCalls'";
            
            if (InstanceConfig().Debug)
                response = $"<color=red>Вы не имеете права использовать данную команду!</color>\n" +
                           $"<color=orange>[{requestPermission}]</color>";
            else
                response = "<color=red>Вы не имеете права использовать данную команду!</color>";
            
            return false;
        }

        IsCallsEnabled = !IsCallsEnabled;
        response = $"<color=green>Вызовы {(IsCallsEnabled ? "включены" : "отключены")}!</color>";
        return true;
    }

    private static Config InstanceConfig()
    {
        return Plugin.Instance.Config;
    }
}