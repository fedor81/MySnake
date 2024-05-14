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

    public Graph(IEnumerable<T> values) : this(values.Select(value => new Node<T>(value)))
    {
    }

    public Graph(IEnumerable<Node<T>> nodes)
    {
        foreach (var node in nodes)
            AddNode(node);
    }

    public Node<T> AddNode(T value)
    {
        var node = new Node<T>(value);
        AddNode(node);
        return node;
    }

    public bool AddNode(Node<T> node)
    {
        return _nodes.Add(node);
    }

    public void RemoveEdge(Edge<T> edge)
    {
        if (!_edges.Contains(edge))
            throw new Exception("Edge is not contained in the graph");
        
        _edges.Remove(edge);
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
        if (!_nodes.Contains(node))
            throw new Exception("Node is not contained in the graph");
        
        foreach (var edge in _edges.Where(edge => edge.From.Equals(node) || edge.To.Equals(node)))
            RemoveEdge(edge);

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

        foreach (var edge in _edges.Where(edge => edge.From.Equals(node)))
            yield return edge.To;
    }

    public Edge<T> Connect(Node<T> from, Node<T> to, int weight=0)
    {
        if (from.Equals(to))
            throw new Exception("Node cannot be connected to itself");
        if (!_nodes.Contains(from) || !_nodes.Contains(to))
            throw new Exception("The nodes are not in the graph");
        
        var edge = new Edge<T>(from, to, weight);
        _edges.Add(edge);
        return edge;
    }

    public Edge<T> Connect(Edge<T> edge) => Connect(edge.From, edge.To, edge.Weight);

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

    public bool ContainsNode(Node<T> node) => _nodes.Contains(node);
    public bool ContainsEdge(Edge<T> edge) => _edges.Contains(edge);

    public IEnumerable<Node<T>> GetNodes() => _nodes;
    public IEnumerable<Edge<T>> GetEdges() => _edges;
}

public class Node<T>
{
    public T Value { get; }

    public Node(T value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return $"Node({Value})";
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

    public override string ToString()
    {
        return $"Edge From: {From}, To: {To}, Weight: {Weight}";
    }
}