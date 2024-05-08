using System;

namespace MySnake.Model;

public class Growth
{
    public bool[,] Get(bool[,] initial)
    {
        var width = initial.GetLength(0);
        var height = initial.GetLength(1);
        var temp = new bool[width, height];
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
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

                if (countNeighbors < 7)
                    temp[x, y] = true;
            }
        }

        return temp;
        

        int GetToBounds(int value, int delta, int maxValue)
        {
            if (value + delta < 0 || value + delta >= maxValue)
                return 0;
            return delta;
        }
    }
}