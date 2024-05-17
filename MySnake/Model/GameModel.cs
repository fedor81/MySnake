using System;
using System.Collections.Generic;
using System.Linq;
using MySnake.MapGenerator;

namespace MySnake.Model;

public class GameModel
{
    private const int PlayerInitLength = 10;
    
    public GameModel()
    {
        var random = new Random();
        var binder = new MapBinder(random.Next());
        
        FoodSpawner = new FoodSpawner(1);
        FoodSpawner.SpawnFood += SpawnFood;
        
        Map = binder.CreateGameMap().GetNodes().First().Value;
        Player = new Snake(MapWidth / 2, MapHeight / 2, PlayerInitLength);
        Map[Player.Head] = MapCell.Player;
        
        Player.TailRemoved += RemoveSnakeTailFromMap;
        Player.HeadMoved += AddSnakeBodyToMap;
    }

    public event Action StateChanged;
    public event Action MapChanged;

    private FoodSpawner FoodSpawner { get; set; }
    public GameMap Map { get; set; }
    
    public int MapWidth => Map.Width;
    public int MapHeight => Map.Height;

    private Snake Player { get; set; }
    private List<Snake> _snakes = new();

    public Point GetPlayerHead() => Player.Head;
    public MapCell GetMapCell(int x, int y) => Map[x, y];
    public MapCell[,] GetOriginalMap() => Map.GetOriginalMap();

    private long GameTime { get; set; }

    public void MovePlayer(Direction direction)
    {
        var nextPoint = Player.Head.With(Orientation.DirectionToMove[direction]);
        if (!Player.CanMove(direction) || !Map.IsWithinMap(nextPoint)) return;

        var mapCell = Map[nextPoint];

        if (mapCell is MapCell.Snake or MapCell.Wall or MapCell.Player)
            Player.GetDamage();
        else
        {
            if (mapCell == MapCell.Food)
            {
                Player.Grow();
                FoodSpawner.EatFood();
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
            if (Map[x, y] is not MapCell.Empty or MapCell.Grass) continue;
            Map[x, y] = MapCell.Food;
            break;
        }
    }

    private void Update()
    {
        GameTime++;
        StateChanged?.Invoke();
        FoodSpawner.Update(GameTime);
    }
}