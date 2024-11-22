using System.Collections.Generic;
using UnityEngine;

public class ThetaStarNode
{
    public Vector2Int Position;        // �ڵ��ڵ�ͼ�е�����
    public ThetaStarNode Parent;       // ���ڵ㣬����·������
    public int GCost;                  // ��㵽��ǰ�ڵ��·������
    public int HCost;                  // ��ǰ�ڵ㵽Ŀ��ڵ�Ĺ������
    public int FCost => GCost + HCost; // �ܴ��� (GCost + HCost)

    public ThetaStarNode(Vector2Int position, ThetaStarNode parent, int gCost, int hCost)
    {
        Position = position;
        Parent = parent;
        GCost = gCost;
        HCost = hCost;
    }

    // �ж������ڵ��Ƿ���ȣ����� HashSet �ȼ��ϲ�����
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
        new Vector2Int(0, 1),    // ��
        new Vector2Int(1, 0),    // ��
        new Vector2Int(0, -1),   // ��
        new Vector2Int(-1, 0),   // ��
        new Vector2Int(1, 1),    // ����
        new Vector2Int(1, -1),   // ����
        new Vector2Int(-1, -1),  // ����
        new Vector2Int(-1, 1)    // ����
    };

    public static List<ThetaStarNode> FindPath(int[,] map, Vector2Int startPos, Vector2Int targetPos)
    {
        int mapWidth = map.GetLength(0);
        int mapHeight = map.GetLength(1);

        // �����б�͹ر��б�
        List<ThetaStarNode> openList = new List<ThetaStarNode>();
        HashSet<Vector2Int> closedList = new HashSet<Vector2Int>();

        // �����յ�ڵ�
        ThetaStarNode startNode = new ThetaStarNode(startPos, null, 0, Heuristic(startPos, targetPos));
        ThetaStarNode endNode = null;

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            // �ҵ������б��� `FCost` ��С�Ľڵ�
            ThetaStarNode current = GetNodeWithLowestCost(openList);

            // ����Ѿ�����Ŀ��
            if (current.Position == targetPos)
            {
                endNode = current;
                break;
            }

            openList.Remove(current);
            closedList.Add(current.Position);

            // �������ڽڵ�
            foreach (var direction in Directions)
            {
                Vector2Int neighborPos = current.Position + direction;

                // ����ھ��Ƿ��ڵ�ͼ��Χ�ں��Ƿ����ϰ�
                if (neighborPos.x < 0 || neighborPos.x >= mapWidth ||
                    neighborPos.y < 0 || neighborPos.y >= mapHeight ||
                    map[neighborPos.x, neighborPos.y] != 0 ||  // ֻ�� `0` ��ʾ������
                    closedList.Contains(neighborPos))          // �Ƿ��ڹر��б���
                {
                    continue;
                }

                ThetaStarNode neighbor = new ThetaStarNode(neighborPos, null, 0, 0);

                // �����ھӵ��� GCost
                int tentativeGCost = current.GCost + GetDistance(current.Position, neighborPos);

                // ���ֱ�߿ɼ���
                if (current.Parent != null && IsLineOfSight(map, current.Parent.Position, neighborPos))
                {
                    tentativeGCost = current.Parent.GCost + GetDistance(current.Parent.Position, neighborPos);
                    neighbor.Parent = current.Parent;
                }
                else
                {
                    neighbor.Parent = current;
                }

                // ����ھ��Ѿ��ڿ����б�������·���������
                var existingNeighbor = openList.Find(n => n.Position == neighborPos);
                if (existingNeighbor != null && tentativeGCost >= existingNeighbor.GCost)
                    continue;

                // �����ھӵĴ���
                neighbor.GCost = tentativeGCost;
                neighbor.HCost = Heuristic(neighborPos, targetPos);

                if (existingNeighbor == null)
                    openList.Add(neighbor);
            }
        }

        // ����ҵ���·�������ݲ�����·��
        return endNode != null ? BuildPath(endNode) : new List<ThetaStarNode>();
    }

    private static bool IsLineOfSight(int[,] map, Vector2Int start, Vector2Int end)
    {
        // ʹ�� Bresenham's Line Algorithm ���ֱ���Ƿ�ɼ�
        int x0 = start.x, y0 = start.y;
        int x1 = end.x, y1 = end.y;

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);

        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;

        int err = dx - dy;

        while (true)
        {
            if (map[x0, y0] != 0) // ���·�������ϰ�
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
        // ʹ�������پ�����Ϊ����ʽ
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
