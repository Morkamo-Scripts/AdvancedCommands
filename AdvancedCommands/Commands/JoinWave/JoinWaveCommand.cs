using System;
using AdvancedCommands.Components.Extensions;
using CommandSystem;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using PlayerRoles;
using Respawning;

namespace AdvancedCommands.Commands.JoinWave;

[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class JoinWaveCommand : ICommand
{
    public string Command { get; } = "JoinWave";
    public string[] Aliases { get; } = { "jwave", "jw" };
    public string Description { get; } = $"Спавнит игрока в недавно прибывшем отряде в течение овертайма." +
                                         $"({InstanceConfig().JoinWave.SpawnOvertime} cек.)";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission("ac.joinwave"))
        {
            string requestPermission = "Требуется разрешение - 'ac.joinwave'";
            
            if (InstanceConfig().Debug)
                response = $"<color=red>Вы не имеете права использовать данную команду!</color>\n" +
                           $"<color=orange>[{requestPermission}]</color>";
            else
                response = "<color=red>Вы не имеете права использовать данную команду!</color>";
            
            return false;
        }
        
        if (!Round.IsStarted)
        {
            response = "<color=orange>Раунд ещё не начался!</color>";
            return false;
        }

        if (sender.AsPlayer().Role.Type != RoleTypeId.Spectator)
        {
            response = "<color=orange>Команда доступна только наблюдателям!</color>";
            return false;
        }

        if (Plugin.Instance.LastSpawnTime == null)
        {
            response = "<color=orange>Ни один отряд ещё не прибывал!</color>";
            return false;
        }

        if (sender.AsPlayer().AdvancedCommand().PlayerProperties.HasBeenSpawned)
        {
            response = "<color=orange>Вы уже использовали эту команду за этот отряд!</color>";
            return false;
        }
        
        if (DateTime.UtcNow - Plugin.Instance.LastSpawnTime >= TimeSpan.FromSeconds(30))
        {
            response = $"<color=orange>Время овертайма истекло! ({InstanceConfig().JoinWave.SpawnOvertime} сек.)</color>";
            return false;
        }

        switch (Plugin.Instance.LastSpawnTeam)
        {
            case Team.ChaosInsurgency:
            {
                var role = InstanceConfig().JoinWave.AllowedChaosInsurgencyRoles.GetRandomValue();
                
                sender.AsPlayer().Role.Set(role);
                sender.AsPlayer().AdvancedCommand().PlayerProperties.HasBeenSpawned = true;
                
                response = $"<color=green>Вы успешно появились за <b>{role}</b></color>";
                return true;
            }
            case Team.FoundationForces:
            {
                var role = InstanceConfig().JoinWave.AllowedNtfRoles.GetRandomValue();
                
                sender.AsPlayer().Role.Set(role);
                sender.AsPlayer().AdvancedCommand().PlayerProperties.HasBeenSpawned = true;
                
                response = $"<color=green>Вы успешно появились за <b>{role}</b></color>";
                return true;
            }
            default:
                response = $"<color=red>Неизвестная ошибка...</color>";
                return false;
        }
    }
    
    private static Config InstanceConfig()
    {
        return Plugin.Instance.Config;
    }
}