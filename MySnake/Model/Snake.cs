using System;
using System.Collections.Generic;

namespace MySnake.Model;

public class Snake
{
    public event EventHandler<Point> TailRemoved;
    public event EventHandler<Point> HeadMoved;
    public event Action<Snake> SnakeGotDamage;

    public Point Head { get; private set; }
    public IEnumerable<Point> Body => _body;
    public int Length => _body.Count + 1;
    private readonly Queue<Point> _body = new();

    private Direction? _previousMove;
    private bool _grow;

    public bool CanMove(Direction direction) =>
        _previousMove == null || direction != Orientation.OppositeMoves[(Direction)_previousMove];

    public Snake(int x, int y, int length)
    {
        Head = new Point(x, y);
        for (int i = 1; i < length; i++) _body.Enqueue(new Point(x, y));
    }

    public void Move(Direction direction)
    {
        _previousMove = direction;
        Move(Orientation.DirectionToMove[direction]);
    }

    private void Move((int, int) move) => Move(move.Item1, move.Item2);

    private void Move(int x, int y)
    {
        _body.Enqueue(Head);
        Head = Head.With(x, y);
        HeadMoved?.Invoke(this, Head);

        if (_grow)
            _grow = false;
        else
            RemoveTail();
    }

    public void Grow() => _grow = true;
    private void RemoveTail() => TailRemoved?.Invoke(this, _body.Dequeue());

    public void GetDamage()
    {
        RemoveTail();
        SnakeGotDamage?.Invoke(this);
    }
}