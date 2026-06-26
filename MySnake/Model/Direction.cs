using System.Collections.Generic;

namespace MySnake.Model;

public enum Direction
{
    Right,
    Left,
    Up,
    Down
}

public static class DirectionHelper
{
    public static readonly List<Direction> AllDirections = new() { Direction.Right, Direction.Left, Direction.Up, Direction.Down };
}