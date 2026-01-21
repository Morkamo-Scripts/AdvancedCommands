namespace AdvancedCommands.Events.Components;

public interface ICancelableEvent
{
    public bool IsAllowed { get; set; }
}