using System.Collections.Generic;
using System.Linq;
using BFG.Core;
using BFG.Graphs;
using NUnit.Framework;
using UnityEngine;
using AssertionException = UnityEngine.Assertions.AssertionException;

namespace Tests.EditMode {
public class TestGraph {
    // ╶╵╴╷┼
    // ┌┐└┘─│
    // ├ ┬ ┴ ┤

    [Test]
    [Timeout(1)]
    public void Test_1() {
        Test(
            new[] {
                "╶╴",
            },
            new() {
                new() {
                    GraphNode.Right,
                    GraphNode.Left,
                },
            }
        );
    }

    [Test]
    [Timeout(1)]
    public void Test_2() {
        Test(
            new[] {
                "┌┐",
                "└┘",
            },
            new() {
                new() {
                    GraphNode.Right | GraphNode.Up,
                    GraphNode.Left | GraphNode.Up,
                },
                new() {
                    GraphNode.Right | GraphNode.Down,
                    GraphNode.Left | GraphNode.Down,
                },
            }
        );
    }

    [Test]
    [Timeout(1)]
    public void Test_3() {
        var graph = new Graph();
        graph.Mark(1, 1, Direction.Right);

        var actual = Graph.Tests.GetNodes(graph);
        Assert.AreEqual(
            new List<List<byte>> {
                new() {
                    GraphNode.Right,
                },
            },
            actual
        );
    }

    [Test]
    [Timeout(1)]
    public void Test_4() {
        var graph = new Graph();
        graph.Mark(1, 1, Direction.Right);
        graph.Mark(0, 0, Direction.Left);

        var actual = Graph.Tests.GetNodes(graph);
        Assert.AreEqual(
            new List<List<byte>> {
                new() {
                    GraphNode.Left,
                    0,
                },
                new() {
                    0,
                    GraphNode.Right,
                },
            },
            actual
        );
    }

    static Graph FromStrings(params string[] strings) {
        var graph = new Graph();

        var height = strings.Length;
        if (height == 0) {
            return graph;
        }

        var width = strings[0].Length;
        foreach (var str in strings) {
            Assert.AreEqual(width, str.Length);
        }

        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                switch (strings[height - y - 1][x]) {
                    case '╶':
                        graph.Mark(x, y, Direction.Right);
                        break;
                    case '╵':
                        graph.Mark(x, y, Direction.Up);
                        break;
                    case '╴':
                        graph.Mark(x, y, Direction.Left);
                        break;
                    case '╷':
                        graph.Mark(x, y, Direction.Down);
                        break;
                    case '┌':
                        graph.Mark(x, y, Direction.Down);
                        graph.Mark(x, y, Direction.Right);
                        break;
                    case '┐':
                        graph.Mark(x, y, Direction.Left);
                        graph.Mark(x, y, Direction.Down);
                        break;
                    case '└':
                        graph.Mark(x, y, Direction.Up);
                        graph.Mark(x, y, Direction.Right);
                        break;
                    case '┘':
                        graph.Mark(x, y, Direction.Up);
                        graph.Mark(x, y, Direction.Left);
                        break;
                    case '─':
                        graph.Mark(x, y, Direction.Left);
                        graph.Mark(x, y, Direction.Right);
                        break;
                    case '│':
                        graph.Mark(x, y, Direction.Up);
                        graph.Mark(x, y, Direction.Down);
                        break;
                    case '├':
                        graph.Mark(x, y, Direction.Up);
                        graph.Mark(x, y, Direction.Down);
                        graph.Mark(x, y, Direction.Right);
                        break;
                    case '┬':
                        graph.Mark(x, y, Direction.Left);
                        graph.Mark(x, y, Direction.Down);
                        graph.Mark(x, y, Direction.Right);
                        break;
                    case '┴':
                        graph.Mark(x, y, Direction.Up);
                        graph.Mark(x, y, Direction.Left);
                        graph.Mark(x, y, Direction.Right);
                        break;
                    case '┤':
                        graph.Mark(x, y, Direction.Up);
                        graph.Mark(x, y, Direction.Down);
                        graph.Mark(x, y, Direction.Left);
                        break;
                    case '┼':
                        graph.Mark(x, y, Direction.Up);
                        graph.Mark(x, y, Direction.Down);
                        graph.Mark(x, y, Direction.Right);
                        graph.Mark(x, y, Direction.Left);
                        break;
                    case '.':
                        break;
                    default:
                        Assert.IsTrue(false);
                        break;
                }
            }
        }

