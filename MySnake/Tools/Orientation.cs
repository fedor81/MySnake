using System.Collections.Generic;
using MySnake.Model;

namespace MySnake.Tools
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

        private static IEnumerable<Point> GetNeighbors(Point point, int offsetX, int offsetY)
        {
            for (int dx = -offsetX; dx <= offsetX; dx++)
            for (int dy = -offsetY; dy <= offsetY; dy++)
                yield return point.With(dy, dx);
        }

        public static IEnumerable<Point> Get8Neighbors(Point point) => GetNeighbors(point, 1, 1);

        public static IEnumerable<Point> Get24Neighbors(Point point) => GetNeighbors(point, 2, 2);
    }
}