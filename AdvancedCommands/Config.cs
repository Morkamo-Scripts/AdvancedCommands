using System.ComponentModel;
using AdvancedCommands.Commands.IWantScp;
using AdvancedCommands.Commands.JoinWave;
using Exiled.API.Interfaces;

namespace AdvancedCommands
{
    public sealed class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        
        [Description("Спавнит игрока в недавно прибывшем отряде в течение овертайма.")]
        public JoinWaveHandler JoinWave { get; set; } = new();
    }
}