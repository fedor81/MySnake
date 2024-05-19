using System;

namespace MySnake.Model;

public class FoodSpawner
{
    public Action SpawnFood;
    private double StartFoodSpawnFactor { get; set; }
    private double FoodSpawnFactor { get; set; }
    private int NumberFoodEaten { get; set; }
    private Func<int, double> _getDelta;
    private int TimeSinceLastEat { get; set; }

    public FoodSpawner(double startFoodSpawnFactor)
    {
        StartFoodSpawnFactor = startFoodSpawnFactor;
        
        // TODO: Случайный выбор функции спавна еды
        _getDelta = Exp;
        
        EatFood();
    }

    private static readonly Func<int, double> Sin = n => 1 + Math.Sin(n);
    private static readonly Func<int, double> Sigma = n => 1 / (1 + Math.Exp(-n));
    private static readonly Func<int, double> Log2 = n => Math.Log2(n + 1);

    private static readonly Func<int, double> Exp = n =>
    {
        const double slow = 100.0;
        return Math.Exp(-n / slow);
    };

    public void EatFood()
    {
        NumberFoodEaten++;
        FoodSpawnFactor = StartFoodSpawnFactor * _getDelta(NumberFoodEaten);
    }
    public void Update(double gameTime)
    {
        // TODO
        if (gameTime % FoodSpawnFactor == 0)
            SpawnFood?.Invoke();
    }
}