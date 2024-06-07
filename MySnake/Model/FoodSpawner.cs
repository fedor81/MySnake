using System;
using MySnake.Tools;

namespace MySnake.Model;

public class FoodSpawner
{
    public Action SpawnFood;
    private double StartFoodSpawnFactor { get; set; }
    private double FoodSpawnFactor { get; set; }
    private int NumberFoodEaten { get; set; }
    private readonly Func<int, double> _getDelta;
    private int TimeSinceLastSpawn { get; set; }

    public FoodSpawner(double startFoodSpawnFactor)
    {
        StartFoodSpawnFactor = startFoodSpawnFactor;

        var methodSelector = new RandomMethodSelector(this);
        _getDelta = methodSelector.GetRandomMethod() as Func<int, double>;

        NumberFoodEaten = -1;
        EatFood();
    }

    [SelectableMethod]
    private static double Sin(int n) => 1 + n * Math.Abs(Math.Sin(n)) / 5;

    [SelectableMethod]
    private static double Sigma(int n) => n / 10.0 / (1 + Math.Exp(-n));

    [SelectableMethod]
    private static double Log2(int n) => 3 * Math.Log2(n + 1);

    [SelectableMethod]
    private static double Sqrt(int n) => Math.Sqrt(n);

    [SelectableMethod]
    private static double Exp(int n)
    {
        const double slow = 30.0;
        return Math.Exp(n / slow);
    }

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