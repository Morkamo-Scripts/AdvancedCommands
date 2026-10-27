using System;
using System.Collections.Generic;
using System.Linq;
using AdvancedCommands.Components;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using PlayerRoles;
using UnityEngine;

namespace AdvancedCommands.Commands.Jail
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(ClientCommandHandler))]
    public class JailCommand : ICommand, IAdvancedCommand, IUsageProvider
    {
        public bool IsActive => Plugin.Instance?.Config.CommandProperties.CpJail.IsActive ?? false;

        public string Command => "jail";
        public string[] Aliases { get; } = { "jl" };
        public string Description => "Вызывает игрока на допрос или освобождает его";
        public string[] Usage { get; } = { "ID" };
        
        public Dictionary<string, PlayerInfo.PlayerInfoForJail> JailDict = new ();

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!IsActive)
            {
                response = "Команда отключена сервером";
                return false;
            }
            
            if (!sender.CheckPermission("ac.jail"))
            {
                response = "У вас <color=red>нет прав</color> на использование этой команды!";
                return false;
            }
            
            if (arguments.Count < 1)
            {
                response = "Использование: <color=yellow>jail [ID]</color>";
                return false;
            }
            
            Player target = Player.Get(arguments.At(0));
            if (target == null)
            {
                response = "Игрок с таким ID не найден.";
                return false;
            }

            var plugin = Plugin.Instance;
            
            if (JailDict.TryGetValue(target.UserId, out var jailInfo))
            {
                target.IsGodModeEnabled = false;
                target.Role.Set(jailInfo.TargetRole);
                target.Health = jailInfo.HP;
                target.Position = jailInfo.Position;
                target.ClearInventory();

                foreach (ItemType item in jailInfo.Inventory)
                    target.AddItem(item);

                foreach (var ammo in jailInfo.Ammo)
                    target.Ammo[ammo.Key] = ammo.Value;

                JailDict.Remove(target.UserId);

                response = "Игрок удалён с разборок.";
            }
            else
            {
                var currentItems = target.Items.Select(item => item.Type).ToList();
                var currentAmmos = new Dictionary<ItemType, ushort>(target.Ammo);
                
                JailDict[target.UserId] = new PlayerInfo.PlayerInfoForJail
                {
                    TargetRole = target.Role.Type,
                    HP = (int)target.Health,
                    Position = target.Position,
                    Ammo = currentAmmos,
                    Inventory = currentItems
                };

                target.IsGodModeEnabled = true;
                target.Role.Set(RoleTypeId.Tutorial);
                target.ClearInventory();
                target.ShowHint("<color=red>Вас вызвал администратор на проверку, просьба не выходить с сервера</color>", 5f);

                response = "Игрок вызван на разборки.";
            }

            return true;
        }
    }
    
    public class PlayerInfo
    {
        public class PlayerInfoForJail
        {
            public RoleTypeId TargetRole { get; set; }
            public int HP { get; set; }
            public Vector3 Position { get; set; }
            public List<ItemType> Inventory { get; set; }
            public Dictionary<ItemType, ushort> Ammo { get; set; }
        }
    }
}