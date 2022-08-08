using System.Collections.Generic;

/// <summary>
/// The field model.
/// </summary>
public class FieldModel
{
    /// <summary>
    /// The tile models.
    /// </summary>
    public List<TileModel> Tiles;

    /// <summary>
    /// The list of indexes for the spawnpoints.
    /// </summary>
    public List<int> SpawnPoints;

    /// <summary>
    /// The list of indexes for the opponent's spawnpoints.
    /// </summary>
    public List<int> OpponentSpawnPoints;
}
