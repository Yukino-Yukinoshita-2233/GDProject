using System.Collections.Generic;
using UnityEngine;

public class ThetaStarNode
{
    public Vector2Int Position;        // 节点在地图中的坐标
    public ThetaStarNode Parent;       // 父节点，用于路径回溯
    public int GCost;                  // 起点到当前节点的路径代价
    public int HCost;                  // 当前节点到目标节点的估算代价
    public int FCost => GCost + HCost; // 总代价 (GCost + HCost)

    public ThetaStarNode(Vector2Int position, ThetaStarNode parent, int gCost, int hCost)
    {
        Position = position;
        Parent = parent;
        GCost = gCost;
        HCost = hCost;
    }

    // 判断两个节点是否相等（用于 HashSet 等集合操作）
    public override bool Equals(object obj)
    {
        if (obj is ThetaStarNode node)
            return Position == node.Position;
        return false;
    }

    public override int GetHashCode()
    {
        return Position.GetHashCode();
    }
}

public static class ThetaStar
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

    public static List<ThetaStarNode> FindPath(int[,] map, Vector2Int startPos, Vector2Int targetPos)
    {
        int mapWidth = map.GetLength(0);
        int mapHeight = map.GetLength(1);

        // 开放列表和关闭列表
        List<ThetaStarNode> openList = new List<ThetaStarNode>();
        HashSet<Vector2Int> closedList = new HashSet<Vector2Int>();

        // 起点和终点节点
        ThetaStarNode startNode = new ThetaStarNode(startPos, null, 0, Heuristic(startPos, targetPos));
        ThetaStarNode endNode = null;

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            // 找到开放列表中 `FCost` 最小的节点
            ThetaStarNode current = GetNodeWithLowestCost(openList);

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

                // 检查邻居是否在地图范围内和是否是障碍
                if (neighborPos.x < 0 || neighborPos.x >= mapWidth ||
                    neighborPos.y < 0 || neighborPos.y >= mapHeight ||
                    map[neighborPos.x, neighborPos.y] != 0 ||  // 只有 `0` 表示可行走
                    closedList.Contains(neighborPos))          // 是否在关闭列表中
                {
                    continue;
                }

                ThetaStarNode neighbor = new ThetaStarNode(neighborPos, null, 0, 0);

                // 计算邻居的新 GCost
                int tentativeGCost = current.GCost + GetDistance(current.Position, neighborPos);

                // 检查直线可见性
                if (current.Parent != null && IsLineOfSight(map, current.Parent.Position, neighborPos))
                {
                    tentativeGCost = current.Parent.GCost + GetDistance(current.Parent.Position, neighborPos);
                    neighbor.Parent = current.Parent;
                }
                else
                {
                    neighbor.Parent = current;
                }

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
        return endNode != null ? BuildPath(endNode) : new List<ThetaStarNode>();
    }

    private static bool IsLineOfSight(int[,] map, Vector2Int start, Vector2Int end)
    {
        // 使用 Bresenham's Line Algorithm 检查直线是否可见
        int x0 = start.x, y0 = start.y;
        int x1 = end.x, y1 = end.y;

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);

        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;

        int err = dx - dy;

        while (true)
        {
            if (map[x0, y0] != 0) // 如果路径中有障碍
                return false;

            if (x0 == x1 && y0 == y1)
                break;

            int e2 = 2 * err;

            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }

            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }

        return true;
    }

    private static ThetaStarNode GetNodeWithLowestCost(List<ThetaStarNode> openList)
    {
        ThetaStarNode bestNode = null;
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
        // 使用曼哈顿距离作为启发式
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private static int GetDistance(Vector2Int a, Vector2Int b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        return dx + dy;
    }

    private static List<ThetaStarNode> BuildPath(ThetaStarNode endNode)
    {
        List<ThetaStarNode> path = new List<ThetaStarNode>();
        ThetaStarNode current = endNode;

        while (current != null)
        {
            path.Add(current);
            current = current.Parent;
        }

        path.Reverse();
        return path;
    }
}
