namespace MySnake.Model;

public class GameMap
{
    private MapCell[,] Map { get; }
    private MapCell[,] OriginalMap { get; }

    public readonly int Width;
    public readonly int Height;

    public int MapGameTime { get; private set; }

    public GameMap(int width, int height) : this(new MapCell[width, height])
    {
    }

    public GameMap(MapCell[,] map)
    {
        Map = map.Clone() as MapCell[,];
        OriginalMap = Map.Clone() as MapCell[,];
        Width = Map.GetLength(0);
        Height = Map.GetLength(1);
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

    public MapCell GetOriginalMapCell(int x, int y) => OriginalMap[x, y];
    public MapCell GetOriginalMapCell(Point point) => GetOriginalMapCell(point.X, point.Y);

    public MapCell[,] GetMap() => Map.Clone() as MapCell[,];

    public MapCell[,] GetOriginalMap() => OriginalMap.Clone() as MapCell[,];

    public void RevertCell(Point point)
    {
        this[point] = OriginalMap[point.X, point.Y];
    }

    public override string ToString()
    {
        return $"GameMap({Width}:{Height})";
    }

    public void Update()
    {
        MapGameTime++;
    }

    public bool IsWithinMap(Point point) => IsWithinMap(point.X, point.Y);
    public bool IsWithinMap((int, int) coords) => IsWithinMap(coords.Item1, coords.Item2);
    public bool IsWithinMap(int x, int y) => 0 <= x && x < Width && 0 <= y && y < Height;
}