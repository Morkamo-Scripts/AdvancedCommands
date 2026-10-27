using System;
using AdvancedCommands.Components;
using AdvancedCommands.Components.Extensions;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using Utils;

namespace AdvancedCommands.Commands.ExplodeSimulate;

[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class ExplodeSimulateCommand : ICommand, IAdvancedCommand
{
    public bool IsActive => Plugin.Instance?.Config.CommandProperties.CpExplodeSimulate.IsActive ?? false;

    public string Command { get; } = "explodeSimulate";
    public string[] Aliases { get; } = { "exs", "exsim" };
    public string Description { get; } = "Создаёт эффект взрыва";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!IsActive)
        {
            response = "Команда отключена сервером";
            return false;
        }

        if (!sender.CheckPermission("ac.exsim"))
        {
            string requestPermission = "Требуется разрешение - 'ac.exsim'";
            
            if (InstanceConfig().Debug)
                response = $"<color=red>Вы не имеете права использовать данную команду!</color>\n" +
                           $"<color=orange>[{requestPermission}]</color>";
            else
                response = "<color=red>Вы не имеете права использовать данную команду!</color>";
            
            return false;
        }
        
        if (arguments.Count < 2)
        {
            response = "<color=orange>Формат ввода: exsim [playerId] [explodeType (flash/frag/ghost)]</color>";
            return false;
        }

        if (!short.TryParse(arguments.At(0), out var id) ||
            (arguments.At(1).ToLower() != "flash" &&
             arguments.At(1).ToLower() != "frag" &&
             arguments.At(1).ToLower() != "ghost"))
        {
            response = "<color=orange>Формат ввода: exsim [playerId] [explodeType (flash/frag/ghost)]</color>";
            return false;
        }
        
        var target = Player.Get((ushort)id);

        if (target == null)
        {
            response = "<color=orange>Игрок не найден!</color>";
            return false;
        }

        ItemType explodeType;

        switch (arguments.At(1).ToLower())
        {
            case "flash":
                explodeType = ItemType.GrenadeFlash;
                break;
            case "frag":
                explodeType = ItemType.GrenadeHE;
                break;
            case "ghost":
                explodeType = ItemType.SCP2176;
                break;
            default:
                explodeType = ItemType.GrenadeHE;
                break;
        }
        
        ExplosionUtils.ServerSpawnEffect(target.Position, explodeType);
        response = $"<color=green>Успешно!</color>";
        return true;
    }
    
    private static Config InstanceConfig()
    {
        return Plugin.Instance.Config;
    }
}