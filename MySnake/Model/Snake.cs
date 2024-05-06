using System;
using System.Collections.Generic;

namespace MySnake.Model;

public class Snake
{
    public event EventHandler<SnakeBody> TailRemoved;
    public event EventHandler<SnakeBody> HeadMoved;

    public SnakeBody Head { get; private set; }
    public IEnumerable<SnakeBody> Body => _body;
    public int Length => _body.Count + 1;
    private readonly Queue<SnakeBody> _body = new();

    public bool CanMove(Direction direction) =>
        _previousMove == null || direction != Orientation.OppositeMoves[(Direction)_previousMove];

    private Direction? _previousMove;
    public bool Grow { get; set; }

    public Snake(int x, int y, int length)
    {
        Head = new SnakeBody(x, y);
        for (int i = 1; i < length; i++) _body.Enqueue(new SnakeBody(x, y));
    }

    public bool Move(Direction direction)
    {
        _previousMove = direction;
        Move(Orientation.DirectionToMove[direction]);
        return true;
    }

    private void Move((int, int) move) => Move(move.Item1, move.Item2);

    private void Move(int x, int y)
    {
        _body.Enqueue(Head);
        Head = Head.With(x, y);
        HeadMoved?.Invoke(this, Head);

        if (Grow)
            Grow = false;
        else
            RemoveTail();
    }

    private void RemoveTail() => TailRemoved?.Invoke(this, _body.Dequeue());

    public void GetDamage()
    {
        RemoveTail();
    }
}

public struct SnakeBody
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public SnakeBody With((int, int) delta) => With(delta.Item1, delta.Item2);

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