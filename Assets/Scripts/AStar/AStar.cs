using System.Collections.Generic;
using UnityEngine;

public static class AStar
{
    /// <summary>
    /// Builds a path for the room, from the startGridPosition to the endGridPosition,
    /// and adds movement steps to the returned stack.  Returns null if no path is found.
    /// </summary>
    /// <param name="room"></param>
    /// <param name="startGridPosition"></param>
    /// <param name="vector3Int"></param>
    /// <returns></returns>
    public static Stack<Vector3> BuildPath(Room room, Vector3Int startGridPosition, Vector3Int endGridPosition)
    {
        // Adjust positions by lower bounds -- rebases gridPositons to a grid that has a (0,0) origin
        startGridPosition -= (Vector3Int)room.templateLowerBounds;
        endGridPosition -= (Vector3Int)room.templateLowerBounds;

        // Create open list and closed hash set
        List<Node> openNodeList = new List<Node>();
        HashSet<Node> closedNodeHashSet = new HashSet<Node>();

        // Create gridNodes for path finding
        GridNodes gridNodes = new GridNodes(room.templateUpperBounds.x - room.templateLowerBounds.x + 1, room.templateUpperBounds.y - room.templateLowerBounds.y + 1);

        Node startNode = gridNodes.GetGridNode(startGridPosition.x, startGridPosition.y);
        Node targetNode = gridNodes.GetGridNode(endGridPosition.x, endGridPosition.y);

        Node endPathNode = FindShortestPath(startNode, targetNode, gridNodes, openNodeList, closedNodeHashSet, room.instantiatedRoom);

        if (endPathNode != null)
        {
            return CreatePathStack(endPathNode, room);
        }

        return null;
    }
}
