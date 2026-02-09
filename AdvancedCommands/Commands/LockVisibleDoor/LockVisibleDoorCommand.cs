using System;
using AdvancedCommands.Components.Extensions;
using CommandSystem;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.Permissions.Extensions;
using Interactables.Interobjects.DoorUtils;
using RemoteAdmin;
using UnityEngine;

namespace AdvancedCommands.Commands.LockVisibleDoor;

[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class LockVisibleDoorCommand : ICommand
{
    public string Command { get; } = "LockVisibleDoor";
    public string[] Aliases { get; } = { "lockvd", "lvd" };
    public string Description { get; } = "Включает/выключает блокировку двери, на которую вы смотрите.";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (sender is not PlayerCommandSender)
        {
            response = "<color=orange>Только для игроков!</color>";
            return false;
        }
        
        if (!sender.CheckPermission("ac.lvd"))
        {
            string requestPermission = "Требуется разрешение - 'ac.lvd'";
            
            if (Plugin.Instance.Config.Debug)
                response = $"<color=red>Вы не имеете права использовать данную команду!</color>\n" +
                           $"<color=orange>[{requestPermission}]</color>";
            else
                response = "<color=red>Вы не имеете права использовать данную команду!</color>";
            
            return false;
        }

        if (!sender.AsPlayer().IsAlive)
        {
            response = "<color=orange>Доступно только живым игрокам!</color>";
            return false;
        }
        
        var cam = sender.AsPlayer().CameraTransform;
        var ray = new Ray(cam.position, cam.forward);

        if (!Physics.SphereCast(ray, 1f, out var hit, 10f, ~0, QueryTriggerInteraction.Ignore))
        {
            response = "<color=orange>Дверь не найдена! Возможно сторонняя коллизия перекрывает дверь.</color>";
            return false;
        }
        
        var doorVariant = GetDoorVariant(hit);
        if (doorVariant == null)
        {
            response = "<color=orange>Дверь не найдена! Возможно сторонняя коллизия перекрывает дверь.</color>";
            return false;
        }

        var door = Door.Get(doorVariant);
        var newState = !door.IsLocked;

        doorVariant.ServerChangeLock(DoorLockReason.AdminCommand, newState);

        response = $"<color=green>Успешно {(newState ? "заблокировано" : "разблокировано")}</color>";
        return true;
    }
    
    public static DoorVariant GetDoorVariant(RaycastHit hit)
    {
        var collider = hit.collider;

        return
            collider.GetComponent<DoorVariant>() ??
            collider.GetComponentInParent<DoorVariant>() ??
            collider.GetComponentInChildren<DoorVariant>();
    }
}