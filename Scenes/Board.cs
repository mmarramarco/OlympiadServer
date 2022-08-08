using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// The board.
/// </summary>
public class Board : TileMap
{
    /// <summary>
    /// The percent of obstacle on the board.
    /// </summary>
    [Export]
    public int ObstaclePercentage = 30;

    /// <summary>
    /// The number of allowed spawnpoints per player.
    /// </summary>
    [Export]
    public int SpawnPointsNumber = 3;

    /// <summary>
    /// The map size.
    /// </summary>
    [Export]
    public int MapSize = 8;

    /// <summary>
    /// The percent of obstacles that are holes on the board.
    /// </summary>
    [Export]
    public int HolePercentage = 50;

    private FieldModel _field;
    private Random _random = new Random();

    /// <summary>
    /// Get the field object, and spawnpoints depending on if we're player one or two.
    /// </summary>
    /// <returns></returns>
    public FieldModel GetField(bool isPlayer2 = false)
    {
        if (_field == null)
        {
            _field = GenerateField();
        }

        if (isPlayer2)
        {
            var playerOneSpawnPoints = _field.SpawnPoints;
            _field.SpawnPoints = _field.OpponentSpawnPoints;
            _field.OpponentSpawnPoints = playerOneSpawnPoints;
        }

        return _field;
    }

    private FieldModel GenerateField()
    {
        var tiles = GenerateTiles();
        var spawnpoints = GenerateSpawnPoints(tiles);
        var opponentSpawnPoints = GenerateSpawnPoints(tiles, spawnpoints);

        var field = new FieldModel
        {
            Tiles = tiles,
            SpawnPoints = spawnpoints,
            OpponentSpawnPoints = opponentSpawnPoints
        };

        return field;
    }

    private List<TileModel> GenerateTiles()
    {
        var tiles = new List<TileModel>();
        var startingPoint = MapSize / 2;
        TileModel tile;
        for (int x = -startingPoint; x < startingPoint; x++)
        {
            for (int y = -startingPoint; y < startingPoint; y++)
            {
                var makeObstacle = _random.Next(100) <= ObstaclePercentage;
                var tileType = makeObstacle ?
                    MakeObstacle() :
                    TileType.Walkable;
                tile = new TileModel
                {
                    X = x,
                    Y = y,
                    TileType = tileType
                };
                tiles.Add(tile);
            }
        }

        return tiles;
    }

    private TileType MakeObstacle()
    {
        var isHole = _random.Next(100) <= HolePercentage;
        return isHole ?
            TileType.Hole :
            TileType.SmallObstacle;
    }

    private List<int> GenerateSpawnPoints(List<TileModel> tiles, List<int> opponentSpawnPoints = null)
    {
        var spawnpoints = new List<int>();
        opponentSpawnPoints = opponentSpawnPoints ?? new List<int>(); // we attribute a new list so we don't have to do null check in the if later down the line.
        var legallySpawnablePoints = tiles.Where(_ => _.TileType == TileType.Walkable).ToList();

        while (spawnpoints.Count != SpawnPointsNumber)
        {
            var randomIndex = _random.Next(legallySpawnablePoints.Count);
            if (!spawnpoints.Contains(randomIndex) && // We always want 3 spawnpoints, so if we randomly elected a spawnpoints we already have, we don't add it to do one more loop.
                !opponentSpawnPoints.Contains(randomIndex)) // We want spawnpoints different than the opponent's spawnpoints.
            {
                spawnpoints.Add(randomIndex);
            }
        }

        return spawnpoints;
    }
}
