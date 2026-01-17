using System;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;

namespace AdvancedCommands.Commands.SerpentHandsWave;

[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class SerpentHandsWaveCommand : ICommand
{
    public string Command { get; } = "serpentHandsWave";
    public string[] Aliases { get; } = { "shw" };
    public string Description { get; } = "Устанавливает или отменяет приезд отряда Длани Змемя в следующей волне.";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission("ac.shw"))
        {
            string requestPermission = "Требуется разрешение - 'ac.shw'";
            
            if (InstanceConfig().Debug)
                response = $"<color=red>Вы не имеете права использовать данную команду!</color>\n" +
                           $"<color=orange>[{requestPermission}]</color>";
            else
                response = "<color=red>Вы не имеете права использовать данную команду!</color>";
            
            return false;
        }
        
        if (arguments.Count < 1 || (arguments.At(0).ToLower() != "force" && arguments.At(0).ToLower() != "cancel"))
        {
            response = "<color=orange>Формат ввода: shw [force/cancel]</color>";
            return false;
        }

        switch (arguments.At(0).ToLower())
        {
            case "force":
            {
                if (SerpentHandsWaveHandler.IsNextWaveForSerpentHands)
                {
                    response = "<color=orange>Отряд уже прибывает!</color>";
                    return false;
                }

                SerpentHandsWaveHandler.IsNextWaveForSerpentHands = true;
                response = $"<color=green>Успешно!</color>";
                return true;
            }
            case "cancel":
            {
                if (!SerpentHandsWaveHandler.IsNextWaveForSerpentHands)
                {
                    response = "<color=orange>Отряд и так не прибывает!</color>";
                    return false;
                }

                SerpentHandsWaveHandler.IsNextWaveForSerpentHands = false;
                response = $"<color=green>Успешно!</color>";
                return true;
            }
            default:
            {
                response = "<color=orange>Аргумент не распознан!</color>";
                return false;
            }
        }
    }
    
    private static Config InstanceConfig()
    {
        return Plugin.Instance.Config;
    }
}