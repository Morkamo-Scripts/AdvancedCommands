using System;
using AdvancedCommands.Components.Extensions;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;

namespace AdvancedCommands.Commands.NonBreakableGlass;

[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class NonBreakableGlassCommand : ICommand
{
    public string Command { get; } = "nonBreakableGlass";
    public string[] Aliases { get; } = { "nbg" };
    public string Description { get; } = "Включает или отключает режим небьющегося стекла!";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission("ac.nbg"))
        {
            string requestPermission = "Требуется разрешение - 'ac.nbg'";
            
            if (InstanceConfig().Debug)
                response = $"<color=red>Вы не имеете права использовать данную команду!</color>\n" +
                           $"<color=orange>[{requestPermission}]</color>";
            else
                response = "<color=red>Вы не имеете права использовать данную команду!</color>";
            
            return false;
        }
        
        if (arguments.Count < 1)
        {
            response = "<color=orange>Формат ввода: nbg [1/0]. (Пример: nbg 1)</color>";
            return false;
        }
        
        if (!byte.TryParse(arguments.At(0), out var state) || state != 1 && state != 0)
        {
            response = "<color=orange>Некорректное значение состояния. " +
                       "Введите значение 1 если хотите включить режим или 0 чтобы выключить.</color>";
            return false;
        }
        
        bool isEnabled = state == 1;

        NonBreakableGlassHandler.IsEnabled = isEnabled;
        
        response = isEnabled ? 
            "<color=green>Успешно включено!</color>" 
            : 
            "<color=green>Успешно отключено!</color>";
        
        return true;
    }
    
    private static Config InstanceConfig()
    {
        return Plugin.Instance.Config;
    }
}