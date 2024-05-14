using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Graphs.Tests;

[TestFixture]
[TestOf(typeof(WorkWithGraphs))]
public class WorkWithGraphsTests
{
    [Test]
    public void GetMinimumSpanningTreeByKruskalTest()
    {
        var nodes = Enumerable.Range(0, 7).Select(_ => new Node<int>(0)).ToArray();
        var graph = new Graph<int>(nodes);
        var expected = new List<Edge<int>>();
        
        expected.Add(graph.Connect(nodes[0], nodes[1], 10));
        graph.Connect(nodes[0], nodes[2], 10);
        graph.Connect(nodes[0], nodes[6], 10);
        expected.Add(graph.Connect(nodes[0], nodes[5], 1));
        graph.Connect(nodes[1], nodes[2], 10);
        expected.Add(graph.Connect(nodes[2], nodes[3], 1));
        graph.Connect(nodes[2], nodes[4], 10);
        graph.Connect(nodes[4], nodes[3], 10);
        expected.Add(graph.Connect(nodes[5], nodes[4], 1));
        expected.Add(graph.Connect(nodes[5], nodes[6], 1));
        expected.Add(graph.Connect(nodes[5], nodes[2], 1));

        var actual = WorkWithGraphs.GetMinimumSpanningTreeByKruskal(graph).GetEdges();
        
        CollectionAssert.AreEquivalent(expected, actual);
    }
}