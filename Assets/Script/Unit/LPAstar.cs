using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ʾ LPA* �㷨�еĽڵ㡣
/// </summary>
public class LPAStarNode
{
    public Vector2Int Position;    // �ڵ��ڵ�ͼ�е�����
    public LPAStarNode Parent;     // �ýڵ�ĸ��ڵ㣬����·������
    public int GCost;              // ����㵽��ǰ�ڵ��ʵ��·������
    public int RHS;                // ��ǰ�ڵ�ĺ�ѡ·������
    public int HCost;              // ��ǰ�ڵ㵽Ŀ��ڵ�Ĺ�����ۣ�����ʽ��
    public int FCost => Mathf.Min(GCost, RHS) + HCost;  // �ܴ��ۣ���������

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
/// ʵ�� LPA* �㷨��
/// </summary>
public static class LPAStar
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

    public static List<LPAStarNode> FindPath(int[,] map, Vector2Int startPos, Vector2Int targetPos)
    {
        int mapWidth = map.GetLength(0);
        int mapHeight = map.GetLength(1);

        // �����б�͹ر��б�
        List<LPAStarNode> openList = new List<LPAStarNode>();
        HashSet<Vector2Int> closedList = new HashSet<Vector2Int>();

        // �����յ�ڵ�
        LPAStarNode startNode = new LPAStarNode(startPos, gCost: 0, rhs: 0, hCost: Heuristic(startPos, targetPos));
        LPAStarNode endNode = null;

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            // �ҵ������б��� `FCost` ��С�Ľڵ�
            LPAStarNode current = GetNodeWithLowestCost(openList);

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

                // ����ھ��Ƿ��ڵ�ͼ��Χ��
                if (neighborPos.x < 0 || neighborPos.x >= mapWidth ||
                    neighborPos.y < 0 || neighborPos.y >= mapHeight ||
                    map[neighborPos.x, neighborPos.y] != 0 ||  // �� `0` ��Ϊ�ϰ�
                    closedList.Contains(neighborPos))         // �Ƿ��ڹر��б���
                {
                    continue;
                }

                LPAStarNode neighbor = new LPAStarNode(neighborPos);

                // �����ھӵ��� GCost
                int tentativeGCost = current.GCost + GetDistance(current.Position, neighborPos);

                // �������·�����������ڵ�
                neighbor.Parent = current;

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
        // �����پ���
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
