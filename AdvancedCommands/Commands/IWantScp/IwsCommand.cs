using System;
using AdvancedCommands.Components.Extensions;
using CommandSystem;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using PlayerRoles;

namespace AdvancedCommands.Commands.IWantScp;

[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class IwsCommand : ICommand
{
    public string Command { get; } = "IWantScp";
    public string[] Aliases { get; } = { "iws" };
    public string Description { get; } = "Добавляет вас в список участников лотерей на замену вышедшего SCP.";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!Plugin.Instance.IwsHandler.IsLotteryProcessing)
        {
            response = "<color=orange>Сейчас лотерея не проводится!</color>";
            return false;
        }
        
        if (sender.AsPlayer().IsScp)
        {
            response = "<color=orange>SCP не могут быть участниками лотереи!</color>";
            return false;
        }

        if (sender.AsPlayer().IsOverwatchEnabled)
        {
            response = "<color=orange>Игроки в режиме надзирателя не могут быть участниками лотереи!</color>";
            return false;
        }

        if (Plugin.Instance.IwsHandler.IsLotteryClosed)
        {
            response = "<color=orange>Лотерея окончена! Идёт подсчёт тикетов...</color>";
            return false;
        }

        var members = Plugin.Instance.IwsHandler.LotteryMembers;

        if (members.Contains(sender.AsPlayer()))
        {
            response = "<color=orange>Вы уже учавствуете в лотерее!</color>";
            return false;
        }
        
        members.Add(sender.AsPlayer());
        
        response = "<color=green>Вы добавлены в список участников лотереи!</color>";
        return true;
    }
    
    private static Config InstanceConfig()
    {
        return Plugin.Instance.Config;
    }
}