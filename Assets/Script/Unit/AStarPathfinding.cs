using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding
{
    public static List<Node> FindPath(int[,] map, Vector2Int startPos, Vector2Int targetPos)
    {
        List<Node> openList = new List<Node>(); // 开放列表
        HashSet<Node> closedList = new HashSet<Node>(); // 闭合列表
        Node startNode = new Node(startPos, null, 0, GetDistance(startPos, targetPos));
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Node currentNode = openList[0];
            foreach (Node node in openList)
            {
                if (node.FCost < currentNode.FCost || (node.FCost == currentNode.FCost && node.HCost < currentNode.HCost))
                    currentNode = node;
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            // 找到目标位置后开始打印路径
            if (currentNode.Position == targetPos)
            {
                List<Node> path = RetracePath(startNode, currentNode);

                // 输出路径中的节点信息
                Debug.Log("Path found:");
                foreach (Node node in path)
                {
                    Debug.Log(node); // 逐个输出节点信息
                }

                return path;
            }

            foreach (Vector2Int direction in GetNeighbourPositions(currentNode.Position, map))
            {
                if (map[direction.x, direction.y] == 1 || closedList.Contains(new Node(direction))) // 忽略障碍或已处理的节点
                    continue;

                int newMovementCost = currentNode.GCost + GetDistance(currentNode.Position, direction);
                Node neighbourNode = new Node(direction, currentNode, newMovementCost, GetDistance(direction, targetPos));

                if (newMovementCost < neighbourNode.GCost || !openList.Contains(neighbourNode))
                {
                    neighbourNode.GCost = newMovementCost;
                    neighbourNode.Parent = currentNode;

                    if (!openList.Contains(neighbourNode))
                        openList.Add(neighbourNode);
                }
            }
        }
        return null; // 找不到路径时返回空
    }

    // 返回路径
    private static List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        path.Reverse();
        return path;
    }

    // 获取相邻节点位置
    private static List<Vector2Int> GetNeighbourPositions(Vector2Int nodePos, int[,] map)
    {
        List<Vector2Int> neighbours = new List<Vector2Int>();

        if (nodePos.x + 1 < map.GetLength(0)) neighbours.Add(new Vector2Int(nodePos.x + 1, nodePos.y));
        if (nodePos.x - 1 >= 0) neighbours.Add(new Vector2Int(nodePos.x - 1, nodePos.y));
        if (nodePos.y + 1 < map.GetLength(1)) neighbours.Add(new Vector2Int(nodePos.x, nodePos.y + 1));
        if (nodePos.y - 1 >= 0) neighbours.Add(new Vector2Int(nodePos.x, nodePos.y - 1));

        return neighbours;
    }

    // 计算距离（曼哈顿距离）
    public static int GetDistance(Vector2Int posA, Vector2Int posB)
    {
        return Mathf.Abs(posA.x - posB.x) + Mathf.Abs(posA.y - posB.y);
    }
}

public class Node
{
    public Vector2Int Position;   // 节点的坐标
    public Node Parent;           // 父节点
    public int GCost;             // 从起点到该节点的代价
    public int HCost;             // 从该节点到终点的估算代价
    public int FCost => GCost + HCost; // 总代价（G + H）

    public Node(Vector2Int pos, Node parent = null, int gCost = 0, int hCost = 0)
    {
        Position = pos;
        Parent = parent;
        GCost = gCost;
        HCost = hCost;
    }

    public override bool Equals(object obj)
    {
        if (obj is Node node)
            return Position == node.Position;
        return false;
    }

    public override int GetHashCode()
    {
        return Position.GetHashCode();
    }
    public override string ToString()
    {
        return $"Node(Position: {Position.x}, {Position.y})";
    }

}
