using System;

namespace MySnake.Model;

public class MapGenerator
{
    private readonly Random _random;
    private readonly PerlinNoise _perlin;
    private readonly Growth _growth;

    private const int MinWidth = 30;
    private const int MinHeight = 30;
    private const int MaxWidth = 100;
    private const int MaxHeight = 100;

    public MapGenerator(int seed)
    {
        _random = new Random(seed);
        _perlin = new PerlinNoise(_random.Next());
        _growth = new Growth(_random.Next());
    }

    public GameMap GenerateRandomMap(int width = 0, int height = 0)
    {
        if (width == 0 || height == 0)
        {
            width = _random.Next(MinWidth, MaxWidth);
            height = _random.Next(MinHeight, MaxHeight);
        }


        return _random.Next(3) switch
        {
            0 => GenerateMapWithAnyWalls(width, height),
            1 => GenerateMapWithThickWall(width, height),
            _ => GenerateMapWithRoundWalls(width, height),
        };
    }

    public GameMap GenerateMapWithThickWall(int width, int height)
    {
        const float minNoiseValue = 0.6f;
        const float maxNoiseValue = 0.8f;

        var wallMap = GenerateMapByPerlin(width, height,
            noiseValue => Interpolation.CubicCurve(noiseValue) is > minNoiseValue and < maxNoiseValue);

        const int growIterations = 2;

        wallMap = _growth.Grow(wallMap, growIterations);
        wallMap = _growth.Decrease(wallMap);

        var map = GetMap(width, height, wallMap);
        return new GameMap(map);
    }

    public GameMap GenerateMapWithAnyWalls(int width, int height)
    {
        const float minNoiseValue = 0.6f;
        const float maxNoiseValue = 0.8f;

        var wallMap =
            GenerateMapByPerlin(width, height, noiseValue => noiseValue is > minNoiseValue and < maxNoiseValue);

        const int liveIterations = 100;
        const int growIterations = 2;
        const int randomOffset = 0;

        wallMap = _growth.Grow(wallMap, growIterations);
        wallMap = _growth.GrowByGameOfLive(wallMap, liveIterations, randomOffset);
        wallMap = _growth.Median(wallMap);

        var map = GetMap(width, height, wallMap);
        return new GameMap(map);
    }

    public GameMap GenerateMapWithRoundWalls(int width, int height)
    {
        var wallMap = GenerateMapByPerlin(width, height,
            noiseValue => Interpolation.QuinticCurve(noiseValue) is > 0.2f and < 0.8f);

        const int liveIterations = 200;
        const int growIterations = 2;

        wallMap = _growth.GrowByGameOfLive(wallMap, liveIterations, _random.Next(6));
        wallMap = _growth.Decrease(wallMap);
        wallMap = _growth.Grow(wallMap, growIterations);

        var map = GetMap(width, height, wallMap);
        return new GameMap(map);
    }

    private MapCell[,] GetMap(int width, int height, bool[,] wallMap)
    {
        var map = new MapCell[width, height];
        SetWallsOnMap(map, wallMap);
        SetGrassOnMap(map);
        SetBoundaryWalls(map);
        return map;
    }

    private static MapCell[,] SetBoundaryWalls(MapCell[,] map)
    {
        var width = map.GetLength(0);
        var height = map.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            map[x, 0] = MapCell.Wall;
            map[x, height - 1] = MapCell.Wall;
        }

        for (int y = 0; y < height; y++)
        {
            map[0, y] = MapCell.Wall;
            map[width - 1, y] = MapCell.Wall;
        }

        return map;
    }

    private static MapCell[,] SetCellsOnMap(MapCell[,] map, bool[,] cellMap, MapCell cellType,
        Func<int, int, bool> additionalCondition = null)
    {
        var width = map.GetLength(0);
        var height = map.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!cellMap[x, y]) continue;
                if (additionalCondition == null || additionalCondition(x, y))
                    map[x, y] = cellType;
            }
        }

        return map;
    }

    private MapCell[,] SetGrassOnMap(MapCell[,] map) =>
        SetGrassOnMap(map, GetGrassMap(map.GetLength(0), map.GetLength(1)));

    private static MapCell[,] SetGrassOnMap(MapCell[,] map, bool[,] grassMap) =>
        SetCellsOnMap(map, grassMap, MapCell.Grass, (x, y) => map[x, y] != MapCell.Wall);

    private static MapCell[,] SetWallsOnMap(MapCell[,] map, bool[,] wallMap) =>
        SetCellsOnMap(map, wallMap, MapCell.Wall);

    private bool[,] GenerateMapByPerlin(int width, int height, Func<float, bool> condition)
    {
        var map = new bool[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var noise = _perlin.GetNoise(x + _random.NextSingle(), y + _random.NextSingle());
                map[x, y] = condition(noise);
            }
        }

        return map;
    }

    private bool[,] GetGrassMap(int width, int height)
    {
        const double minNoiseValue = 0.2;
        const double maxNoiseValue = 0.3;

        var grassMap = GenerateMapByPerlin(width, height,
            noiseValue => minNoiseValue < noiseValue && noiseValue < maxNoiseValue);

        const int minGrowthGrass = 3;
        const int maxGrowthGrass = 7;

        grassMap = _growth.Grow(grassMap, _random.Next(minGrowthGrass, maxGrowthGrass));
        return grassMap;
    }
}