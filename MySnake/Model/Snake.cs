using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MySnake.Tools;

namespace MySnake.Model;

public class Snake
{
    public static event EventHandler<Point> TailRemovedStatic;
    public event EventHandler<Point> TailRemoved;
    public static event EventHandler<Point> HeadMovedStatic;
    public event Action<Snake> SnakeGotDamage;
    public static event Action<Snake> SnakeDiedStatic;

    public Point Head { get; private set; }
    public IEnumerable<Point> Body => _body;
    public int Length => _body.Count;
    private readonly Queue<Point> _body = new();
    
    // TODO: Можно увеличивать дальность обзора, когда player кушает какие-нибудь бонусы
    private Func<Point, IEnumerable<Point>> _getNeighbors = Orientation.Get8Neighbors;
    private readonly Queue<HashSet<Point>> _occupiedSpace = new();

    private Direction? _previousMove;
    private bool _grow;

    public int Hunger { get; private set; }
    private int MaxHunger { get; }

    public bool CanMove(Direction direction) =>
        Length == 1 || _previousMove == null || direction != Orientation.OppositeMoves[(Direction)_previousMove];

    public Snake(int x, int y, int length=10, int maxHungerValue=10)
    {
        MaxHunger = maxHungerValue;
        Hunger = MaxHunger;
        Head = new Point(x, y);
        
        for (int i = 0; i < length; i++)
        {
            _body.Enqueue(new Point(x, y));
            SaveOccupiedPointsFromHead();
        }
    }

    public void Move(Direction direction)
    {
        _previousMove = direction;
        Move(Orientation.DirectionToMove[direction]);
    }

    private void Move((int, int) move) => Move(move.Item1, move.Item2);

    private void Move(int x, int y)
    {
        Head = Head.With(x, y);
        _body.Enqueue(Head);
        SaveOccupiedPointsFromHead();
        HeadMovedStatic?.Invoke(this, Head);

        if (_grow)
            _grow = false;
        else
        {
            RemoveTail();
            Hunger--;

            if (Hunger != 0) return;
            GetDamage();
            Hunger = MaxHunger;
        }
    }

    public void EatFood()
    {
        _grow = true;
        Hunger = MaxHunger;
    }

    private void RemoveTail()
    {
        var tail = _body.Dequeue();
        DeleteOccupiedPointsFromTail();

        TailRemovedStatic?.Invoke(this, tail);
        TailRemoved?.Invoke(this, tail);
        
        if (Length == 0)
            SnakeDiedStatic?.Invoke(this);
    }

    public void GetDamage()
    {
        RemoveTail();
        SnakeGotDamage?.Invoke(this);
    }

    public HashSet<Point> GetSpaceOccupiedBySnake()
    {
        var res = new HashSet<Point>();
        foreach (var set in _occupiedSpace) res.UnionWith(set);
        return res;
    }

    private void SaveOccupiedPointsFromHead()
    {
        // TODO: При увеличении дальности обзора, увеличивается количество дублирующихся(ненужных) точек
        _occupiedSpace.Enqueue(_getNeighbors(Head).ToHashSet());
    }

    private void DeleteOccupiedPointsFromTail()
    {
        _occupiedSpace.Dequeue();
    }
}