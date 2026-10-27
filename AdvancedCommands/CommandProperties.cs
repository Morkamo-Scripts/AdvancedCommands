using AdvancedCommands.Commands;
using AdvancedCommands.Components;
using YamlDotNet.Serialization;

namespace AdvancedCommands;

public class CommandProperties
{
    [YamlMember(Alias = "cp_addahp", ApplyNamingConventions = false)]
    public CommandSettings CpAddahp { get; set; } = new();

    [YamlMember(Alias = "cp_addhs", ApplyNamingConventions = false)]
    public CommandSettings CpAddhs { get; set; } = new();

    [YamlMember(Alias = "cp_callAdmin", ApplyNamingConventions = false)]
    public CommandSettings CpCallAdmin { get; set; } = new();

    [YamlMember(Alias = "cp_DisableCalls", ApplyNamingConventions = false)]
    public CommandSettings CpDisableCalls { get; set; } = new();

    [YamlMember(Alias = "cp_isleep", ApplyNamingConventions = false)]
    public CommandSettings CpIsleep { get; set; } = new();

    [YamlMember(Alias = "cp_call956", ApplyNamingConventions = false)]
    public CommandSettings CpCall956 { get; set; } = new();

    [YamlMember(Alias = "cp_capybaramode", ApplyNamingConventions = false)]
    public CommandSettings CpCapybaramode { get; set; } = new();

    [YamlMember(Alias = "cp_disable2536", ApplyNamingConventions = false)]
    public CommandSettings CpDisable2536 { get; set; } = new();

    [YamlMember(Alias = "cp_elevatorsLock", ApplyNamingConventions = false)]
    public CommandSettings CpElevatorsLock { get; set; } = new();

    [YamlMember(Alias = "cp_explodeGrenade", ApplyNamingConventions = false)]
    public CommandSettings CpExplodeGrenade { get; set; } = new();

    [YamlMember(Alias = "cp_explodeSimulate", ApplyNamingConventions = false)]
    public CommandSettings CpExplodeSimulate { get; set; } = new();

    [YamlMember(Alias = "cp_globalInfinityRadio", ApplyNamingConventions = false)]
    public CommandSettings CpGlobalInfinityRadio { get; set; } = new();

    [YamlMember(Alias = "cp_infinityJailbird", ApplyNamingConventions = false)]
    public CommandSettings CpInfinityJailbird { get; set; } = new();

    [YamlMember(Alias = "cp_infinityMagazines", ApplyNamingConventions = false)]
    public CommandSettings CpInfinityMagazines { get; set; } = new();

    [YamlMember(Alias = "cp_itsMyScp", ApplyNamingConventions = false)]
    public CommandSettings CpItsMyScp { get; set; } = new();

    [YamlMember(Alias = "cp_IWantScp", ApplyNamingConventions = false)]
    public CommandSettings CpIWantScp { get; set; } = new();

    [YamlMember(Alias = "cp_jail", ApplyNamingConventions = false)]
    public CommandSettings CpJail { get; set; } = new();

    [YamlMember(Alias = "cp_JoinWave", ApplyNamingConventions = false)]
    public CommandSettings CpJoinWave { get; set; } = new();

    [YamlMember(Alias = "cp_LockVisibleDoor", ApplyNamingConventions = false)]
    public CommandSettings CpLockVisibleDoor { get; set; } = new();

    [YamlMember(Alias = "cp_miyabimode", ApplyNamingConventions = false)]
    public CommandSettings CpMiyabimode { get; set; } = new();

    [YamlMember(Alias = "cp_nonBreakableGlass", ApplyNamingConventions = false)]
    public CommandSettings CpNonBreakableGlass { get; set; } = new();

    [YamlMember(Alias = "cp_OwnerMode", ApplyNamingConventions = false)]
    public CommandSettings CpOwnerMode { get; set; } = new();

    [YamlMember(Alias = "cp_playerSize", ApplyNamingConventions = false)]
    public CommandSettings CpPlayerSize { get; set; } = new();

    [YamlMember(Alias = "cp_RandomPlayerSelection", ApplyNamingConventions = false)]
    public CommandSettings CpRandomPlayerSelection { get; set; } = new();

    [YamlMember(Alias = "cp_reservedSlot", ApplyNamingConventions = false)]
    public CommandSettings CpReservedSlot { get; set; } = new();

    [YamlMember(Alias = "cp_saddlePlayer", ApplyNamingConventions = false)]
    public CommandSettings CpSaddlePlayer { get; set; } = new();

    [YamlMember(Alias = "cp_serpentHandsWave", ApplyNamingConventions = false)]
    public CommandSettings CpSerpentHandsWave { get; set; } = new();
}

public class CommandSettings : IAdvancedCommand
{
    [YamlMember(Alias = "isActive", ApplyNamingConventions = false)]
    public bool IsActive { get; set; } = false;
}
