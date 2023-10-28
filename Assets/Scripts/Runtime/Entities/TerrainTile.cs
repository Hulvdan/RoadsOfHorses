﻿using JetBrains.Annotations;

namespace BFG.Runtime {
/// <summary>
/// Is it a grass, a cliff?
/// </summary>
public class TerrainTile {
    public bool IsBooked;
    public string Name;

    public int Height;

    [CanBeNull]
    public ScriptableResource Resource;

    public int ResourceAmount;
}
}