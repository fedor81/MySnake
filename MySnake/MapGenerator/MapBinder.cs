using System;
using System.Linq;
using Graphs;
using MySnake.Model;

namespace MySnake.MapGenerator;

public class MapBinder
{
    private readonly Random _random;
    private readonly MapGenerator _mapGenerator;
    
    private const int MinNumberOfIslands = 7;
    private const int MxnNumberOfIslands = 12;

    public MapBinder(int seed=0)
    {
        _random = new Random(seed);
        _mapGenerator = new MapGenerator(_random.Next());
    }

    public Graph<GameMap> CreateGameMap()
    {
        var numberIslands = _random.Next(MinNumberOfIslands, MxnNumberOfIslands);
        var graph = new Graph<GameMap>(
            Enumerable.Range(0, numberIslands).Select(_ => _mapGenerator.GenerateRandomMap()));
        
        graph.ConnectAllNodes();
        
        // TODO: Нужно выдавать одинаковые веса в двух направлениях
        foreach (var edge in graph.GetEdges()) 
            edge.Weight = _random.Next();

        // TODO: Нужно брать ребра в обе стороны либо соеденить существующие
        graph = WorkWithGraphs.GetMinimumSpanningTreeByKruskal(graph);

        return graph;
    }

    // TODO
    private void MakePassBetweenMaps()
    {
    }
}