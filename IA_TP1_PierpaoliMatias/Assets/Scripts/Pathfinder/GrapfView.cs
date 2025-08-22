using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class GrapfView : MonoBehaviour
{
    public Vector2IntGrapf<Node<Vector2Int>> grapf;
    public Vector2Int[] blockedNodes;
    [NonSerialized] public Node<Vector2Int> startNode;
    [NonSerialized] public Node<Vector2Int> destinationNode;

    [Header("Grid")]
    [SerializeField] private int originX;
    [SerializeField] private int originY;
    [SerializeField] private int gridSizeX;
    [SerializeField] private int gridSizeY;

    [Header("Viz")]
    [SerializeField] float gizmoScreenScale = 0.05f;

    void Awake()
    {
        grapf = new Vector2IntGrapf<Node<Vector2Int>>(originX, originY, gridSizeX, gridSizeY);

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
            Vector2Int pos = new Vector2Int(UnityEngine.Random.Range(originX, gridSizeX), UnityEngine.Random.Range(originY, gridSizeY));
            usados.Add(pos); // si ya estaba, no lo agrega, asi que no hay repetidos
        }

        blockedNodes = new Vector2Int[blockedNodes.Length];
        usados.CopyTo(blockedNodes);
    }

    public Vector3 GridToWorld(Vector2Int c)
        => transform.TransformPoint(new Vector3(c.x * gridSizeX, c.y * gridSizeY, 0));

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || grapf == null) return;

        foreach (var node in grapf.nodes)
        {
            var p = GridToWorld(node.GetCoordinate());

#if UNITY_EDITOR
            // siempre visible sobre todo
            Handles.zTest = CompareFunction.Always;

            // tamaño constante en pantalla
            float s = HandleUtility.GetHandleSize(p) * gizmoScreenScale;

            if (node == startNode) Handles.color = new Color(0.2f, 1f, 0.2f, 1f);
            else if (node == destinationNode) Handles.color = new Color(0.2f, 0.6f, 1f, 1f);
            else if (node.IsBloqued()) Handles.color = new Color(1f, 0.3f, 0.3f, 1f);
            else Handles.color = new Color(0.8f, 0.8f, 0.8f, 0.8f);

            // disco sólido (podés cambiar por DrawSolidRectangleWithOutline si preferís cuadrados)
            Handles.DrawSolidDisc(new Vector3(node.GetCoordinate().x, node.GetCoordinate().y, 0) , Vector3.forward, s);
#else
            Gizmos.color = node.IsBloqued() ? Color.red : Color.gray;
            Gizmos.DrawSphere(p, 0.15f * cellSize);
#endif
        }
    }
}
