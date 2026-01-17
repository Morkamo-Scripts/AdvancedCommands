using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using Interactables.Interobjects.DoorUtils;
using LabApi.Events.Arguments.WarheadEvents;
using LabApi.Features.Wrappers;
using ElevatorDoor = Interactables.Interobjects.ElevatorDoor;

namespace AdvancedCommands.Commands.ElevalorLock;

[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class ElevalorsLockCommand : ICommand
{
    public string Command { get; } = "elevatorsLock";
    public string[] Aliases { get; } = { "elslock" };
    public string Description { get; } = "Включает и выключает блокировку всех лифтов!";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission("ac.elslock"))
        {
            string requestPermission = "Требуется разрешение - 'ac.elslock'";
            
            if (InstanceConfig().Debug)
                response = $"<color=red>Вы не имеете права использовать данную команду!</color>\n" +
                           $"<color=orange>[{requestPermission}]</color>";
            else
                response = "<color=red>Вы не имеете права использовать данную команду!</color>";
            
            return false;
        }
        
        if (arguments.Count < 1)
        {
            response = "<color=orange>Формат ввода: elslock [1/0]. (Пример: elslock 1)</color>";
            return false;
        }

        if (!byte.TryParse(arguments.At(0), out var state) || state != 1 && state != 0)
        {
            response = "<color=orange>Некорректное значение состояния. " +
                       "Введите значение 1 если хотите включить блокировку или 0 чтобы выключить.</color>";
            return false;
        }
        
        bool isLocked = state == 1;

        foreach (DoorVariant allDoor in DoorVariant.AllDoors)
        {
            if (allDoor is ElevatorDoor elevatorDoor)
            {
                if (isLocked)
                    elevatorDoor.NetworkActiveLocks = (ushort) (elevatorDoor.ActiveLocks | 4U);
                else
                    elevatorDoor.NetworkActiveLocks = 0;
            }
        }
        
        response = isLocked ? 
            "<color=green>Блокировка включена!</color>" 
            : 
            "<color=green>Блокировка отключена!</color>";
        
        return true;
    }
    
    private static Config InstanceConfig()
    {
        return Plugin.Instance.Config;
    }
}