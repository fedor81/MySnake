namespace Graphs;

public static class WorkWithGraphs
{
    public static Graph<T> GetMinimumSpanningTreeByKruskal<T>(Graph<T> graph)
    {
        var mst = new Graph<T>(graph.GetNodes());
        var unionFind = new UnionFind<Node<T>>(graph.GetNodes());
        
        foreach (var edge in graph.GetEdges().OrderBy(edge => edge.Weight))
        {
            if (mst.CountEdges == mst.CountNodes - 1) break;
            if (unionFind.IsConnected(edge.From, edge.To)) continue;
            
            unionFind.Unite(edge.From, edge.To);
            mst.Connect(edge);
        }

        return mst;
    }
}