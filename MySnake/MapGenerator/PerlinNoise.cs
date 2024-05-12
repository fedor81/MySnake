using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MySnake.Model;

public class PerlinNoise
{
    private const int TableSize = 256;
    private const int GradientVectorCount = 4;
    private readonly int[] _permutationTable;

    public PerlinNoise(int seed = 0)
    {
        var random = new Random(seed);
        _permutationTable = Enumerable.Range(0, TableSize).OrderBy(_ => random.Next()).ToArray();
    }

    private Vector2 GetPseudoRandomGradientVector(int x, int y)
    {
        var v = _permutationTable[(y + _permutationTable[x % TableSize]) % TableSize] % GradientVectorCount;

        return v switch
        {
            0 => new Vector2(1, 0),
            1 => new Vector2(-1, 0),
            2 => new Vector2(0, 1),
            _ => new Vector2(0, -1)
        };
    }

    public float GetNoise(float x, float y)
    {
        var left = (int)Math.Floor(x);
        var top = (int)Math.Floor(y);

        var localX = x - left;
        var localY = y - top;

        var topLeftGradient = GetPseudoRandomGradientVector(left, top);
        var topRightGradient = GetPseudoRandomGradientVector(left + 1, top);
        var bottomLeftGradient = GetPseudoRandomGradientVector(left, top + 1);
        var bottomRightGradient = GetPseudoRandomGradientVector(left + 1, top + 1);

        var topLeftVectorToPoint = new Vector2(localX, localY);
        var topRightVectorToPoint = new Vector2(localX - 1, localY);
        var bottomLeftVectorToPoint = new Vector2(localX, localY - 1);
        var bottomRightVectorToPoint = new Vector2(localX - 1, localY - 1);

        var dotTopLeft = Vector2.Dot(topLeftVectorToPoint, topLeftGradient);
        var dotTopRight = Vector2.Dot(topRightVectorToPoint, topRightGradient);
        var dotBottomLeft = Vector2.Dot(bottomLeftVectorToPoint, bottomLeftGradient);
        var dotBottomRight = Vector2.Dot(bottomRightVectorToPoint, bottomRightGradient);

        localX = Interpolation.QuinticCurve(localX);
        localY = Interpolation.QuinticCurve(localY);

        var topInterpolation = Interpolation.Lerp(dotTopLeft, dotTopRight, localX);
        var bottomInterpolation = Interpolation.Lerp(dotBottomLeft, dotBottomRight, localX);
        var finalNoiseValue = Interpolation.Lerp(topInterpolation, bottomInterpolation, localY);

        return finalNoiseValue;
    }
}