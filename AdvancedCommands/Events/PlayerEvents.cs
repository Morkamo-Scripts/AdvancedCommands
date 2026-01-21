using System;
using AdvancedCommands.Events.EventArgs.Player;
using Exiled.API.Features;

namespace AdvancedCommands.Events
{
    public partial class PlayerEvents
    {
        public event Action<PlayerCalledAdminEventArgs> PlayerCalledAdmin;
        public event Action<PlayerFullConnectedEventArgs> PlayerFullConnected;
    }

    public partial class PlayerEvents
    {
        public void InvokePlayerCalledAdmin(Player player)
        {
            var ev = new PlayerCalledAdminEventArgs(player);
            PlayerCalledAdmin?.Invoke(ev);
        }
        
        public void InvokePlayerFullConnected(Player player)
        {
            var ev = new PlayerFullConnectedEventArgs(player);
            PlayerFullConnected?.Invoke(ev);
        }
    }
}