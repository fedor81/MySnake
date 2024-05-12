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

    public void RemoveEdge(Node<T> node1, Node<T> node2)
    {
        if (!Nodes.Contains(node1) || !Nodes.Contains(node2))
            throw new Exception("The vertices are not in the graph");

        RemoveEdge(new Edge<T>(node1, node2));
    }

    public void RemoveNode(Node<T> node)
    {
        foreach (var edge in node.Edges)
            RemoveEdge(edge);

        Nodes.Remove(node);
    }

    public bool IsConnected(Node<T> node1, Node<T> node2)
    {
        return Edges.Contains(new Edge<T>(node1, node2));
    }

    public IEnumerable<Node<T>> GetNeighbors(Node<T> node)
    {
        if (!Nodes.Contains(node))
            throw new Exception("Node is not contained in the graph");

        foreach (var edge in node.Edges)
            yield return edge.Node1.Equals(node) ? edge.Node2 : edge.Node1;
    }

    public bool Connect(Node<T> node1, Node<T> node2)
    {
        if (node1.Equals(node2))
            throw new Exception("Node cannot be connected to itself");
        if (!Nodes.Contains(node1) || !Nodes.Contains(node2))
            throw new Exception("The nodes are not in the graph");

        return Edges.Add(new Edge<T>(node1, node2));
    }

    public void ConnectAllNodes()
    {
        foreach (var node1 in Nodes)
        {
            foreach (var node2 in Nodes)
            {
                if (node1 != node2)
                    Connect(node1, node2);
            }
        }
    }
}

public class Node<T>
{
    public T Value { get; set; }
    public readonly HashSet<Edge<T>> Edges = new();

    public Node(T value)
    {
        Value = value;
    }

    public bool RemoveEdge(Edge<T> edge)
    {
        return Edges.Remove(edge);
    }
}

public class Edge<T>
{
    public Node<T> Node1 { get; }
    public Node<T> Node2 { get; }

    public Edge(Node<T> node1, Node<T> node2)
    {
        Node1 = node1;
        Node2 = node2;
        Node1.Edges.Add(this);
        Node2.Edges.Add(this);
    }

    public void DeleteEdge()
    {
        if (!Node1.RemoveEdge(this) || !Node2.RemoveEdge(this))
            throw new Exception("Nodes do not contain an edge");
    }

    public bool Equals(Edge<T> edge)
    {
        return edge.Node1.Equals(this.Node1) && edge.Node2.Equals(this.Node2) ||
               edge.Node2.Equals(this.Node1) && edge.Node1.Equals(this.Node2);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var node1Hash = Node1.GetHashCode();
            var node2Hash = Node2.GetHashCode();

            var hash = 17;

            hash = hash * 31 + Math.Min(node1Hash, node2Hash);
            hash = hash * 31 + Math.Max(node1Hash, node2Hash);

            return hash;
        }
    }
}