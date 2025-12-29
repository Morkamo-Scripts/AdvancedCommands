using System;
using System.Collections.Generic;
using System.IO;

namespace AdvancedCommands.Commands.ReservedSlotsManagement;

public class RsmHeader
{
    public string RaConfigPatch { get; set; } = @"C:\Users\cfg.txt";

    public HashSet<string> ReservedGroups { get; set; } = new()
    {
        "owner",
    };
}