using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Theta* �ڵ��࣬���ڱ�ʾ��ͼ�е�һ�������Լ�·����������еĸ�����Ϣ
/// </summary>
public class ThetaStarNode
{
    public Vector2Int Position;        // ��ǰ�ڵ��ڵ�ͼ�ϵ�����
    public ThetaStarNode Parent;       // ��ǰ�ڵ�ĸ��ڵ㣨����·�����ݣ�
    public int GCost;                  // ����㵽��ǰ�ڵ��ʵ�ʴ���
    public int HCost;                  // �ӵ�ǰ�ڵ㵽Ŀ��ڵ�Ĺ�����ۣ�����������
    public int FCost => GCost + HCost; // �ܴ��� F = G + H���������ȼ��ж�

    public ThetaStarNode(Vector2Int position, ThetaStarNode parent, int gCost, int hCost)
    {
        Position = position;
        Parent = parent;
        GCost = gCost;
        HCost = hCost;
    }
}

/// <summary>
/// Theta* ·���滮�㷨�ࣨ��̬��
/// </summary>
public static class ThetaStar
{
    // �˷����ƶ����������� + �ĸ��ԽǷ���
    private static readonly Vector2Int[] Directions = new Vector2Int[]
    {
        new Vector2Int(0, 1),    // ��
        new Vector2Int(1, 0),    // ��
        new Vector2Int(0, -1),   // ��
        new Vector2Int(-1, 0),   // ��
        new Vector2Int(1, 1),    // ����
        new Vector2Int(1, -1),   // ����
        new Vector2Int(-1, -1),  // ����
        new Vector2Int(-1, 1)    // ����
    };

    /// <summary>
    /// Ѱ�Ҵ���㵽Ŀ����һ��·��
    /// </summary>
    /// <param name="map">��ͼ���飨0 Ϊ��ͨ�У�����Ϊ�ϰ���</param>
    /// <param name="startPos">�������</param>
    /// <param name="targetPos">Ŀ�������</param>
    /// <returns>·���ڵ��б�����Ҳ���·���򷵻ؿ��б�</returns>
    public static List<ThetaStarNode> FindPath(int[,] map, Vector2Int startPos, Vector2Int targetPos)
    {
        int mapWidth = map.GetLength(0);
        int mapHeight = map.GetLength(1);

        // �����б������Ľڵ㣩
        List<ThetaStarNode> openList = new List<ThetaStarNode>();
        // �ر��б��Ѿ������Ľڵ㣩
        HashSet<Vector2Int> closedList = new HashSet<Vector2Int>();

        // �������ڵ㲢���뿪���б�
        ThetaStarNode startNode = new ThetaStarNode(startPos, null, 0, Heuristic(startPos, targetPos));
        openList.Add(startNode);

        ThetaStarNode endNode = null; // ����·���յ�

        while (openList.Count > 0)
        {
            // �ӿ����б���ѡȡ F ������С�Ľڵ�
            ThetaStarNode current = GetNodeWithLowestCost(openList);

            // �����ǰ�ڵ����Ŀ��㣬��ʾ·���ҵ�
            if (current.Position == targetPos)
            {
                endNode = current;
                break;
            }

            // ����ǰ�ڵ�ӿ����б����Ƴ�������ر��б�
            openList.Remove(current);
            closedList.Add(current.Position);

            // ������ǰ�ڵ���������ڽڵ�
            foreach (var direction in Directions)
            {
                Vector2Int neighborPos = current.Position + direction;

                // Խ���� + �ϰ���� + �Ѵ���ڵ�����
                if (neighborPos.x < 0 || neighborPos.x >= mapWidth ||
                    neighborPos.y < 0 || neighborPos.y >= mapHeight ||
                    map[neighborPos.x, neighborPos.y] != 0 ||
                    closedList.Contains(neighborPos))
                {
                    continue;
                }

                // �����ھӽڵ�
                ThetaStarNode neighbor = new ThetaStarNode(neighborPos, null, 0, 0);
                // Ĭ�ϴӵ�ǰ�ڵ��߹�ȥ�� G ����
                int tentativeGCost = current.GCost + GetDistance(current.Position, neighborPos);

                // �����ǰ�ڵ�ĸ��ڵ����ھӽڵ�֮����ֱ�߿���·���������ֱ�����Ӹ��ڵ�
                if (current.Parent != null && IsLineOfSight(map, current.Parent.Position, neighborPos))
                {
                    // ʹ�ø��ڵ�� G ���� + ֱ�߾������
                    tentativeGCost = current.Parent.GCost + GetDistance(current.Parent.Position, neighborPos);
                    neighbor.Parent = current.Parent;
                }
                else
                {
                    // ���򽫵�ǰ�ڵ���Ϊ���ڵ�
                    neighbor.Parent = current;
                }

                // ��������б������иýڵ㣬���µ�·�����۲����ţ�������
                var existingNeighbor = openList.Find(n => n.Position == neighborPos);
                if (existingNeighbor != null && tentativeGCost >= existingNeighbor.GCost)
                    continue;

                // ���ô���
                neighbor.GCost = tentativeGCost;
                neighbor.HCost = Heuristic(neighborPos, targetPos);

                // ��������б��в����ڸ��ھӣ������
                if (existingNeighbor == null)
                    openList.Add(neighbor);
            }
        }

        // ����·������Ŀ�����ݵ���㣩
        return endNode != null ? BuildPath(endNode) : new List<ThetaStarNode>();
    }

    /// <summary>
    /// �ж�����֮���Ƿ���ڲ����ϰ��赲��ֱ��·����Bresenhamֱ���㷨��
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
            // ���·����ĳ���������ϰ�����߱��赲
            if (map[x0, y0] != 0) return false;
            if (x0 == x1 && y0 == y1) break;

            int e2 = 2 * err;
            if (e2 > -dy) { err -= dy; x0 += sx; }
            if (e2 < dx) { err += dx; y0 += sy; }
        }
        return true;
    }

    /// <summary>
    /// �ӿ����б���ѡȡ F ������С�Ľڵ�
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
    /// ���������������پ��루�����������ͼ��
    /// </summary>
    private static int Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    /// <summary>
    /// ����������֮��ľ��루���� G ���ۣ�
    /// </summary>
    private static int GetDistance(Vector2Int a, Vector2Int b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        return dx + dy;
    }

    /// <summary>
    /// ���ݹ���·�������յ���������ٷ�ת
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

        path.Reverse(); // ·������㵽�յ�
        return path;
    }
}
