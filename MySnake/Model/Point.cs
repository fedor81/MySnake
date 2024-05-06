namespace MySnake.Model;

public struct Point
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public Point With((int, int) delta) => With(delta.Item1, delta.Item2);

    public Point With(int deltaX, int deltaY)
    {
        return new Point(X + deltaX, Y + deltaY);
    }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }
}