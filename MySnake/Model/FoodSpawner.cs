using System;

namespace MySnake.Model;

public class FoodSpawner
{
    public Action SpawnFood;
    private double StartFoodSpawnFactor { get; set; }
    private double FoodSpawnFactor { get; set; }
    private int NumberFoodEaten { get; set; }
    private Func<int, double> _getDelta;
    private int TimeSinceLastSpawn { get; set; }

    public FoodSpawner(double startFoodSpawnFactor)
    {
        StartFoodSpawnFactor = startFoodSpawnFactor;

        // TODO: Случайный выбор функции спавна еды
        _getDelta = Exp;

        NumberFoodEaten = -1;
        EatFood();
    }

    private static readonly Func<int, double> Sin = n => 1 + n * Math.Abs(Math.Sin(n)) / 5;
    private static readonly Func<int, double> Sigma = n => n / 10.0 / (1 + Math.Exp(-n));
    private static readonly Func<int, double> Log2 = n => 3 * Math.Log2(n + 1);
    private static readonly Func<int, double> Sqrt = n => Math.Sqrt(n);

    private static readonly Func<int, double> Exp = n =>
    {
        const double slow = 30.0;
        return Math.Exp(n / slow);
    };

    public void EatFood()
    {
        NumberFoodEaten++;
        FoodSpawnFactor = StartFoodSpawnFactor * _getDelta(NumberFoodEaten);
    }

    public void Update(int gameTime)
    {
        if (gameTime - TimeSinceLastSpawn < FoodSpawnFactor) return;
        TimeSinceLastSpawn = gameTime;
        SpawnFood?.Invoke();
    }
}