using System;
using System.Collections.Generic;

namespace MySnake.Model;

public class Snake
{
    public SnakeBody Head { get; private set; }
    public IEnumerable<SnakeBody> Body => _body;
    public SnakeBody End { get; private set; }
    public int Length => _body.Count + 1;
    private readonly Queue<SnakeBody> _body = new();

    private bool CanMoveUp => _previousMove != Direction.Up;
    private bool CanMoveDown => _previousMove != Direction.Down;
    private bool CanMoveRight => _previousMove != Direction.Right;
    private bool CanMoveLeft => _previousMove != Direction.Left;

    private readonly Dictionary<Direction, int[]> _directionToMove = new()
    {
        [Direction.Up] = new[] { 0, -1 },
        [Direction.Down] = new[] { 0, 1 },
        [Direction.Left] = new[] { -1, 0 },
        [Direction.Right] = new[] { 1, 0 }
    };

    private readonly Dictionary<Direction, Direction> _oppositeMoves = new()
    {
        [Direction.Up] = Direction.Down,
        [Direction.Down] = Direction.Up,
        [Direction.Left] = Direction.Right,
        [Direction.Right] = Direction.Left,
    };

    private Direction? _previousMove;
    public bool Grow { get; set; }

    public Snake(int x, int y, int length)
    {
        Head = new SnakeBody(x, y);
        for (int i = 0; i < length - 1; i++) _body.Enqueue(new SnakeBody(x, y));
    }

    public bool Move(Direction direction)
    {
        if (_previousMove != null && direction == _oppositeMoves[(Direction)_previousMove]) return false;
        _previousMove = direction;
        Move(_directionToMove[direction]);
        return true;
    }

    private void Move(IReadOnlyList<int> move) => Move(move[0], move[1]);

    private void Move(int x, int y)
    {
        _body.Enqueue(Head);
        Head = Head.With(x, y);

        if (Grow)
            Grow = false;
        else
            _body.Dequeue();
    }
}

public struct SnakeBody
{
    public int X { get; private set; }
    public int Y { get; private set; }

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