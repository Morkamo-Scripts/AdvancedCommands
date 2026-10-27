using System;
using AdvancedCommands.Components;
using AdvancedCommands.Components.Extensions;
using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using UnityEngine;

namespace AdvancedCommands.Commands.MiyabiMode;

[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class MiyabiModeCommand : ICommand, IAdvancedCommand
{
    public bool IsActive => Plugin.Instance?.Config.CommandProperties.CpMiyabimode.IsActive ?? false;

    public string Command { get; } = "miyabimode";
    public string[] Aliases { get; } = { "mimode" };
    public string Description { get; } = "Меняет видимую модель игрока на модель MiyabiToy!";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!IsActive)
        {
            response = "Команда отключена сервером";
            return false;
        }

        if (!sender.CheckPermission("ac.mimode"))
        {
            string requestPermission = "Требуется разрешение - 'ac.mimode'";
            
            if (InstanceConfig().Debug)
                response = $"<color=red>Вы не имеете права использовать данную команду!</color>\n" +
                           $"<color=orange>[{requestPermission}]</color>";
            else
                response = "<color=red>Вы не имеете права использовать данную команду!</color>";
            
            return false;
        }
        
        if (arguments.Count < 2)
        {
            response = "<color=orange>Формат ввода: mimode [playerId] [1/0]</color>";
            return false;
        }

        if (!short.TryParse(arguments.At(0), out var id) ||
            !short.TryParse(arguments.At(1), out var value) || (value != 0 && value != 1))
        {
            response = "<color=orange>Формат ввода: mimode [playerId] [1/0]</color>";
            return false;
        }
        
        var target = Player.Get((ushort)id);

        if (target == null || target.IsNPC || !target.IsAlive)
        {
            response = "<color=orange>Игрок не найден!</color>";
            return false;
        }

        var pp = target.AdvancedCommand().PlayerProperties;
        
        if (value == 1)
        {
            if (pp.IsMiyabiModeEnabled)
            {
                response = "<color=orange>Режим уже включен!</color>";
                return false;
            }
            
            target.EnableEffect(EffectType.Fade, 255);
            
            var cam = target.CameraTransform;

            Vector3 spawnPos = (cam.position - new Vector3(0, 0.65f, 0)) + cam.forward * -0.01f;
            Quaternion rotation = Quaternion.LookRotation(cam.forward);

            var miyabiToy = ProjectMER.Features.ObjectSpawner.SpawnSchematic("MiyabiToyStatic", spawnPos, rotation);
            miyabiToy.Scale = new Vector3(2.2f, 2.2f, 2.2f);
            miyabiToy.transform.SetParent(target.Transform);
            
            pp.MiyabiModel = miyabiToy.gameObject;
            pp.IsMiyabiModeEnabled = true;
        }
        else
        {
            target.DisableEffect(EffectType.Fade);
            UnityEngine.Object.Destroy(pp.MiyabiModel);
            pp.MiyabiModel = null;
            pp.IsMiyabiModeEnabled = false;
        }
        
        response = $"<color=green>Успешно!</color>";
        return true;
    }
    
    private static Config InstanceConfig()
    {
        return Plugin.Instance.Config;
    }
}