using System;
using System.Collections.Generic;
using UnityEngine;

public class GrapfView : MonoBehaviour
{
    public Vector2IntGrapf<Node<Vector2Int>> grapf;
    public Vector2Int[] blockedNodes;
    [NonSerialized] public Node<Vector2Int> startNode;
    [NonSerialized] public Node<Vector2Int> destinationNode;

    void Awake()
    {
        grapf = new Vector2IntGrapf<Node<Vector2Int>>(10, 10);

        foreach (var coord in blockedNodes)
        {
            var node = grapf.nodes.Find(n => n.GetCoordinate() == coord);
            if (node != null) node.SetBlocked(true);
        }
    }

    [ContextMenu("Randomize Blocked Nodes")]
    void RandomizeBlockedNodes()
    {
        HashSet<Vector2Int> usados = new HashSet<Vector2Int>();

        while (usados.Count < blockedNodes.Length)
        {
            Vector2Int pos = new Vector2Int(UnityEngine.Random.Range(0, 10), UnityEngine.Random.Range(0, 10));
            usados.Add(pos); // si ya estaba, no lo agrega, asi que no hay repetidos
        }

        blockedNodes = new Vector2Int[blockedNodes.Length];
        usados.CopyTo(blockedNodes);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;
        foreach (Node<Vector2Int> node in grapf.nodes)
        {
            if (node == startNode)
                Gizmos.color = Color.green;
            else if (node == destinationNode)
                Gizmos.color = Color.blue;
            else if (node.IsBloqued())
                Gizmos.color = Color.red;
            else
                Gizmos.color = Color.gray;
            
            Gizmos.DrawWireSphere(new Vector3(node.GetCoordinate().x, node.GetCoordinate().y), 0.1f);
        }
    }
}
