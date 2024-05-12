using System;
using System.Collections.Generic;
using System.Linq;

namespace MySnake.Model;

public class GameModel
{
    public GameModel()
    {
        var random = new Random();
        var binder = new MapBinder(random.Next());
        Map = binder.CreateGameMap().GetNodes().First().Value;
        Player = new Snake(MapWidth / 2, MapHeight / 2, 10);
        Map[Player.Head] = MapCell.Player;
        SpawnFood();
        Player.TailRemoved += RemoveSnakeTailFromMap;
        Player.HeadMoved += AddSnakeBodyToMap;
    }

    public event Action StateChanged;
    public event Action MapChanged;

    public GameMap Map { get; set; }
    public int MapWidth => Map.Width;
    public int MapHeight => Map.Height;

    private Snake Player { get; set; }
    private List<Snake> _snakes = new();

    public Point GetPlayerHead() => Player.Head;
    public MapCell GetMapCell(int x, int y) => Map[x, y];
    public MapCell[,] GetOriginalMap() => Map.GetOriginalMap();

    // TODO: Damage при столкновении со стеной
    public void MovePlayer(Direction direction)
    {
        var nextPoint = Player.Head.With(Orientation.DirectionToMove[direction]);
        if (!Player.CanMove(direction) || !Map.IsWithinMap(nextPoint)) return;

        var mapCell = Map[nextPoint];

        if (mapCell == MapCell.Player)
            Player.GetDamage();
        else
        {
            if (mapCell == MapCell.Food)
            {
                Player.Grow();
                Map[nextPoint] = MapCell.Empty;
                SpawnFood();
            }
            Player.Move(direction);
        }
        Update();
    }

    private void AddSnakeBodyToMap(object snake, Point point)
    {
        var cell = snake.Equals(Player) ? MapCell.Player : MapCell.Snake;
        Map[point] = cell;
    }

    private void RemoveSnakeTailFromMap(object snake, Point point)
    {
        Map.RevertCell(point);
    }

    private void SpawnFood()
    {
        while (true)
        {
            var x = new Random().Next(0, MapWidth);
            var y = new Random().Next(0, MapHeight);
            if (Map[x, y] != MapCell.Empty) continue;
            Map[x, y] = MapCell.Food;
            break;
        }
    }

    private void Update()
    {
        StateChanged?.Invoke();
    }
}