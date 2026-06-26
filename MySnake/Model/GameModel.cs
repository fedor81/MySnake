using System;
using System.Collections.Generic;
using System.Linq;
using MySnake.MapGenerator;
using MySnake.Tools;

namespace MySnake.Model;

public class GameModel
{
    private const int PlayerInitLength = 5;

    public GameModel()
    {
        var random = new Random();
        var binder = new MapBinder(random.Next());

        FoodSpawner = new FoodSpawner(1);
        FoodSpawner.SpawnFood += SpawnFood;

        Map = binder.CreateGameMap().GetNodes().First().Value;

        Snake.TailRemovedStatic += RevertMapCell;
        Snake.HeadMovedStatic += AddSnakeBodyToMap;
        Snake.SnakeDiedStatic += ProcessSnakeDied;

        var playerX = MapWidth / 2;
        var playerY = MapHeight / 2;

        while (!CanSpawnPlayer(new Point(playerX, playerY)))
            Map = binder.CreateGameMap().GetNodes().First().Value;

        Player = new Snake(playerX, playerY, PlayerInitLength);

        AddSnakeToGame(Player, isPlayer: true);
    }

    bool CanSpawnPlayer(Point playerHead)
    {
        if (!Map.IsWithinMap(playerHead) || Map[playerHead] == MapCell.Wall)
            return false;

        foreach (var point in DirectionHelper.AllDirections.Select(dir => playerHead.With(Orientation.DirectionToMove[dir])))
        {
            if (!Map.IsWithinMap(point) || Map[point] == MapCell.Wall)
                return false;
        }

        return true;
    }

    public Action Exit;
    public event Action StateChanged;
    public event Action MapChanged;

    private FoodSpawner FoodSpawner { get; set; }
    private GameMap Map { get; set; }

    public int MapWidth => Map.Width;
    public int MapHeight => Map.Height;

    public MapCell GetMapCell(int x, int y) => Map[x, y];
    public MapCell GetOriginalMapCell(int x, int y) => Map.GetOriginalMapCell(x, y);

    private Snake Player { get; set; }
    private HashSet<Snake> _snakes = new();

    public int GetPlayerHungerValue() => Player.Hunger;
    public Point GetPlayerHead() => Player.Head;
    private int GameTime { get; set; }

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
                Player.EatFood();
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

    private void RevertMapCell(object sender, Point point)
    {
        Map.RevertCell(point);
    }

    private void SpawnFood()
    {
        while (true)
        {
            var x = new Random().Next(0, MapWidth);
            var y = new Random().Next(0, MapHeight);

            if (Map[x, y] is not (MapCell.Empty or MapCell.Grass)) continue;
            Map[x, y] = MapCell.Food;
            break;
        }
    }

    private void ProcessSnakeDied(Snake snake)
    {
        if (snake.Equals(Player))
            Exit?.Invoke();
        else
            _snakes.Remove(snake);
    }

    private void Update()
    {
        GameTime++;
        Map.Update();
        StateChanged?.Invoke();
        FoodSpawner.Update(GameTime);
    }

    private void AddSnakeToGame(Snake snake, bool isPlayer = false)
    {
        _snakes.Add(snake);
        Map[snake.Head] = isPlayer ? MapCell.Player : MapCell.Snake;
    }

    public HashSet<Point> GetOccupiedSpaceByPlayer() => Player.GetSpaceOccupiedBySnake();
}