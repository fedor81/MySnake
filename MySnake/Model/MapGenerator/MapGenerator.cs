using System;

namespace MySnake.Model;

public class MapGenerator
{
    public GameMap GenerateMap()
    {
        var width = 100;
        var height = 100;
        var map = new MapCell[width, height];
        
        var NoiseMap = new float[width, height];
        var random = new Random();
        var perlin = new PerlinNoise(random.Next());
        var growth = new Growth(random.Next());

        var wallMap = new bool[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var noise = perlin.GetNoise(x + random.NextSingle(), y + random.NextSingle());
                NoiseMap[x, y] = Math.Max(0, noise);
                wallMap[x, y] = Interpolation.CubicCurve(noise) is > 0.6f and < 0.8f;
            }
        }

        var iterations = 200;
        // wallMap = growth.GrowByGameOfLive(wallMap, iterations, random.Next(6));
        var minIterations = 4;
        wallMap = growth.Grow(wallMap, 2);
        wallMap = growth.Decrease(wallMap, 1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y] = wallMap[x, y] ? MapCell.Wall : MapCell.Empty;
            }
        }

        var grassMap = new bool[width, height];
        var minNoiseValue = 0.2;
        var maxNoiseValue = 0.3;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var noise = perlin.GetNoise(x + random.NextSingle(), y + random.NextSingle());
                if (minNoiseValue < noise && noise < maxNoiseValue && map[x, y] != MapCell.Wall)
                    grassMap[x, y] = true;
            }
        }

        const int minGrowthGrass = 3;
        const int maxGrowthGrass = 7;

        grassMap = growth.Grow(grassMap, random.Next(minGrowthGrass, maxGrowthGrass));

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grassMap[x, y] && map[x, y] != MapCell.Wall)
                    map[x, y] = MapCell.Grass;
            }
        }

        return new GameMap(map);
    }
}