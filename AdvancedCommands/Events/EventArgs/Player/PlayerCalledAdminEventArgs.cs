namespace AdvancedCommands.Events.EventArgs.Player
{
    public class PlayerCalledAdminEventArgs(Exiled.API.Features.Player player)
    {
        public Exiled.API.Features.Player Player => player;
    }
}