using System;

namespace MySnake.Model;

public class GameOfLife
{
    private Random random;

    public GameOfLife(int seed)
    {
        random = new Random(seed);
    }

    public bool[,] Get(bool[,] initial, int numberIterations, int randomOffset)
    {
        var width = initial.GetLength(0);
        var height = initial.GetLength(1);
        var temp = new bool[width, height];

        for (int i = 0; i < numberIterations; i++)
        {
            for (int x = 0; x < width; x += 1 + random.Next(randomOffset))
            {
                for (int y = 0; y < width; y += 1 + random.Next(randomOffset))
                {
                    var countNeighbors = 0;
                    
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            if (Math.Abs(dx) + Math.Abs(dy) != 0 && initial[x + GetToBounds(x, dx, width), y + GetToBounds(y, dy, height)])
                                countNeighbors++;
                        }
                    }

                    switch (temp[x, y])
                    {
                        case true when countNeighbors >= 3:
                        case false when countNeighbors is >= 2 and <= 3:
                            temp[x, y] = true;
                            break;
                        default:
                            temp[x, y] = false;
                            break;
                    }
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
}