using AdvancedCommands.Components.Features.Components;
using Exiled.API.Features;
using UnityEngine;

namespace AdvancedCommands.Components.Features;

public sealed class PlayerAdvancedCommands() : MonoBehaviour
{
    private void Awake()
    {
        Player = Player.Get(gameObject);
        PlayerProperties = new PlayerProperties(this);
    }
    
    public Player Player { get; private set; }
    public PlayerProperties PlayerProperties { get; private set; }
}