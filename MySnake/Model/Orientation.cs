using System.Collections.Generic;

namespace MySnake.Model
{
    public static class Orientation
    {
        public static readonly Dictionary<Direction, (int, int)> DirectionToMove = new()
        {
            [Direction.Up] = (0, -1),
            [Direction.Down] = (0, 1),
            [Direction.Left] = (-1, 0),
            [Direction.Right] = (1, 0)
        };

        public static readonly Dictionary<Direction, Direction> OppositeMoves = new()
        {
            [Direction.Up] = Direction.Down,
            [Direction.Down] = Direction.Up,
            [Direction.Left] = Direction.Right,
            [Direction.Right] = Direction.Left,
        };
    }
}