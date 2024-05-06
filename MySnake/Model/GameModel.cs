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
        Player.HeadMoved += AddSnakeBodyToMap;
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

    // TODO: Еда
    public void MovePlayer(Direction direction)
    {
        var nextMove = Player.Head.With(Orientation.DirectionToMove[direction]);

        if (!Player.CanMove(direction) || !InWindow(nextMove)) return;
        if (Map[nextMove.X, nextMove.Y] == MapCell.Player)
            Player.GetDamage();
        else
        {
            Player.Move(direction);
        }
    }

    private void AddSnakeBodyToMap(object snake, SnakeBody snakeBody)
    {
        var cell = snake.Equals(Player) ? MapCell.Player : MapCell.Snake;
        Map[snakeBody.X, snakeBody.Y] = cell;
    }

    private void RemoveSnakeTailFromMap(object snake, SnakeBody snakeBody)
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