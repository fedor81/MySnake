using System;

namespace MySnake.MapGenerator;

public static class Interpolation
{
    public static float QuinticCurve(float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    public static float Lerp(float t, float a, float b)
    {
        return a + t * (b - a);
    }

    public static float CosineCurve(float t)
    {
        return (float)((1 - Math.Cos(t * Math.PI)) / 2);
    }

    public static float CubicCurve(float t)
    {
        return -2 * t * t * t + 3 * t * t;
    }
}