using System;
using System.Collections;
using System.Collections.Generic;
using AdvancedCommands.Components;
using AdvancedCommands.Components.Extensions;
using AdvancedCommands.Components.Features;
using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using UnityEngine;

namespace AdvancedCommands.Commands.SaddlePlayer
{
    [CommandHandler(typeof(ClientCommandHandler))]
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class SaddlePlayerCommand : ICommand, IAdvancedCommand
    {
        public bool IsActive => Plugin.Instance?.Config.CommandProperties.CpSaddlePlayer.IsActive ?? false;

        public string Command { get; } = "saddlePlayer";
        public string[] Aliases { get; } = { "sadpl", "sdpl" };
        public string Description { get; } = "Подбирает выбранного игрока!";
        
        private static Dictionary<ushort, Coroutine> _playerCoroutines = new Dictionary<ushort, Coroutine>();

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!IsActive)
            {
                response = "Команда отключена сервером";
                return false;
            }

            if (!sender.CheckPermission("ac.sadpl"))
            {
                string requestPermission = "Требуется разрешение - 'ac.sadpl'";

                if (InstanceConfig().Debug)
                    response = $"<color=red>Вы не имеете права использовать данную команду!</color>\n" +
                               $"<color=orange>[{requestPermission}]</color>";
                else
                    response = "<color=red>Вы не имеете права использовать данную команду!</color>";

                return false;
            }

            if (arguments.Count < 2)
            {
                response = "<color=orange>Формат ввода: sdpl [playerId] [1/0]</color>";
                return false;
            }

            if (!ushort.TryParse(arguments.At(0), out var id) ||
                !short.TryParse(arguments.At(1), out var value) || (value != 0 && value != 1))
            {
                response = "<color=orange>Формат ввода: sdpl [playerId] [1/0]</color>";
                return false;
            }

            var target = Player.Get(id);

            if (target == null /* || target.IsNPC || !target.IsAlive*/)
            {
                response = "<color=orange>Игрок не найден!</color>";
                return false;
            }

            if (value == 1)
            {
                var cam = sender.AsPlayer().CameraTransform;
                Coroutine teleportCoroutine = CoroutineRunner.Run(TeleportPlayer(target, sender.AsPlayer()));
                
                if (_playerCoroutines.ContainsKey(id))
                {
                    CoroutineRunner.Stop(_playerCoroutines[id]);
                }

                _playerCoroutines[id] = teleportCoroutine;

                target.IsGodModeEnabled = true;
                target.EnableEffect(EffectType.Ensnared);
                target.Transform.SetParent(sender.AsPlayer().Transform);
            }
            else
            {
                if (_playerCoroutines.ContainsKey(id))
                {
                    CoroutineRunner.Stop(_playerCoroutines[id]);
                    _playerCoroutines.Remove(id);
                }

                target.IsGodModeEnabled = false;
                target.DisableEffect(EffectType.Ensnared);
                target.Transform.SetParent(null);
            }

            response = $"<color=green>Успешно!</color>";
            return true;
        }

        private static IEnumerator TeleportPlayer(Player target, Player owner)
        {
            while (true)
            {
                Vector3 spawnPos = (owner.CameraTransform.position - new Vector3(0, 0, 0)) + owner.CameraTransform.forward * 0.01f;
                target.Teleport(spawnPos);
                yield return new WaitForSeconds(0.1f);
            }
        }

        private static Config InstanceConfig()
        {
            return Plugin.Instance.Config;
        }
    }
}