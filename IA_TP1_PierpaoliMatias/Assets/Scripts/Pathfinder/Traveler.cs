using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AlgorithmTypes
{
    DepthFirst,
    BreadthFirst//,
    //Dijkstra,
    //AStar
}

public class Traveler : MonoBehaviour
{
    public GrapfView grapfView;
    public AlgorithmTypes selectedAlgorithm = AlgorithmTypes.DepthFirst;

    private Pathfinder<Node<Vector2Int>> pathfinder;

    private Node<Vector2Int> startNode; 
    private Node<Vector2Int> destinationNode;

    void Start()
    {
        pathfinder = CreatePathfinder(selectedAlgorithm);

        var libres = grapfView.grapf.nodes.FindAll(n => !n.IsBloqued());
        if (libres.Count < 2) { Debug.LogError("No hay nodos libres suficientes"); return; }

        startNode = libres[Random.Range(0, libres.Count)];
        do
        {
            destinationNode = libres[Random.Range(0, libres.Count)];

        } while (destinationNode == startNode);

        grapfView.startNode = startNode;
        grapfView.destinationNode = destinationNode;

        transform.position = new Vector3(startNode.GetCoordinate().x, startNode.GetCoordinate().y, 0);

        var path = pathfinder.FindPath(startNode, destinationNode, grapfView.grapf.nodes);
        if (path == null) { Debug.LogWarning("No se encontró camino."); return; }

        StartCoroutine(Move(path));
    }

    public IEnumerator Move(List<Node<Vector2Int>> path) 
    {
        foreach (Node<Vector2Int> node in path)
        {
            Vector3 currentPosition = transform.position;
            transform.position = new Vector3(node.GetCoordinate().x, node.GetCoordinate().y);
            Debug.DrawLine(currentPosition, transform.position, Color.green, 30f);
            yield return new WaitForSeconds(1.0f);
        }

        Debug.Log("Destino alcanzado en: " + pathfinder.GetName());
    }

    private Pathfinder<Node<Vector2Int>> CreatePathfinder(AlgorithmTypes selectedAlgorithm)
    {
        switch (selectedAlgorithm)
        {
            case AlgorithmTypes.DepthFirst:
                return new DepthFirstPathfinder<Node<Vector2Int>>();
            case AlgorithmTypes.BreadthFirst:
                return new BreadthFirstPathfinder<Node<Vector2Int>>();
            //case AlgorithmTypes.Dijkstra:
            //    return new DijstraPathfinder<Node<Vector2Int>>();
            //case AlgorithmTypes.AStar:
            //    return new AStarPathfinder<Node<Vector2Int>>();
            default:
                Debug.LogWarning("Algoritmo no soportado, usando DFS por defecto.");
                return new DepthFirstPathfinder<Node<Vector2Int>>();
        }
    }
}
