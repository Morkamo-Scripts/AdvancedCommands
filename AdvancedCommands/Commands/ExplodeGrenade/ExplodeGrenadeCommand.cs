using System;
using AdvancedCommands.Components;
using AdvancedCommands.Components.Extensions;
using CommandSystem;
using Exiled.API.Features.Items;
using Exiled.Permissions.Extensions;
using LabApi.Features.Wrappers;
using Utils;
using Item = Exiled.API.Features.Items.Item;
using Player = Exiled.API.Features.Player;

namespace AdvancedCommands.Commands.ExplodeGrenade;

[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class ExplodeGrenadeCommand : ICommand, IAdvancedCommand
{
    public bool IsActive => Plugin.Instance?.Config.CommandProperties.CpExplodeGrenade.IsActive ?? false;

    public string Command { get; } = "explodeGrenade";
    public string[] Aliases { get; } = { "exg", "exgrenade" };
    public string Description { get; } = "Создаёт взрыв!";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!IsActive)
        {
            response = "Команда отключена сервером";
            return false;
        }

        if (!sender.CheckPermission("ac.exgrenade"))
        {
            string requestPermission = "Требуется разрешение - 'ac.exgrenade'";
            
            if (InstanceConfig().Debug)
                response = $"<color=red>Вы не имеете права использовать данную команду!</color>\n" +
                           $"<color=orange>[{requestPermission}]</color>";
            else
                response = "<color=red>Вы не имеете права использовать данную команду!</color>";
            
            return false;
        }
        
        if (arguments.Count < 2)
        {
            response = "<color=orange>Формат ввода: exgrenade [playerId] [explodeType (flash/frag/ghost)]</color>";
            return false;
        }

        if (!short.TryParse(arguments.At(0), out var id) ||
            (arguments.At(1).ToLower() != "flash" &&
            arguments.At(1).ToLower() != "frag" &&
            arguments.At(1).ToLower() != "ghost"))
        {
            response = "<color=orange>Формат ввода: exgrenade [playerId] [explodeType (flash/frag/ghost)]</color>";
            return false;
        }
        
        var target = Player.Get((ushort)id);

        if (target == null)
        {
            response = "<color=orange>Игрок не найден!</color>";
            return false;
        }

        if (arguments.At(1).ToLower() == "frag")
            ExplosionUtils.ServerExplode(target.Position, sender.AsPlayer().Footprint, ExplosionType.Grenade);
        else if (arguments.At(1).ToLower() == "flash")
        {
            FlashGrenade flash = (FlashGrenade)Item.Create(ItemType.GrenadeFlash, sender.AsPlayer());
            flash.FuseTime = 0f;
            flash.SpawnActive(target.Position);
        }
        else if (arguments.At(1).ToLower() == "ghost")
        {
            Scp2176 ghost = (Scp2176)Item.Create(ItemType.SCP2176, sender.AsPlayer());
            ghost.FuseTime = 0;
            ghost.SpawnActive(target.Position, sender.AsPlayer());
        }
        
        response = $"<color=green>Успешно!</color>";
        return true;
    }
    
    private static Config InstanceConfig()
    {
        return Plugin.Instance.Config;
    }
}