        graph.FinishBuilding();
        return graph;
    }

    void Test(string[] strings, List<List<byte>> expectedNodesGraph) {
        var graph = FromStrings(strings);
        var actual = Graph.Tests.GetNodes(graph);
        Assert.AreEqual(expectedNodesGraph, actual);
    }

    [Test]
    public void Test_GetCenters_Empty1() {
        var strings = new string[] { };
        var graph = FromStrings(strings);

        Assert.Throws<AssertionException>(() => graph.GetCenters());
    }

    [Test]
    public void Test_FinishBuildingThrowsWhenEmpty() {
        var graph = new Graph();
        Assert.Throws<AssertionException>(() => graph.FinishBuilding());
    }

    [Test]
    public void Test_GetCenters_2() {
        var graph = FromStrings("╶╴");

        var expected = new List<Vector2Int> { new(0, 0), new(1, 0) };
        var centers = graph.GetCenters();

        TestUtils.AssertSetEquals(expected, centers);
        Assert.AreEqual(0, graph.Cost(new(0, 0), new(0, 0)));
        Assert.AreEqual(1, graph.Cost(new(0, 0), new(1, 0)));
    }

    [Test]
    public void Test_GetCenters_2_Rotated() {
        var graph = FromStrings(
            "╷",
            "╵"
        );

        var expected = new List<Vector2Int> { new(0, 0), new(0, 1) };
        var centers = graph.GetCenters();

        TestUtils.AssertSetEquals(expected, centers);
    }

    [Test]
    public void Test_GetCenters_3() {
        var graph = FromStrings("╶─╴");

        var expected = new List<Vector2Int> { new(1, 0) };
        var centers = graph.GetCenters();

        TestUtils.AssertSetEquals(expected, centers);
    }

    [Test]
    public void Test_GetCenters_4_1() {
        var graph = FromStrings("╶──╴");

        var expected = new List<Vector2Int> { new(1, 0), new(2, 0) };
        var centers = graph.GetCenters();

        TestUtils.AssertSetEquals(expected, centers);
    }

    [Test]
    public void Test_GetCenters_4_2() {
        var graph = FromStrings(
            ".╷.",
            "╶┴╴"
        );

        var expected = new List<Vector2Int> { new(1, 0) };
        var centers = graph.GetCenters();

        TestUtils.AssertSetEquals(expected, centers);
    }

    [Test]
    public void Test_GetCenters_40() {
        var graph = FromStrings(
            ".╷..",
            ".├┬╴",
            "╶┴┤.",
            "..╵."
        );

        var expected = new List<Vector2Int> {
            new(1, 1),
            new(2, 1),
            new(1, 2),
            new(2, 2),
        };
        var centers = graph.GetCenters();

        TestUtils.AssertSetEquals(expected, centers);
    }

    [Test]
    public void Test_GetCenters_WithOffset() {
        var graph = new Graph();
        graph.Mark(10, 10, Direction.Right);
        graph.Mark(11, 10, Direction.Right);
        graph.Mark(11, 10, Direction.Left);
        graph.Mark(12, 10, Direction.Left);
        graph.FinishBuilding();

        var centers = graph.GetCenters();
        var expected = new List<Vector2Int> { new(11, 10) };

        TestUtils.AssertSetEquals(expected, centers);
    }

    [Test]
    public void Test_IsUndirected_1() {
        var graph = new Graph();
        graph.Mark(0, 0, Direction.Right, false);

        Assert.Throws<AssertionException>(() => graph.FinishBuilding());
    }

    [Test]
    public void Test_IsUndirected_2() {
        var graph = FromStrings("╶╴");
        Assert.IsTrue(Graph.Tests.IsUndirected(graph));
    }

    [Test]
    public void Test_IsUndirected_2_Oriented() {
        var graph = FromStrings("╴╶");
        Assert.IsFalse(Graph.Tests.IsUndirected(graph));
    }

    [Test]
    public void Test_IsUndirected_2_Rotated() {
        var graph = FromStrings(
            "╷",
            "╵"
        );
        Assert.IsTrue(Graph.Tests.IsUndirected(graph));
    }

    [Test]
    public void Test_IsUndirected_10() {
        var graph = FromStrings(
            ".╷..",
            ".├┬╴",
            "╶┴┤.",
            "..╵."
        );

        Assert.IsTrue(Graph.Tests.IsUndirected(graph));
    }

    [Test]
    public void Test_GetShortestPath_1() {
        var graph = FromStrings("╶╴");

        var actual = graph.GetShortestPath(new(0, 0), new(1, 0));
        var expected = new List<Vector2Int> { new(0, 0), new(1, 0) };

        Assert.IsTrue(expected.SequenceEqual(actual));
    }

    [Test]
    public void Test_GetShortestPath_2() {
        var graph = FromStrings(
            ".╷..",
            ".├┬╴",
            "╶┴┤.",
            "..╵."
        );

        var actual = graph.GetShortestPath(new(0, 1), new(3, 2));
        var expected1 = new List<Vector2Int> {
            new(0, 1), new(1, 1), new(2, 1), new(2, 2), new(3, 2),
        };
        var expected2 = new List<Vector2Int> {
            new(0, 1), new(1, 1), new(1, 2), new(2, 2), new(3, 2),
        };

        Assert.IsTrue(
            expected1.SequenceEqual(actual)
            || expected2.SequenceEqual(actual)
        );
        Assert.AreEqual(expected1.Count - 1, graph.Cost(new(0, 1), new(3, 2)));
    }

    [Test]
    public void Test_GetShortestPath_3() {
        var graph = FromStrings(
            ".╷..",
            ".├┬╴",
            "╶┴┤.",
            "..╵."
        );

        var actual = graph.GetShortestPath(new(0, 1), new(1, 3));
        var expected = new List<Vector2Int> {
            new(0, 1), new(1, 1), new(1, 2), new(1, 3),
        };

        Assert.That(actual, Is.EquivalentTo(expected));
        Assert.AreEqual(expected.Count - 1, graph.Cost(new(0, 1), new(1, 3)));
    }

    [Test]
    public void Test_GetShortestPath_WithOffset() {
        var graph = new Graph();
        graph.Mark(10, 10, Direction.Right);
        graph.Mark(11, 10, Direction.Right);
        graph.Mark(11, 10, Direction.Left);
        graph.Mark(12, 10, Direction.Left);
        graph.FinishBuilding();

        var actual = graph.GetShortestPath(new(10, 10), new(12, 10));
        var expected = new List<Vector2Int> {
            new(10, 10), new(11, 10), new(12, 10),
        };

        Assert.That(actual, Is.EquivalentTo(expected));
    }

    [Test]
    public void Test_GetShortestPath_WithOffset_2() {
        var graph = new Graph();
        graph.Mark(8, 7, Direction.Down);
        graph.Mark(8, 6, Direction.Up);
        graph.Mark(8, 6, Direction.Right);
        graph.Mark(9, 6, Direction.Left);
        graph.Mark(9, 6, Direction.Right);
        graph.Mark(10, 6, Direction.Left);
        graph.Mark(10, 6, Direction.Right);
        graph.Mark(11, 6, Direction.Left);
        graph.Mark(13, 13, Direction.Right, false);
        graph.FinishBuilding();

        var actual = graph.GetShortestPath(new(8, 7), new(11, 6));
        var expected = new List<Vector2Int> {
            new(8, 7), new(8, 6), new(9, 6), new(10, 6), new(11, 6),
        };

        Assert.That(actual, Is.EquivalentTo(expected));
    }

    [Test]
    public void Test_GetShortestPath_WithOffset_3() {
        var graph = new Graph();
        graph.Mark(8, 7, Direction.Down);
        graph.Mark(8, 6, Direction.Up);
        graph.Mark(8, 6, Direction.Right);
        graph.Mark(9, 6, Direction.Left);
        graph.Mark(9, 6, Direction.Right);
        graph.Mark(10, 6, Direction.Left);
        graph.Mark(10, 6, Direction.Right);
        graph.Mark(11, 6, Direction.Left);
        graph.FinishBuilding();

        var actual = graph.GetShortestPath(new(8, 7), new(11, 6));
        var expected = new List<Vector2Int> {
            new(8, 7), new(8, 6), new(9, 6), new(10, 6), new(11, 6),
        };

        Assert.That(actual, Is.EquivalentTo(expected));
    }

    [Test]
    public void Test_ContainsNode() {
        var graph = new Graph();
        graph.Mark(7, 10, Direction.Right);

        Assert.IsTrue(graph.Contains(7, 10));
        Assert.IsFalse(graph.Contains(8, 10));
        Assert.IsFalse(graph.Contains(6, 10));
        Assert.IsFalse(graph.Contains(7, 11));
        Assert.IsFalse(graph.Contains(7, 9));
    }
}
}
