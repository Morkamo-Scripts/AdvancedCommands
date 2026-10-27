using System;
using AdvancedCommands.Components;
using AdvancedCommands.Components.Extensions;
using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Exiled.Permissions.Extensions;
using PlayerRoles;
using PlayerRoles.FirstPersonControl.Thirdperson;
using UnityEngine;
using Object = System.Object;
using Random = System.Random;

namespace AdvancedCommands.Commands.CapybaraMode;

[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class CapybaraModeCommand : ICommand, IAdvancedCommand
{
    public bool IsActive => Plugin.Instance?.Config.CommandProperties.CpCapybaramode.IsActive ?? false;

    public string Command { get; } = "capybaramode";
    public string[] Aliases { get; } = { "cpmode", "cpm" };
    public string Description { get; } = "Меняет видимую модель игрока на модель капибары!";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!IsActive)
        {
            response = "Команда отключена сервером";
            return false;
        }

        if (!sender.CheckPermission("ac.capybaramode"))
        {
            string requestPermission = "Требуется разрешение - 'ac.capybaramode'";
            
            if (InstanceConfig().Debug)
                response = $"<color=red>Вы не имеете права использовать данную команду!</color>\n" +
                           $"<color=orange>[{requestPermission}]</color>";
            else
                response = "<color=red>Вы не имеете права использовать данную команду!</color>";
            
            return false;
        }
        
        if (arguments.Count < 2)
        {
            response = "<color=orange>Формат ввода: cpm [playerId] [1/0]</color>";
            return false;
        }

        if (!short.TryParse(arguments.At(0), out var id) ||
            !short.TryParse(arguments.At(1), out var value) || (value != 0 && value != 1))
        {
            response = "<color=orange>Формат ввода: cpm [playerId] [1/0]</color>";
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
            if (pp.IsCapybaraModeEnabled)
            {
                response = "<color=orange>Режим уже включен!</color>";
                return false;
            }
            
            target.EnableEffect(EffectType.Fade, 255);
            target.EnableEffect(EffectType.SilentWalk, 10);
            
            var cam = target.CameraTransform;

            Vector3 spawnPos = (cam.position - new Vector3(0, 1.55f, 0)) + cam.forward * 0.01f;
            Quaternion rotation = Quaternion.LookRotation(cam.forward);

            var capybara = PrefabHelper.Spawn(PrefabType.CapybaraToy, spawnPos, rotation);
            capybara.SetWorldScale(new Vector3(1f, 1f, 1f));
            capybara.transform.SetParent(target.Transform);
            
            pp.CapybaraModel = capybara;
            pp.IsCapybaraModeEnabled = true;
        }
        else
        {
            target.DisableEffect(EffectType.Fade);
            target.DisableEffect(EffectType.SilentWalk);
            UnityEngine.Object.Destroy(pp.CapybaraModel);
            pp.CapybaraModel = null;
            pp.IsCapybaraModeEnabled = false;
        }
        
        
        response = $"<color=green>Успешно!</color>";
        return true;
    }
    
    private static Config InstanceConfig()
    {
        return Plugin.Instance.Config;
    }
}