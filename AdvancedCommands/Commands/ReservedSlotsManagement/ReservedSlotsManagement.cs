using System;
using CommandSystem;
using Exiled.Events.EventArgs.Player;
using Exiled.Loader;
using Exiled.Permissions.Extensions;
using LabApi.Features.Wrappers;
using MEC;

namespace AdvancedCommands.Commands.ReservedSlotsManagement;

[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class ReservedSlotsManagement : ICommand
{
    public string Command { get; } = "reservedSlot";
    public string[] Aliases { get; } = { "resslot", "rslot", "rsslot" };
    public string Description { get; } = "Добавляет и удаляет игроков из списка резервных слотов.";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission("ac.reservedSlotsControl"))
        {
            string requestPermission = "Требуется разрешение - 'ac.reservedSlotsControl'";
            
            if (Plugin.Instance.Config.Debug)
                response = $"Вы не имеете права использовать данную команду!\n" +
                           $"[{requestPermission}]";
            else
                response = "Вы не имеете права использовать данную команду!";
            
            return false;
        }
        
        if (arguments.Count < 2)
        {
            response = "Формат ввода: addrs [add/remove] [777777777777777@steam]";
            return false;
        }

        if (arguments.At(0).ToLower() != "add" && arguments.At(0).ToLower() != "remove")
        {
            response = "Некорректный идентификатор обработки. Используйте [add] или [remove].";
            return false;
        }
        
        if (!arguments.At(1).EndsWith("@steam", StringComparison.OrdinalIgnoreCase))
        {
            response = "Некорректный SteamId64. Пример: [77777777777777@steam].";
            return false;
        }

        if (arguments.At(0).ToLower() == "add")
        {
            if (ReservedSlot.HasReservedSlot(arguments.At(1)))
            {
                response = "Аккаунт уже имеет резервный слот.";
                return false;
            }
            
            ReservedSlots.Add(arguments.At(1));
            ReservedSlots.Reload();
            
            response = $"Аккаунт {arguments.At(1)} получил резервный слот.";
            return true;
        }
        
        if (!ReservedSlot.HasReservedSlot(arguments.At(1)))
        {
            response = "У данного игрока нет резервного слота.";
            return false;
        }
        
        ReservedSlots.Remove(arguments.At(1));
        ReservedSlots.Reload();
            
        response = $"Резервный слот успешно снят с аккаунта {arguments.At(1)}.";
        return true;
    }
}