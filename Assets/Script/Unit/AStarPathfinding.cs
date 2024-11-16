using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding
{
    public static List<Node> FindPath(int[,] map, Vector2Int startPos, Vector2Int targetPos)
    {
        List<Node> openList = new List<Node>(); // �����б�
        HashSet<Node> closedList = new HashSet<Node>(); // �պ��б�
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

            // �ҵ�Ŀ��λ�ú�ʼ��ӡ·��
            if (currentNode.Position == targetPos)
            {
                List<Node> path = RetracePath(startNode, currentNode);

                // ���·���еĽڵ���Ϣ
                Debug.Log("Path found:");
                foreach (Node node in path)
                {
                    Debug.Log(node); // �������ڵ���Ϣ
                }

                return path;
            }

            foreach (Vector2Int direction in GetNeighbourPositions(currentNode.Position, map))
            {
                if (map[direction.x, direction.y] == 1 || closedList.Contains(new Node(direction))) // �����ϰ����Ѵ���Ľڵ�
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
        return null; // �Ҳ���·��ʱ���ؿ�
    }

    // ����·��
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

    // ��ȡ���ڽڵ�λ��
    private static List<Vector2Int> GetNeighbourPositions(Vector2Int nodePos, int[,] map)
    {
        List<Vector2Int> neighbours = new List<Vector2Int>();

        if (nodePos.x + 1 < map.GetLength(0)) neighbours.Add(new Vector2Int(nodePos.x + 1, nodePos.y));
        if (nodePos.x - 1 >= 0) neighbours.Add(new Vector2Int(nodePos.x - 1, nodePos.y));
        if (nodePos.y + 1 < map.GetLength(1)) neighbours.Add(new Vector2Int(nodePos.x, nodePos.y + 1));
        if (nodePos.y - 1 >= 0) neighbours.Add(new Vector2Int(nodePos.x, nodePos.y - 1));

        return neighbours;
    }

    // ������루�����پ��룩
    public static int GetDistance(Vector2Int posA, Vector2Int posB)
    {
        return Mathf.Abs(posA.x - posB.x) + Mathf.Abs(posA.y - posB.y);
    }
}

public class Node
{
    public Vector2Int Position;   // �ڵ������
    public Node Parent;           // ���ڵ�
    public int GCost;             // ����㵽�ýڵ�Ĵ���
    public int HCost;             // �Ӹýڵ㵽�յ�Ĺ������
    public int FCost => GCost + HCost; // �ܴ��ۣ�G + H��

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
