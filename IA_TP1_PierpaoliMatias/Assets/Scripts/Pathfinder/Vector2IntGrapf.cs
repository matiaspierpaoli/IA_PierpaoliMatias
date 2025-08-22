using System.Collections.Generic;

public class Vector2IntGrapf<NodeType> 
    where NodeType : INode<UnityEngine.Vector2Int>, INode, new()
{ 
    public List<NodeType> nodes = new List<NodeType>();

    public Vector2IntGrapf(int originX, int originY, int gridSizeX, int gridSizeY) 
    {
        for (int i = originX; i < originX + gridSizeX; i++)
        {
            for (int j = originY; j < originY + gridSizeY; j++)
            {
                NodeType node = new NodeType();
                node.SetCoordinate(new UnityEngine.Vector2Int(i, j));
                nodes.Add(node);
            }
        }
    }

}
