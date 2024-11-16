using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector2Int Position;    // �ڵ��ڵ�ͼ�е�����
    public Node Parent;            // �ýڵ�ĸ��ڵ㣬����·������
    public int GCost;              // ��㵽��ǰ�ڵ��·������
    public int HCost;              // ��ǰ�ڵ㵽Ŀ��ڵ�Ĺ��ƴ���
    public int FCost => GCost + HCost;  // �ܴ��� (GCost + HCost)

    // ���캯��
    public Node(Vector2Int position, Node parent, int gCost, int hCost)
    {
        Position = position;
        Parent = parent;
        GCost = gCost;
        HCost = hCost;
    }

    // ���ڱȽϽڵ��Ƿ���ȵ����ط������ж������Ƿ���ͬ
    public override bool Equals(object obj)
    {
        if (obj is Node node)
            return Position == node.Position;
        return false;
    }

    // ���� HashSet �洢�Ĺ�ϣ����
    public override int GetHashCode()
    {
        return Position.GetHashCode();
    }
}

public class AStarPathfinding
{
    // A* Ѱ·�㷨��������㵽�յ��·��
    public static List<Node> FindPath(int[,] map, Vector2Int startPos, Vector2Int targetPos)
    {
        List<Node> openList = new List<Node>(); // �����б��洢������ڵ�
        HashSet<Node> closedList = new HashSet<Node>(); // �պ��б��洢�Ѵ���ڵ�

        // ������ʼ�ڵ㣬��������뿪���б�
        Node startNode = new Node(startPos, null, 0, GetDistance(startPos, targetPos));
        openList.Add(startNode);

        // ��ʼ�������б��еĽڵ�
        while (openList.Count > 0)
        {
            // ѡ�������С FCost �Ľڵ���Ϊ��ǰ�ڵ�
            Node currentNode = openList[0];
            foreach (Node node in openList)
            {
                if (node.FCost < currentNode.FCost || (node.FCost == currentNode.FCost && node.HCost < currentNode.HCost))
                    currentNode = node;
            }

            // �ӿ����б����Ƴ���ǰ�ڵ㣬���������պ��б�
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            // �����ǰ�ڵ�ΪĿ��ڵ㣬����������·��
            if (currentNode.Position == targetPos)
                return RetracePath(startNode, currentNode);

            // ������ǰ�ڵ���������ڽڵ�
            foreach (Vector2Int neighborPos in GetNeighborPositions(currentNode.Position, map))
            {
                // �����ϰ�����Ѿ�������Ľڵ�
                if (map[neighborPos.x, neighborPos.y] != 0 || closedList.Contains(new Node(neighborPos, null, 0, 0))) // ���ȱ�ٵĲ���
                    continue;

                // �����ھӽڵ�� GCost������㵽���ھӵĴ��ۣ�
                int newMovementCostToNeighbor = currentNode.GCost + GetDistance(currentNode.Position, neighborPos);
                Node neighborNode = new Node(neighborPos, currentNode, newMovementCostToNeighbor, GetDistance(neighborPos, targetPos));

                // ������ھӽڵ㲻�ڿ����б��У����µ�·�����۸�С�����¸��ھӽڵ�
                if (newMovementCostToNeighbor < neighborNode.GCost || !openList.Contains(neighborNode))
                {
                    neighborNode.GCost = newMovementCostToNeighbor;
                    neighborNode.Parent = currentNode;

                    // ����ھӽڵ㲻�ڿ����б��У��������
                    if (!openList.Contains(neighborNode))
                        openList.Add(neighborNode);
                }
            }
        }

        // ��������б�Ϊ����δ�ҵ�·�������� null ��ʾ��·��
        return null;
    }

    // ����·������Ŀ��ڵ��� Parent ָ����ݵ���ʼ�ڵ�
    private static List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        // ���ݵ���㣬��������·��
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        path.Reverse(); // ·����ת��ʹ·������㵽�յ�
        return path;
    }

    // ��ȡ��ǰ�ڵ��ĸ������������ң����ڵĽڵ�����
    private static List<Vector2Int> GetNeighborPositions(Vector2Int position, int[,] map)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        // ������ڸ����Ƿ��ڵ�ͼ��Χ�ڣ�����ӵ��ھ��б�
        if (position.x + 1 < map.GetLength(0)) neighbors.Add(new Vector2Int(position.x + 1, position.y));
        if (position.x - 1 >= 0) neighbors.Add(new Vector2Int(position.x - 1, position.y));
        if (position.y + 1 < map.GetLength(1)) neighbors.Add(new Vector2Int(position.x, position.y + 1));
        if (position.y - 1 >= 0) neighbors.Add(new Vector2Int(position.x, position.y - 1));

        return neighbors;
    }

    // ���������ڵ�֮��������پ��룬���ڹ��� HCost
    private static int GetDistance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y); // �����پ���
    }
}
