using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding
{
    // 查找路径
    public List<GridNode> FindPath(GridNode startNode, GridNode targetNode, GridNode[,] grid)
    {
        List<GridNode> openSet = new List<GridNode>();
        HashSet<GridNode> closedSet = new HashSet<GridNode>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            GridNode currentNode = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentNode.FCost || (openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (GridNode neighbor in GetNeighbors(currentNode, grid))
            {
                if (!neighbor.IsWalkable || closedSet.Contains(neighbor))
                {
                    continue;
                }

                int newMovementCostToNeighbor = currentNode.GCost + GetDistance(currentNode, neighbor);
                if (newMovementCostToNeighbor < neighbor.GCost || !openSet.Contains(neighbor))
                {
                    neighbor.GCost = newMovementCostToNeighbor;
                    neighbor.HCost = GetDistance(neighbor, targetNode);
                    neighbor.Parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return null; // 未找到路径
    }

    // 回溯路径
    List<GridNode> RetracePath(GridNode startNode, GridNode endNode)
    {
        List<GridNode> path = new List<GridNode>();
        GridNode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }

        path.Reverse();
        return path;
    }

    // 获取邻居节点
    List<GridNode> GetNeighbors(GridNode node, GridNode[,] grid)
    {
        List<GridNode> neighbors = new List<GridNode>();

        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                if (x == 0 && z == 0)
                {
                    continue;
                }

                int checkX = node.GridX + x;
                int checkZ = node.GridZ + z;

                if (checkX >= 0 && checkX < grid.GetLength(0) && checkZ >= 0 && checkZ < grid.GetLength(1))
                {
                    neighbors.Add(grid[checkX, checkZ]);
                }
            }
        }

        return neighbors;
    }

    // 计算节点间的距离
    int GetDistance(GridNode nodeA, GridNode nodeB)
    {
        int distX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
        int distZ = Mathf.Abs(nodeA.GridZ - nodeB.GridZ);

        if (distX > distZ)
            return 14 * distZ + 10 * (distX - distZ);
        return 14 * distX + 10 * (distZ - distX);
    }
}
