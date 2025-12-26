/*using System;
using AdminToys;
using AdvancedCommands.Components.Extensions;
using CommandSystem;
using Exiled.API.Features.Toys;
using Exiled.Permissions.Extensions;
using PrimitiveObjectToy = LabApi.Features.Wrappers.PrimitiveObjectToy;

namespace AdvancedCommands.Commands.CapybaraMode;

[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class CapybaraModeCommand : ICommand
{
    public string Command { get; } = "capybaramode";
    public string[] Aliases { get; } = { "cpm" };
    public string Description { get; } = "Превращает вас в неотразимую капибару!";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
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

        if (arguments.Count < 1)
        {
            response = "<color=orange>Формат ввода: capybaramode [on/off]</color>";
            return false;
        }

        switch (arguments.At(0).ToLower())
        {
            case "on":
            {
                
                AdminToy.Get().Spawn();
                
                response = "<color=green>Теперь вы неотразимая капибара!</color>";
                return true;
            }
            case "off":
            {
                response = "<color=green>Теперь вы снова обычный человек!</color>";
                return true;
            }
        }
    }
    
    private static Config InstanceConfig()
    {
        return Plugin.Instance.Config;
    }
}*/