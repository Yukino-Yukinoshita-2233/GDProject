using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 表示 LPA* 算法中的节点。
/// </summary>
public class LPAStarNode
{
    public Vector2Int Position;    // 节点在地图中的坐标
    public LPAStarNode Parent;     // 该节点的父节点，用于路径回溯
    public int GCost;              // 从起点到当前节点的实际路径代价
    public int RHS;                // 当前节点的候选路径代价
    public int HCost;              // 当前节点到目标节点的估算代价（启发式）
    public int FCost => Mathf.Min(GCost, RHS) + HCost;  // 总代价，用于排序

    public LPAStarNode(Vector2Int position, int gCost = int.MaxValue, int rhs = int.MaxValue, int hCost = 0)
    {
        Position = position;
        GCost = gCost;
        RHS = rhs;
        HCost = hCost;
        Parent = null;
    }

    public override bool Equals(object obj) => obj is LPAStarNode node && Position.Equals(node.Position);

    public override int GetHashCode() => Position.GetHashCode();
}

/// <summary>
/// 实现 LPA* 算法。
/// </summary>
public static class LPAStar
{
    private static readonly Vector2Int[] Directions = new Vector2Int[]
    {
        new Vector2Int(0, 1),    // 上
        new Vector2Int(1, 0),    // 右
        new Vector2Int(0, -1),   // 下
        new Vector2Int(-1, 0),   // 左
        new Vector2Int(1, 1),    // 右上
        new Vector2Int(1, -1),   // 右下
        new Vector2Int(-1, -1),  // 左下
        new Vector2Int(-1, 1)    // 左上
    };

    public static List<LPAStarNode> FindPath(int[,] map, Vector2Int startPos, Vector2Int targetPos)
    {
        int mapWidth = map.GetLength(0);
        int mapHeight = map.GetLength(1);

        // 开放列表和关闭列表
        List<LPAStarNode> openList = new List<LPAStarNode>();
        HashSet<Vector2Int> closedList = new HashSet<Vector2Int>();

        // 起点和终点节点
        LPAStarNode startNode = new LPAStarNode(startPos, gCost: 0, rhs: 0, hCost: Heuristic(startPos, targetPos));
        LPAStarNode endNode = null;

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            // 找到开放列表中 `FCost` 最小的节点
            LPAStarNode current = GetNodeWithLowestCost(openList);

            // 如果已经到达目标
            if (current.Position == targetPos)
            {
                endNode = current;
                break;
            }

            openList.Remove(current);
            closedList.Add(current.Position);

            // 遍历相邻节点
            foreach (var direction in Directions)
            {
                Vector2Int neighborPos = current.Position + direction;

                // 检查邻居是否在地图范围内
                if (neighborPos.x < 0 || neighborPos.x >= mapWidth ||
                    neighborPos.y < 0 || neighborPos.y >= mapHeight ||
                    map[neighborPos.x, neighborPos.y] != 0 ||  // 非 `0` 视为障碍
                    closedList.Contains(neighborPos))         // 是否在关闭列表中
                {
                    continue;
                }

                LPAStarNode neighbor = new LPAStarNode(neighborPos);

                // 计算邻居的新 GCost
                int tentativeGCost = current.GCost + GetDistance(current.Position, neighborPos);

                // 检查现有路径并修正父节点
                neighbor.Parent = current;

                // 如果邻居已经在开放列表中且新路径更差，跳过
                var existingNeighbor = openList.Find(n => n.Position == neighborPos);
                if (existingNeighbor != null && tentativeGCost >= existingNeighbor.GCost)
                    continue;

                // 更新邻居的代价
                neighbor.GCost = tentativeGCost;
                neighbor.HCost = Heuristic(neighborPos, targetPos);

                if (existingNeighbor == null)
                    openList.Add(neighbor);
            }
        }

        // 如果找到了路径，回溯并返回路径
        return endNode != null ? BuildPath(endNode) : new List<LPAStarNode>();
    }

    private static LPAStarNode GetNodeWithLowestCost(List<LPAStarNode> openList)
    {
        LPAStarNode bestNode = null;
        int lowestCost = int.MaxValue;

        foreach (var node in openList)
        {
            if (node.FCost < lowestCost)
            {
                lowestCost = node.FCost;
                bestNode = node;
            }
        }

        return bestNode;
    }

    private static int Heuristic(Vector2Int a, Vector2Int b)
    {
        // 曼哈顿距离
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private static int GetDistance(Vector2Int a, Vector2Int b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        return dx + dy;
    }

    private static List<LPAStarNode> BuildPath(LPAStarNode endNode)
    {
        List<LPAStarNode> path = new List<LPAStarNode>();
        LPAStarNode current = endNode;

        while (current != null)
        {
            path.Add(current);
            current = current.Parent;
        }

        path.Reverse();
        return path;
    }
}
