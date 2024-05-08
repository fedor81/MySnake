using System;
using System.Runtime.Intrinsics.X86;

namespace MySnake.Model;

public class GameMap
{
    public float[,] NoiseMap;
    private MapCell[,] Map { get; set; }
    private MapCell[,] OriginalMap { get; set; }
    public int Width { get; private set; }
    public int Height { get; private set; }

    // TODO: генерация карты
    public GameMap(int width, int height)
    {
        Map = new MapCell[width, height];
        var random = new Random();
        var perlin = new PerlinNoise(random.Next());
        NoiseMap = new float[width, height];
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var noise = perlin.GetNoise(x + random.NextSingle(), y + random.NextSingle());
                NoiseMap[x, y] = noise;
                Map[x, y] = noise < 0.6 ? MapCell.Empty : MapCell.Wall;
            }
        }
        OriginalMap = Map.Clone() as MapCell[,];
        Width = width;
        Height = height;
    }

    public MapCell this[int x, int y]
    {
        get => Map[x, y];
        set => Map[x, y] = value;
    }

    public MapCell this[Point point]
    {
        get => this[point.X, point.Y];
        set => this[point.X, point.Y] = value;
    }

    public MapCell[,] GetOriginalMap()
    {
        return OriginalMap.Clone() as MapCell[,];
    }

    public void RevertCell(Point point)
    {
        this[point] = OriginalMap[point.X, point.Y];
    }

    public bool IsWithinMap(Point point) => IsWithinMap(point.X, point.Y);
    public bool IsWithinMap((int, int) coords) => IsWithinMap(coords.Item1, coords.Item2);
    public bool IsWithinMap(int x, int y) => 0 <= x && x < Width && 0 <= y && y < Height;
}