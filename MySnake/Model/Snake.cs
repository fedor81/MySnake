using System.Collections.Generic;

namespace MySnake.Model;

public class Snake
{
    public int Length => _body.Count;
    private readonly Queue<SnakeBody> _body = new();
    private SnakeBody Head => _body.Peek();

    private bool CanMoveUp => _previousMove != Direction.Up;
    private bool CanMoveDown => _previousMove != Direction.Down;
    private bool CanMoveRight => _previousMove != Direction.Right;
    private bool CanMoveLeft => _previousMove != Direction.Left;

    private readonly Dictionary<Direction, int[]> _directionToMove = new()
    {
        [Direction.Up] = new[] { 0, 1 },
        [Direction.Down] = new[] { 0, -1 },
        [Direction.Left] = new[] { -1, 0 },
        [Direction.Right] = new[] { 1, 0 }
    };

    private Direction? _previousMove;
    public bool Grow { get; set; }

    public Snake(int x, int y, int length)
    {
        for (int i = 0; i < length; i++)
        {
            _body.Enqueue(new SnakeBody(x, y));
        }
    }

    public bool Move(Direction direction)
    {
        if (direction == _previousMove) return false;
        _previousMove = direction;
        Move(_directionToMove[direction]);
        return true;
    }

    private void Move(IReadOnlyList<int> move) => Move(move[0], move[1]);

    private void Move(int x, int y)
    {
        _body.Enqueue(Head.With(x, y));

        if (!Grow)
            _body.Dequeue();
        else
            Grow = false;
    }

    private struct SnakeBody
    {
        private int X { get; set; }
        private int Y { get; set; }

        public SnakeBody With(int deltaX, int deltaY)
        {
            return new SnakeBody(X + deltaX, Y + deltaY);
        }

        public SnakeBody(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}