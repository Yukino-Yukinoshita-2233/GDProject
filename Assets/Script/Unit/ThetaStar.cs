using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Theta* 节点类，用于表示地图中的一个格子以及路径计算过程中的各种信息
/// </summary>
public class ThetaStarNode
{
    public Vector2Int Position;        // 当前节点在地图上的坐标
    public ThetaStarNode Parent;       // 当前节点的父节点（用于路径回溯）
    public int GCost;                  // 从起点到当前节点的实际代价
    public int HCost;                  // 从当前节点到目标节点的估算代价（启发函数）
    public int FCost => GCost + HCost; // 总代价 F = G + H，用于优先级判断

    public ThetaStarNode(Vector2Int position, ThetaStarNode parent, int gCost, int hCost)
    {
        Position = position;
        Parent = parent;
        GCost = gCost;
        HCost = hCost;
    }
}

/// <summary>
/// Theta* 路径规划算法类（静态）
/// </summary>
public static class ThetaStar
{
    // 八方向移动：上下左右 + 四个对角方向
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

    /// <summary>
    /// 寻找从起点到目标点的一条路径
    /// </summary>
    /// <param name="map">地图数组（0 为可通行，其他为障碍）</param>
    /// <param name="startPos">起点坐标</param>
    /// <param name="targetPos">目标点坐标</param>
    /// <returns>路径节点列表，如果找不到路径则返回空列表</returns>
    public static List<ThetaStarNode> FindPath(int[,] map, Vector2Int startPos, Vector2Int targetPos)
    {
        int mapWidth = map.GetLength(0);
        int mapHeight = map.GetLength(1);

        // 开放列表（待检查的节点）
        List<ThetaStarNode> openList = new List<ThetaStarNode>();
        // 关闭列表（已经检查过的节点）
        HashSet<Vector2Int> closedList = new HashSet<Vector2Int>();

        // 创建起点节点并加入开放列表
        ThetaStarNode startNode = new ThetaStarNode(startPos, null, 0, Heuristic(startPos, targetPos));
        openList.Add(startNode);

        ThetaStarNode endNode = null; // 最终路径终点

        while (openList.Count > 0)
        {
            // 从开放列表中选取 F 代价最小的节点
            ThetaStarNode current = GetNodeWithLowestCost(openList);

            // 如果当前节点就是目标点，表示路径找到
            if (current.Position == targetPos)
            {
                endNode = current;
                break;
            }

            // 将当前节点从开放列表中移除并加入关闭列表
            openList.Remove(current);
            closedList.Add(current.Position);

            // 遍历当前节点的所有相邻节点
            foreach (var direction in Directions)
            {
                Vector2Int neighborPos = current.Position + direction;

                // 越界检查 + 障碍检查 + 已处理节点跳过
                if (neighborPos.x < 0 || neighborPos.x >= mapWidth ||
                    neighborPos.y < 0 || neighborPos.y >= mapHeight ||
                    map[neighborPos.x, neighborPos.y] != 0 ||
                    closedList.Contains(neighborPos))
                {
                    continue;
                }

                // 创建邻居节点
                ThetaStarNode neighbor = new ThetaStarNode(neighborPos, null, 0, 0);
                // 默认从当前节点走过去的 G 代价
                int tentativeGCost = current.GCost + GetDistance(current.Position, neighborPos);

                // 如果当前节点的父节点与邻居节点之间有直线可视路径，则可以直接连接父节点
                if (current.Parent != null && IsLineOfSight(map, current.Parent.Position, neighborPos))
                {
                    // 使用父节点的 G 代价 + 直线距离代价
                    tentativeGCost = current.Parent.GCost + GetDistance(current.Parent.Position, neighborPos);
                    neighbor.Parent = current.Parent;
                }
                else
                {
                    // 否则将当前节点作为父节点
                    neighbor.Parent = current;
                }

                // 如果开放列表中已有该节点，且新的路径代价不更优，则跳过
                var existingNeighbor = openList.Find(n => n.Position == neighborPos);
                if (existingNeighbor != null && tentativeGCost >= existingNeighbor.GCost)
                    continue;

                // 设置代价
                neighbor.GCost = tentativeGCost;
                neighbor.HCost = Heuristic(neighborPos, targetPos);

                // 如果开放列表中不存在该邻居，则加入
                if (existingNeighbor == null)
                    openList.Add(neighbor);
            }
        }

        // 构造路径（从目标点回溯到起点）
        return endNode != null ? BuildPath(endNode) : new List<ThetaStarNode>();
    }

    /// <summary>
    /// 判断两点之间是否存在不被障碍阻挡的直线路径（Bresenham直线算法）
    /// </summary>
    private static bool IsLineOfSight(int[,] map, Vector2Int start, Vector2Int end)
    {
        int x0 = start.x, y0 = start.y;
        int x1 = end.x, y1 = end.y;
        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            // 如果路径中某个格子是障碍物，视线被阻挡
            if (map[x0, y0] != 0) return false;
            if (x0 == x1 && y0 == y1) break;

            int e2 = 2 * err;
            if (e2 > -dy) { err -= dy; x0 += sx; }
            if (e2 < dx) { err += dx; y0 += sy; }
        }
        return true;
    }

    /// <summary>
    /// 从开放列表中选取 F 代价最小的节点
    /// </summary>
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

    /// <summary>
    /// 启发函数：曼哈顿距离（适用于网格地图）
    /// </summary>
    private static int Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    /// <summary>
    /// 计算两个点之间的距离（用于 G 代价）
    /// </summary>
    private static int GetDistance(Vector2Int a, Vector2Int b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        return dx + dy;
    }

    /// <summary>
    /// 回溯构建路径，从终点往起点走再反转
    /// </summary>
    private static List<ThetaStarNode> BuildPath(ThetaStarNode endNode)
    {
        List<ThetaStarNode> path = new List<ThetaStarNode>();
        ThetaStarNode current = endNode;

        while (current != null)
        {
            path.Add(current);
            current = current.Parent;
        }

        path.Reverse(); // 路径从起点到终点
        return path;
    }
}
