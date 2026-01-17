using System;
using AdvancedCommands.Components.Extensions;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;

namespace AdvancedCommands.Commands.InfinityJailbird;

[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class InfinityJailbirdCommand : ICommand
{
    public string Command { get; } = "infinityJailbird";
    public string[] Aliases { get; } = { "infjb" };
    public string Description { get; } = "Включает или отключает режим не ломающегося Jailbird у игрока!";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission("ac.infjb"))
        {
            string requestPermission = "Требуется разрешение - 'ac.infjb'";
            
            if (InstanceConfig().Debug)
                response = $"<color=red>Вы не имеете права использовать данную команду!</color>\n" +
                           $"<color=orange>[{requestPermission}]</color>";
            else
                response = "<color=red>Вы не имеете права использовать данную команду!</color>";
            
            return false;
        }
        
        if (arguments.Count < 2)
        {
            response = "<color=orange>Формат ввода: infjb [playerId] [1/0]. (Пример: infjb 25 1)</color>";
            return false;
        }
        
        if (!ushort.TryParse(arguments.At(0), out var id))
        {
            response = "<color=orange>Некорректное значение ID игрока!</color>";
            return false;
        }

        if (!byte.TryParse(arguments.At(1), out var state) || state != 1 && state != 0)
        {
            response = "<color=orange>Некорректное значение состояния. " +
                       "Введите значение 1 если хотите включить режим или 0 чтобы выключить.</color>";
            return false;
        }

        var target = Player.Get(id);

        if (target == null)
        {
            response = "<color=orange>Игрок не найден!</color>";
            return false;
        }
        
        bool isEnabled = state == 1;

        target.AdvancedCommand()?.PlayerProperties.IsInfinityJailbird = isEnabled;
        
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