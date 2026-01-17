using System;
using CommandSystem;
using Exiled.Permissions.Extensions;

namespace AdvancedCommands.Commands.GlobalInfinityRadio;

[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class GlobalInfinityRadio : ICommand
{
    public string Command { get; } = "globalInfinityRadio";
    public string[] Aliases { get; } = { "gir" };
    public string Description { get; } = "Включает или отключает режим не разряжающегося радио (рации)!";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission("ac.gir"))
        {
            string requestPermission = "Требуется разрешение - 'ac.gir'";
            
            if (InstanceConfig().Debug)
                response = $"<color=red>Вы не имеете права использовать данную команду!</color>\n" +
                           $"<color=orange>[{requestPermission}]</color>";
            else
                response = "<color=red>Вы не имеете права использовать данную команду!</color>";
            
            return false;
        }
        
        if (arguments.Count < 1)
        {
            response = "<color=orange>Формат ввода: gir [1/0]. (Пример: gir 1)</color>";
            return false;
        }
        
        if (!byte.TryParse(arguments.At(0), out var state) || state != 1 && state != 0)
        {
            response = "<color=orange>Некорректное значение состояния. " +
                       "Введите значение 1 если хотите включить режим или 0 чтобы выключить.</color>";
            return false;
        }
        
        bool isEnabled = state == 1;

        GlobalInfinityRadioHandler.IsEnabled = isEnabled;
        GlobalInfinityRadioHandler.IsFirstUsing = true;
        
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