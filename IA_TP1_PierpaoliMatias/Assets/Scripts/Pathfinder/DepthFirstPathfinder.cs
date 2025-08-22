using System.Collections.Generic;
using UnityEngine;

public class DepthFirstPathfinder<NodeType> : Pathfinder<NodeType> where NodeType : INode
{
    private ICollection<NodeType> graphRef;
    private string name = "Depth Fist Algorithm";

    public override List<NodeType> FindPath(NodeType startNode, NodeType destinationNode, ICollection<NodeType> graph)
    {
        graphRef = graph;
        return base.FindPath(startNode, destinationNode, graph);
    }

    protected override int Distance(NodeType A, NodeType B) => 0;

    public override string GetName() => name;

    protected override ICollection<NodeType> GetNeighbors(NodeType node)
    {
        var result = new List<NodeType>();

        var n = node as Node<Vector2Int>;
        var p = n.GetCoordinate();

        // 4 direcciones
        var deltas = new Vector2Int[]
        {
            new Vector2Int(-1, 0), // izquiereda
            new Vector2Int( 0, 1), // arriba
            new Vector2Int( 0,-1), // abajo
            new Vector2Int( 1, 0) // derecha
        };

        foreach (var d in deltas)
        {
            var q = p + d;

            foreach (var candidate in graphRef)
            {
                var c = candidate as Node<Vector2Int>;
                if (c.GetCoordinate() == q)
                {
                    result.Add(candidate);
                    break;
                }
            }
        }
        return result;
    }

    protected override bool IsBloqued(NodeType node) => node.IsBloqued();

    protected override int MoveToNeighborCost(NodeType A, NodeType b) => -1;

    protected override bool NodesEquals(NodeType A, NodeType B)
    {
        var a = A as Node<Vector2Int>;
        var b = B as Node<Vector2Int>;
        return a.GetCoordinate() == b.GetCoordinate();
    }
}
