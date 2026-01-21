using System.Collections.Generic;
using System.ComponentModel;
using AdvancedCommands.Commands.IWantScp;
using AdvancedCommands.Commands.JoinWave;
using AdvancedCommands.Commands.ReservedSlotsManagement;
using Exiled.API.Interfaces;

namespace AdvancedCommands
{
    public sealed class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        public string ServerIdentifier { get; set; } = "Classic";
        
        [Description("Спавнит игрока в недавно прибывшем отряде в течение овертайма.")]
        public JoinWaveHandler JoinWave { get; set; } = new();

        public RsmHeader ReservedSlotManagement { get; set; } = new();
        
        public CallAdminSettings CallAdminSettings { get; set; } = new();
    }

    public class CallAdminSettings
    {
        [Description("Выводит этим группам список ожидающих игроков!")]
        // ReSharper disable once CollectionNeverUpdated.Global
        public HashSet<string> AdminGroups { get; set; } = new();

        [Description("Сервера, на которых разршена данная команда!")]
        public HashSet<string> AllowedServers { get; set; } = ["OnlyEvents"];
    }
}