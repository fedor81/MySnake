using System.Collections.Generic;

namespace MySnake.Model;

public class GameModel
{
    public GameModel(int mapWidth, int mapHeight)
    {
        MapWidth = mapWidth;
        MapHeight = mapHeight;
        Map = new MapCell[MapWidth, MapHeight];
    }

    public int MapHeight { get; private set; }
    public int MapWidth { get; private set; }
    private MapCell[,] Map { get; set; }

    private Snake Player { get; set; }

    public void MovePlayer(Direction direction)
    {
        if (Player.Move(direction))
            Update();
    }

    private void Update()
    {
        
    }
}