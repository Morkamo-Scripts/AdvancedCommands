using System;
using System.Linq;
using AdvancedCommands.Commands.CallAdmin.Subcommands;
using AdvancedCommands.Components.Extensions;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using RemoteAdmin;

namespace AdvancedCommands.Commands.CallAdmin;

[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(GameConsoleCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class CallAdminCommand : ICommand
{
    public string Command { get; } = "callAdmin";
    public string[] Aliases { get; } = { "call" };
    public string Description { get; } = "Вызывает администратора!";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (sender is not PlayerCommandSender)
        {
            response = "<color=orange>Команда доступна только игрокам!</color>";
            return false;
        }
        
        if (!sender.CheckPermission("ac.callAdmin"))
        {
            string requestPermission = "Требуется разрешение - 'ac.callAdmin'";
            
            if (InstanceConfig().Debug)
                response = $"<color=red>Вы не имеете права использовать данную команду!</color>\n" +
                           $"<color=orange>[{requestPermission}]</color>";
            else
                response = "<color=red>Вы не имеете права использовать данную команду!</color>";
            
            return false;
        }

        var cfg = Plugin.Instance.Config;
        if (!cfg.CallAdminSettings.AllowedServers.Contains(cfg.ServerIdentifier))
        {
            response = "<color=orange>Команда не разрешена на данном сервере!</color>";
            return false;
        }

        if (!DisableCallsCommand.IsCallsEnabled)
        {
            response = "<color=orange>Администратор отключил вызовы в этом раунде!</color>";
            return false;
        }
        
        var lastCall = sender.AsPlayer().AdvancedCommand().PlayerProperties.LastCall;

        if (lastCall.HasValue)
        {
            var timeRate = 90;
            var elapsed = DateTime.UtcNow - lastCall.Value;

            if (elapsed.TotalSeconds < timeRate)
            {
                response = $"\n<color=orange>Вы уже отправили сообщение {elapsed.TotalSeconds:F0} сек назад!\n" +
                           $"<color=orange>Это можно делать не чаще чем раз в {timeRate} секунд!</color>";
                return false;
            }
        }
        
        bool isNotFoundAnyone = true;
        
        foreach (var player in Player.List
                     .Where(pl => !pl.AdvancedCommand().PlayerProperties.ImSleeping && 
                                  Plugin.Instance.Config.CallAdminSettings.AdminGroups
                                      .Contains(pl.GroupName)))
        {
            if (isNotFoundAnyone) 
                isNotFoundAnyone = false;
            
            player.Broadcast(10, $"<b><color=#ffbc2b>" +
                                $"Игрок <color=#c72bff>[{sender.AsPlayer().Id}] {sender.AsPlayer().Nickname}</color>\n вызывает администратора!" +
                                $"</color></b>");
        }

        if (isNotFoundAnyone)
        {
            response = "<color=orange>Не удалось найти хотя бы одного администратора соответствующей группы, " +
                       "который мог бы получить ваше сообщение. Если администратор присутствует на сервере, " +
                       "попросите его отключить спящий режим!</color>";
            return false;
        } 
        
        sender.AsPlayer().AdvancedCommand().PlayerProperties.LastCall = DateTime.UtcNow;
        response = "<color=green>Ваше сообщение отправлено!</color>";
        return true;
    }

    private static Config InstanceConfig()
    {
        return Plugin.Instance.Config;
    }
}