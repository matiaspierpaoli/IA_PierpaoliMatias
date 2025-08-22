using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Traveler : MonoBehaviour
{
    public GrapfView grapfView;
    
    private DepthFirstPathfinder<Node<Vector2Int>> Pathfinder;
    //private BreadthFirstPathfinder<Node<Vector2Int>> Pathfinder;
    //private DijstraPathfinder<Node<Vector2Int>> Pathfinder;
    //private AStarPathfinder<Node<Vector2Int>> Pathfinder;

    private Node<Vector2Int> startNode; 
    private Node<Vector2Int> destinationNode;

    void Start()
    {
        Pathfinder = new DepthFirstPathfinder<Node<Vector2Int>>();

        var libres = grapfView.grapf.nodes.FindAll(n => !n.IsBloqued());
        if (libres.Count < 2) { Debug.LogError("No hay nodos libres suficientes"); return; }

        startNode = libres[Random.Range(0, libres.Count)];
        destinationNode = libres[Random.Range(0, libres.Count)];

        grapfView.startNode = startNode;
        grapfView.destinationNode = destinationNode;

        transform.position = new Vector3(startNode.GetCoordinate().x, startNode.GetCoordinate().y, 0);

        var path = Pathfinder.FindPath(startNode, destinationNode, grapfView.grapf.nodes);
        if (path == null) { Debug.LogWarning("No se encontró camino."); return; }

        StartCoroutine(Move(path));
    }

    public IEnumerator Move(List<Node<Vector2Int>> path) 
    {
        foreach (Node<Vector2Int> node in path)
        {
            transform.position = new Vector3(node.GetCoordinate().x, node.GetCoordinate().y);
            yield return new WaitForSeconds(1.0f);
        }

        Debug.Log("Destino alcanzado");
    }
}
