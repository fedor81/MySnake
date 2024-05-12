using System;
using System.Collections.Generic;
using System.Linq;

namespace MySnake.Model;

public class Graph<T>
{
    public readonly HashSet<Node<T>> Nodes;
    public readonly HashSet<Edge<T>> Edges = new();

    public Graph(IEnumerable<T> values)
    {
        Nodes = values.Select(value => new Node<T>(value)).ToHashSet();
    }

    public void AddNode(T value)
    {
        Nodes.Add(new Node<T>(value));
    }

    public void RemoveEdge(Edge<T> edge)
    {
        if (!Edges.Contains(edge))
            throw new Exception("Edge is not contained in the graph");

        Edges.Remove(edge);
        edge.DeleteEdge();
    }

    public void RemoveEdge(Node<T> from, Node<T> to)
    {
        if (!Nodes.Contains(from) || !Nodes.Contains(to))
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

        Nodes.Remove(node);
    }

    public bool IsConnected(Node<T> from, Node<T> to)
    {
        return Edges.Contains(new Edge<T>(from, to));
    }

    public bool IsConnectedBidirectionally(Node<T> node1, Node<T> node2)
    {
        return IsConnected(node1, node2) && IsConnected(node2, node1);
    }

    public IEnumerable<Node<T>> GetNeighbors(Node<T> node)
    {
        if (!Nodes.Contains(node))
            throw new Exception("Node is not contained in the graph");

        foreach (var edge in node.Edges)
            yield return edge.To;
    }

    public bool Connect(Node<T> from, Node<T> to)
    {
        if (from.Equals(to))
            throw new Exception("Node cannot be connected to itself");
        if (!Nodes.Contains(from) || !Nodes.Contains(to))
            throw new Exception("The nodes are not in the graph");

        var edge = new Edge<T>(from, to);
        edge.Save();
        return Edges.Add(edge);
    }

    public void ConnectBidirectionally(Node<T> node1, Node<T> node2)
    {
        Connect(node1, node2);
        Connect(node2, node1);
    }

    public void ConnectAllNodes()
    {
        foreach (var node1 in Nodes)
        foreach (var node2 in Nodes.Where(node2 => node1 != node2))
            ConnectBidirectionally(node1, node2);
    }
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

    public Edge(Node<T> from, Node<T> to)
    {
        From = from;
        To = to;
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
    
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        var edge = (Edge<T>) obj;
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