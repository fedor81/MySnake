namespace Graphs;

public static class WorkWithGraphs
{
    public static Graph<T> GetMinimumSpanningTreeByKruskal<T>(Graph<T> graph)
    {
        var mst = new Graph<T>();

        foreach (var edge in graph.GetEdges().OrderBy(edge => edge.Weight))
        {
            if (mst.CountNodes == graph.CountNodes) break;
            if (mst.ContainsEdge(edge) && mst.ContainsEdge(edge)) continue;
            mst.AddNode(edge.From);
            mst.AddNode(edge.To);
            mst.Connect(edge);
        }
        
        return mst;
    }
}