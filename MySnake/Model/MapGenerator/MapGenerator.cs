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

    public GameMap GenerateRandomMap()
    {
        var width = _random.Next(MinWidth, MaxWidth);
        var height = _random.Next(MinHeight, MaxHeight);

        return _random.Next(3) switch
        {
            0 => GenerateMapWithAnyWalls(width, height),
            1 => GenerateMapWithThickWall(width, height),
            _ => GenerateMapWithRoundWalls(width, height),
        };
    }

    public GameMap GenerateMapWithThickWall(int width, int height)
    {
        var map = new MapCell[width, height];
        var wallMap = GenerateMapByPerlin(width, height,
            noiseValue => Interpolation.CubicCurve(noiseValue) is > 0.6f and < 0.8f);

        wallMap = _growth.Grow(wallMap, 2);
        wallMap = _growth.Decrease(wallMap);

        SetWallsOnMap(map, wallMap);
        var grassMap = GetGrassMap(width, height, map);
        SetGrassOnMap(map, grassMap);

        return new GameMap(map);
    }

    public GameMap GenerateMapWithAnyWalls(int width, int height)
    {
        var wallMap = GenerateMapByPerlin(width, height, noiseValue => noiseValue is > 0.6f and < 0.8f);

        var liveIterations = 100;
        wallMap = _growth.Grow(wallMap, 2);
        wallMap = _growth.GrowByGameOfLive(wallMap, liveIterations, 0);
        wallMap = _growth.Median(wallMap);

        var map = new MapCell[width, height];
        SetWallsOnMap(map, wallMap);
        var grassMap = GetGrassMap(width, height, map);
        SetGrassOnMap(map, grassMap);

        return new GameMap(map);
    }

    public GameMap GenerateMapWithRoundWalls(int width, int height)
    {
        var map = new MapCell[width, height];

        var wallMap = GenerateMapByPerlin(width, height,
            noiseValue => Interpolation.QuinticCurve(noiseValue) is > 0.2f and < 0.8f);

        var liveIterations = 200;
        wallMap = _growth.GrowByGameOfLive(wallMap, liveIterations, _random.Next(6));
        wallMap = _growth.Decrease(wallMap);
        wallMap = _growth.Grow(wallMap, 2);

        SetWallsOnMap(map, wallMap);

        var grassMap = GetGrassMap(width, height, map);
        SetGrassOnMap(map, grassMap);

        return new GameMap(map);
    }


    private static MapCell[,] SetGrassOnMap(MapCell[,] map, bool[,] grassMap)
    {
        var width = grassMap.GetLength(0);
        var height = grassMap.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grassMap[x, y] && map[x, y] != MapCell.Wall)
                    map[x, y] = MapCell.Grass;
            }
        }

        return map;
    }

    private static MapCell[,] SetWallsOnMap(MapCell[,] map, bool[,] wallMap)
    {
        var width = wallMap.GetLength(0);
        var height = wallMap.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y] = wallMap[x, y] ? MapCell.Wall : MapCell.Empty;
            }
        }

        return map;
    }

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

    private bool[,] GetGrassMap(int width, int height, MapCell[,] map)
    {
        var grassMap = new bool[width, height];
        var minNoiseValue = 0.2;
        var maxNoiseValue = 0.3;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var noise = _perlin.GetNoise(x + _random.NextSingle(), y + _random.NextSingle());
                if (minNoiseValue < noise && noise < maxNoiseValue && map[x, y] != MapCell.Wall)
                    grassMap[x, y] = true;
            }
        }

        const int minGrowthGrass = 3;
        const int maxGrowthGrass = 7;

        grassMap = _growth.Grow(grassMap, _random.Next(minGrowthGrass, maxGrowthGrass));
        return grassMap;
    }
}