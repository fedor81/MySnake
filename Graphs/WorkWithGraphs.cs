namespace Graphs;

public static class WorkWithGraphs
{
    public static Graph<T> GetMinimumSpanningTreeByKruskal<T>(Graph<T> graph)
    {
        var mst = new Graph<T>();
        var addedNodes = new HashSet<Node<T>>();
        var addedEdges = new HashSet<Edge<T>>();

        foreach (var edge in graph.GetEdges().OrderBy(edge => edge.Weight))
        {
            if (!(addedNodes.Contains(edge.From) && addedNodes.Contains(edge.To)))
            {
                addedNodes.Add(edge.From);
                addedNodes.Add(edge.To);
                addedEdges.Add(edge);
            }

            if (addedNodes.Count == graph.CountNodes)
                break;
        }
        
        

        return mst;
    }
}