using System.Collections.Generic;

namespace MySnake.Model;

public class GameModel
{
    public GameModel(int mapWidth, int mapHeight)
    {
        MapWidth = mapWidth;
        MapHeight = mapHeight;
        Map = new MapCell[MapWidth, MapHeight];
        Player = new Snake(mapWidth / 2, mapHeight / 2, 10);
        Map[Player.Head.X, Player.Head.Y] = MapCell.Player;
        Player.TailRemoved += RemoveSnakeTailFromMap;
        _snakes.Add(Player);
    }

    public int MapHeight { get; private set; }
    public int MapWidth { get; private set; }
    private MapCell[,] Map { get; set; }

    private Snake Player { get; set; }
    private List<Snake> _snakes = new();

    public SnakeBody GetPlayerHead() => Player.Head;
    public MapCell GetMapCell(int x, int y) => Map[x, y];
    public MapCell GetMapCell((int, int) cords) => GetMapCell(cords.Item1, cords.Item2);

    public void MovePlayer(Direction direction)
    {
        var nextCords = Player.Head.With(Orientation.DirectionToMove[direction]);
        
        if (Player.CanMove(direction) && InWindow(nextCords))
        {
            Player.Move(direction);
            Map[nextCords.X, nextCords.Y] = MapCell.Player;
        }
    }

    private void RemoveSnakeTailFromMap(SnakeBody snakeBody)
    {
        Map[snakeBody.X, snakeBody.Y] = MapCell.Empty;
    }

    private bool InWindow(SnakeBody snakeBody) => InWindow(snakeBody.X, snakeBody.Y);
    private bool InWindow((int, int) coords) => InWindow(coords.Item1, coords.Item2);
    private bool InWindow(int x, int y) => 0 <= x && x < MapWidth && 0 <= y && y < MapHeight;

    private void Update()
    {
    }
}