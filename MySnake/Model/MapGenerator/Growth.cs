using System;

namespace MySnake.Model;

public class Growth
{
    delegate bool ConditionGrowth(bool initialValue, int countNeighbors);

    private Random Random { get; set; }

    public Growth(int seed)
    {
        Random = new Random(seed);
    }

    public bool[,] GrowByGameOfLive(bool[,] initial, int numberIterations, int randomOffset)
    {
        return PerformIterations(initial, (value, count) => (!value && count >= 3) || (value && count is >= 2 and <= 3),
            randomOffset: randomOffset, numberIterations: numberIterations);
    }

    public bool[,] Decrease(bool[,] initial, int numberIterations=1)
    {
        return PerformIterations(initial, (value, count) => count >= 7, numberIterations);
    }

    private bool[,] PerformIterations(bool[,] initial, ConditionGrowth conditionGrowth, int numberIterations,
        int randomOffset = 0)
    {
        var width = initial.GetLength(0);
        var height = initial.GetLength(1);
        var temp = new bool[width, height];

        for (var i = 0; i < numberIterations; i++)
        {
            for (int x = 0; x < width; x += 1 + Random.Next(randomOffset))
            {
                for (int y = 0; y < width; y += 1 + Random.Next(randomOffset))
                {
                    var countNeighbors = 0;

                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            if (Math.Abs(dx) + Math.Abs(dy) != 0 && initial[x + GetToBounds(x, dx, width),
                                    y + GetToBounds(y, dy, height)])
                                countNeighbors++;
                        }
                    }

                    temp[x, y] = conditionGrowth(initial[x, y], countNeighbors);
                }
            }

            (initial, temp) = (temp, initial);
        }

        return initial;


        int GetToBounds(int value, int delta, int maxValue)
        {
            if (value + delta < 0 || value + delta >= maxValue)
                return 0;
            return delta;
        }
    }

    public bool[,] Grow(bool[,] initial, int numberIterations=1)
    {
        return PerformIterations(initial, (value, count) => count > 1, numberIterations);
    }
}