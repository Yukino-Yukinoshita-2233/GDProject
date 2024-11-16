using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector2Int Position;    // 节点在地图中的坐标
    public Node Parent;            // 该节点的父节点，用于路径回溯
    public int GCost;              // 起点到当前节点的路径代价
    public int HCost;              // 当前节点到目标节点的估计代价
    public int FCost => GCost + HCost;  // 总代价 (GCost + HCost)

    // 构造函数
    public Node(Vector2Int position, Node parent, int gCost, int hCost)
    {
        Position = position;
        Parent = parent;
        GCost = gCost;
        HCost = hCost;
    }

    // 用于比较节点是否相等的重载方法，判断坐标是否相同
    public override bool Equals(object obj)
    {
        if (obj is Node node)
            return Position == node.Position;
        return false;
    }

    // 用于 HashSet 存储的哈希函数
    public override int GetHashCode()
    {
        return Position.GetHashCode();
    }
}

public class AStarPathfinding
{
    // A* 寻路算法，返回起点到终点的路径
    public static List<Node> FindPath(int[,] map, Vector2Int startPos, Vector2Int targetPos)
    {
        List<Node> openList = new List<Node>(); // 开放列表，存储待处理节点
        HashSet<Node> closedList = new HashSet<Node>(); // 闭合列表，存储已处理节点

        // 创建起始节点，并将其加入开放列表
        Node startNode = new Node(startPos, null, 0, GetDistance(startPos, targetPos));
        openList.Add(startNode);

        // 开始处理开放列表中的节点
        while (openList.Count > 0)
        {
            // 选择具有最小 FCost 的节点作为当前节点
            Node currentNode = openList[0];
            foreach (Node node in openList)
            {
                if (node.FCost < currentNode.FCost || (node.FCost == currentNode.FCost && node.HCost < currentNode.HCost))
                    currentNode = node;
            }

            // 从开放列表中移除当前节点，并将其加入闭合列表
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            // 如果当前节点为目标节点，构建并返回路径
            if (currentNode.Position == targetPos)
                return RetracePath(startNode, currentNode);

            // 遍历当前节点的所有相邻节点
            foreach (Vector2Int neighborPos in GetNeighborPositions(currentNode.Position, map))
            {
                // 忽略障碍物和已经处理过的节点
                if (map[neighborPos.x, neighborPos.y] != 0 || closedList.Contains(new Node(neighborPos, null, 0, 0))) // 添加缺少的参数
                    continue;

                // 计算邻居节点的 GCost（从起点到该邻居的代价）
                int newMovementCostToNeighbor = currentNode.GCost + GetDistance(currentNode.Position, neighborPos);
                Node neighborNode = new Node(neighborPos, currentNode, newMovementCostToNeighbor, GetDistance(neighborPos, targetPos));

                // 如果该邻居节点不在开放列表中，或新的路径代价更小，更新该邻居节点
                if (newMovementCostToNeighbor < neighborNode.GCost || !openList.Contains(neighborNode))
                {
                    neighborNode.GCost = newMovementCostToNeighbor;
                    neighborNode.Parent = currentNode;

                    // 如果邻居节点不在开放列表中，将其加入
                    if (!openList.Contains(neighborNode))
                        openList.Add(neighborNode);
                }
            }
        }

        // 如果开放列表为空且未找到路径，返回 null 表示无路径
        return null;
    }

    // 构建路径：从目标节点沿 Parent 指针回溯到起始节点
    private static List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        // 回溯到起点，构建完整路径
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        path.Reverse(); // 路径反转，使路径从起点到终点
        return path;
    }

    // 获取当前节点四个方向（上下左右）相邻的节点坐标
    private static List<Vector2Int> GetNeighborPositions(Vector2Int position, int[,] map)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        // 检查相邻格子是否在地图范围内，并添加到邻居列表
        if (position.x + 1 < map.GetLength(0)) neighbors.Add(new Vector2Int(position.x + 1, position.y));
        if (position.x - 1 >= 0) neighbors.Add(new Vector2Int(position.x - 1, position.y));
        if (position.y + 1 < map.GetLength(1)) neighbors.Add(new Vector2Int(position.x, position.y + 1));
        if (position.y - 1 >= 0) neighbors.Add(new Vector2Int(position.x, position.y - 1));

        return neighbors;
    }

    // 计算两个节点之间的曼哈顿距离，用于估算 HCost
    private static int GetDistance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y); // 曼哈顿距离
    }
}
