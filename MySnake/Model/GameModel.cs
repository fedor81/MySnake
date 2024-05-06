using System.Collections.Generic;

namespace MySnake.Model;

public class GameModel
{
    public GameModel(int mapWidth, int mapHeight)
    {
        MapWidth = mapWidth;
        MapHeight = mapHeight;
        Map = new MapCell[MapWidth, MapHeight];
        MapCells = new MapCell[MapWidth, MapHeight];
        Player = new Snake(50, 50, 10);
        Map[50, 50] = MapCell.Player;
        _snakes.Add(Player);
    }

    public int MapHeight { get; private set; }
    public int MapWidth { get; private set; }
    private MapCell[,] Map { get; set; }
    public MapCell[,] MapCells { get; private set; }

    private Snake Player { get; set; }
    private List<Snake> _snakes = new();

    public SnakeBody GetPlayerHead() => Player.Head;
    public MapCell GetMapCell(int x, int y) => MapCells[x, y];

    public void MovePlayer(Direction direction)
    {
        if (Player.Move(direction))
            Update();
    }

    private void Update()
    {
        foreach (var snake in _snakes)
        {
            MapCells = (MapCell[,])Map.Clone();
            var snakeHead = snake.Head;
            MapCells[snakeHead.X, snakeHead.Y] = MapCell.Snake;
            foreach (var snakeBody in snake.Body)
            {
                MapCells[snakeBody.X, snakeBody.Y] = MapCell.Snake;
            }
        }
    }
}