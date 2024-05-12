namespace Graphs;

public class Graph<T>
{
    private readonly HashSet<Node<T>> _nodes = new();
    private readonly HashSet<Edge<T>> _edges = new();
    public int CountNodes => _nodes.Count;
    public int CountEdges => _edges.Count;

    public Graph()
    {
    }

    public Graph(IEnumerable<T> values)
    {
        foreach (var value in values)
            _nodes.Add(new Node<T>(value));
    }

    public Node<T> AddNode(T value)
    {
        var node = new Node<T>(value);
        _nodes.Add(node);
        return node;
    }

    public void RemoveEdge(Edge<T> edge)
    {
        if (!_edges.Contains(edge))
            throw new Exception("Edge is not contained in the graph");

        _edges.Remove(edge);
        edge.DeleteEdge();
    }

    public void RemoveEdge(Node<T> from, Node<T> to)
    {
        if (!_nodes.Contains(from) || !_nodes.Contains(to))
            throw new Exception("The nodes are not in the graph");

        RemoveEdge(new Edge<T>(from, to));
    }

    public void RemoveEdgeBidirectionally(Node<T> node1, Node<T> node2)
    {
        if (IsConnected(node1, node2))
            RemoveEdge(node1, node2);
        if (IsConnected(node2, node1))
            RemoveEdge(node2, node1);
    }

    public void RemoveNode(Node<T> node)
    {
        foreach (var edge in node.Edges)
            RemoveEdgeBidirectionally(edge.From, edge.To);

        _nodes.Remove(node);
    }

    public bool IsConnected(Node<T> from, Node<T> to)
    {
        return _edges.Contains(new Edge<T>(from, to));
    }

    public bool IsConnectedBidirectionally(Node<T> node1, Node<T> node2)
    {
        return IsConnected(node1, node2) && IsConnected(node2, node1);
    }

    public IEnumerable<Node<T>> GetNeighbors(Node<T> node)
    {
        if (!_nodes.Contains(node))
            throw new Exception("Node is not contained in the graph");

        foreach (var edge in node.Edges)
            yield return edge.To;
    }

    public bool Connect(Node<T> from, Node<T> to, int weight=0)
    {
        if (from.Equals(to))
            throw new Exception("Node cannot be connected to itself");
        if (!_nodes.Contains(from) || !_nodes.Contains(to))
            throw new Exception("The nodes are not in the graph");

        var edge = new Edge<T>(from, to, weight);
        edge.Save();
        return _edges.Add(edge);
    }

    public void ConnectBidirectionally(Node<T> node1, Node<T> node2, int weight=0)
    {
        Connect(node1, node2, weight);
        Connect(node2, node1, weight);
    }

    public void ConnectAllNodes()
    {
        foreach (var node1 in _nodes)
        foreach (var node2 in _nodes.Where(node2 => node1 != node2))
            ConnectBidirectionally(node1, node2);
    }

    public bool Contains(Node<T> node) => _nodes.Contains(node);
    public bool Contains(Edge<T> edge) => _edges.Contains(edge);

    public IEnumerable<Node<T>> GetNodes() => _nodes;
    public IEnumerable<Edge<T>> GetEdges() => _edges;
}

public class Node<T>
{
    public T Value { get; set; }
    internal readonly HashSet<Edge<T>> Edges = new();

    public Node(T value)
    {
        Value = value;
    }

    internal void RemoveEdge(Edge<T> edge)
    {
        if (!Edges.Remove(edge))
            throw new Exception("Node does not contain an edge");
    }
}

public class Edge<T>
{
    public Node<T> From { get; }
    public Node<T> To { get; }
    public int Weight { get; set; }

    public Edge(Node<T> from, Node<T> to, int weight=0)
    {
        From = from;
        To = to;
        Weight = weight;
    }

    internal void Save()
    {
        From.Edges.Add(this);
    }

    internal void DeleteEdge()
    {
        From.RemoveEdge(this);
    }

    public bool Equals(Edge<T> edge)
    {
        return From.Equals(edge.From) && To.Equals(edge.To);
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        var edge = (Edge<T>)obj;
        return From.Equals(edge.From) && To.Equals(edge.To);
    }

    public static bool operator ==(Edge<T> edge1, Edge<T> edge2)
    {
        return edge1.Equals(edge2);
    }

    public static bool operator !=(Edge<T> edge1, Edge<T> edge2)
    {
        return !edge1.Equals(edge2);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hash = 17;

            hash = hash * 31 + From.GetHashCode();
            hash = hash * 31 + To.GetHashCode();

            return hash;
        }
    }
}