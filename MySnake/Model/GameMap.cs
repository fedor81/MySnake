namespace MySnake.Model;

public class GameMap
{
    private MapCell[,] Map { get; set; }
    private MapCell[,] OriginalMap { get; set; }
    
    public readonly int Width;
    public readonly int Height;

    public GameMap(int width, int height)
    {
        Map = new MapCell[width, height];
        OriginalMap = (MapCell[,])Map.Clone();
        Width = width;
        Height = height;
    }

    public GameMap(MapCell[,] map)
    {
        Map = map.Clone() as MapCell[,]; 
        OriginalMap = map.Clone() as MapCell[,];
        Width = map.GetLength(0);
        Height = map.GetLength(1);
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