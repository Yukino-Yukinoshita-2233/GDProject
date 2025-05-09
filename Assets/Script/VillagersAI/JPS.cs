//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
//using UnityEngine;
//using MapManagernamespace;

//public class JPS : MonoBehaviour
//{
//    public Transform target; // Ŀ��Ԥ����
//    public float speed = 5f; // NPC�ƶ��ٶ�
//    private Vector3[] path;  // ·����
//    private int targetIndex = 0;
//    // ��ͼ�ߴ�
//    int widthGen;
//    int heightGen;
//    int scaleGen;
//    int[,] gridMapGen = null;   //�����ͼ

//    private void Start()
//    {
//        widthGen = MapManager.width;
//        heightGen = MapManager.height;
//        scaleGen = MapManager.scale;
//        OnMapGenerated();
//    }

//    private void Start()
//    {
//        widthGen = MapManager.width;
//        heightGen = MapManager.height;
//        scaleGen = MapManager.scale;
//        OnMapGenerated();
//    }

//    private void Update()
//    {
//        if (MapManager.isMapChange)
//        {
//            Debug.Log("��ͼ���ݱ��޸�,���µ�ͼʵ��");
//            widthGen = MapManager.width;
//            heightGen = MapManager.height;
//            scaleGen = MapManager.scale;
//            gridMapGen = MapManager.gridMap;
//            OnMapGenerated();
//            MapManager.isMapChange = false;
//        }

//        if (path != null && targetIndex < path.Length)
//        {
//            Vector3 targetPosition = path[targetIndex];
//            Vector3 moveDirection = (targetPosition - transform.position).normalized;
//            transform.position += moveDirection * speed * Time.deltaTime;

//            // ������ﵱǰĿ��㣬�ƶ�����һ����
//            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
//            {
//                targetIndex++;
//            }
//        }
//    }

//    public void OnMapGenerated()
//    {
//        //Debug.Log("OnMapGenerated:��ͼ������ɣ�����ִ�к�������");
//        // ��ȡ��ͼ����
//        //Print2DArray(MapManager.gridMap);//debug��ͼ����
//        gridMapGen = MapManager.gridMap;
//        //Debug.Log("��ȡ��ͼ���ݳɹ�");
//        // ʵ������ͼ
//        InstantiateMap();
//        Debug.Log("ʵ������ͼ�ɹ�");
//    }

//    //ʵ������ͼ
//    void InstantiateMap()
//    {
//        DeleteChildren();//ɾ����ǰ��ͼ

//        // ���ȼ���Ƿ�����Ϊ "BaseGridMap" ��������
//        CaiLiaoParent = transform.Find("BaseGridMap");

//        // ���û�У�����һ���µ� GameObject ��Ϊ������
//        if (CaiLiaoParent == null)
//        {
//            CaiLiaoParent = new GameObject("BaseGridMap").transform;
//            CaiLiaoParent.SetParent(transform);  // ����Ϊ��ǰ�����������
//            CaiLiaoParent.localPosition = Vector3.zero;  // �������λ��Ϊ��
//            Debug.Log("InstantiateMap:����baseGridMap������ɹ�");
//        }

//        Vector3 WorldPosition;
//        float H = 0.5f;
//        // �����ڵ�����ʵ�������ж���
//        for (int x = 0; x < widthGen; x++)
//        {
//            for (int y = 0; y < heightGen; y++)
//            {

//                if (gridMapGen[x, y] == 3) //����ˮ
//                {
//                    WorldPosition = new Vector3(x, 0 - H, y);
//                    GameObject instance = Instantiate(waterPrefab, WorldPosition, Quaternion.identity, CaiLiaoParent);
//                }
//                else if (gridMapGen[x, y] == 0) //���ɲ�
//                {
//                    WorldPosition = new Vector3(x, 0, y);
//                    GameObject instance = Instantiate(grassPrefab, WorldPosition, Quaternion.identity, CaiLiaoParent);
//                    //instance.layer = GrassLayerMask;

//                }
//                else if (gridMapGen[x, y] == 1)    //����ɽ
//                {
//                    WorldPosition = new Vector3(x, 0 + 2 * H, y);
//                    GameObject instance = Instantiate(mountainPrefab, WorldPosition, Quaternion.identity, CaiLiaoParent);

//                }
//            }
//        }
//    }
//    // ��ʼJPS·������
//    private IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
//    {
//        List<Vector3> newPath = new List<Vector3>();
//        Node startNode = grid.NodeFromWorldPoint(startPos);
//        Node targetNode = grid.NodeFromWorldPoint(targetPos);

//        List<Node> openList = new List<Node> { startNode };
//        HashSet<Node> closedList = new HashSet<Node>();

//        while (openList.Count > 0)
//        {
//            Node currentNode = openList[0];

//            // �ҵ�·���յ�
//            if (currentNode == targetNode)
//            {
//                Node temp = currentNode;
//                while (temp != startNode)
//                {
//                    newPath.Add(temp.worldPosition);
//                    temp = temp.parent;
//                }
//                newPath.Reverse();
//                path = newPath.ToArray();
//                yield break;
//            }

//            openList.Remove(currentNode);
//            closedList.Add(currentNode);

//            // ִ�� JPS ����
//            Jump(currentNode, targetNode, openList, closedList);
//        }
//    }

//    // ִ�� JPS ��Ծ����
//    private void Jump(Node currentNode, Node targetNode, List<Node> openList, HashSet<Node> closedList)
//    {
//        // 4������: ��, ��, ��, ��
//        Vector2Int[] directions = new Vector2Int[]
//        {
//            new Vector2Int(0, 1), // ��
//            new Vector2Int(0, -1), // ��
//            new Vector2Int(-1, 0), // ��
//            new Vector2Int(1, 0) // ��
//        };

//        foreach (Vector2Int dir in directions)
//        {
//            Node jumpNode = JumpToNode(currentNode, dir, targetNode);
//            if (jumpNode != null && !closedList.Contains(jumpNode))
//            {
//                openList.Add(jumpNode);
//                jumpNode.parent = currentNode;
//            }
//        }
//    }

//    // ��Ծ��Ŀ��ڵ�
//    private Node JumpToNode(Node node, Vector2Int direction, Node targetNode)
//    {
//        Vector2Int newPosition = node.gridPosition + direction;

//        if (!grid.IsValidGridPosition(newPosition))
//            return null;

//        Node nextNode = grid.GetNode(newPosition.x, newPosition.y);

//        if (nextNode.isObstacle)
//            return null;

//        if (nextNode == targetNode)
//            return nextNode;

//        // ִ����Ծ��ֱ���ҵ��ؼ������ֹͣ
//        return JumpToNode(nextNode, direction, targetNode);
//    }
//}
using MapManagernamespace;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class JPS : MonoBehaviour
    {
        private MapManager mapManager;
        private int[,] gridMap;

        public Transform targetPrefab; // �Զ����Ŀ��Ԥ���壨NPC��Ŀ�꣩

        private Vector2Int start; // NPC����ʼλ��
        private Vector2Int goal; // Ŀ��λ�ã�Ŀ��㣩

        private void Start()
        {
            mapManager = MapManager.Instance;
            gridMap = mapManager.GetMap();

            // ʾ����ʼλ�ú�Ŀ��λ�ã�������Ҫ���ã�
            start = new Vector2Int(10, 10); // �滻Ϊʵ�ʵ����λ��
            goal = new Vector2Int(40, 30); // �滻Ϊʵ�ʵ�Ŀ��λ��

            List<Vector2Int> path = FindPath(start, goal);
            foreach (Vector2Int node in path)
            {
                Debug.Log("·���ڵ�: " + node);
            }
        }

        // ʹ��JPS�㷨����·��
        public List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
        {
            List<Vector2Int> path = new List<Vector2Int>();

            // �������Ŀ����Ч���赲�����ؿ�·��
            if (!IsWalkable(start) || !IsWalkable(goal) || !IsInBounds(start) || !IsInBounds(goal))
            {
                Debug.Log("��Ч������Ŀ��λ�á�");
                return path;
            }

            List<Vector2Int> openList = new List<Vector2Int>();
            HashSet<Vector2Int> closedList = new HashSet<Vector2Int>();

            openList.Add(start);

            while (openList.Count > 0)
            {
                // �������ȼ����򣨴˴��򵥴���
                openList.Sort((a, b) => (a - goal).sqrMagnitude.CompareTo((b - goal).sqrMagnitude));
                Vector2Int current = openList[0];
                openList.RemoveAt(0);

                if (current == goal)
                {
                    // �ҵ�Ŀ�꣬�ؽ�·��
                    while (current != start)
                    {
                        path.Add(current);
                        current = GetParent(current);
                    }
                    path.Reverse();
                    return path;
                }

                closedList.Add(current);

                // ʹ��JPS�����ھӽڵ�
                List<Vector2Int> neighbors = GetJumpNeighbors(current);

                foreach (Vector2Int neighbor in neighbors)
                {
                    if (closedList.Contains(neighbor) || !IsWalkable(neighbor))
                        continue;

                    openList.Add(neighbor);
                }
            }

            return path; // ���û���ҵ�·�������ؿ�·��
        }

        // ���λ���Ƿ�����ߣ��ݵأ�
        private bool IsWalkable(Vector2Int position)
        {
            return gridMap[position.x, position.y] == 0; // 0����ݵ�
        }

        // ���λ���Ƿ��ڵ�ͼ��Χ��
        private bool IsInBounds(Vector2Int position)
        {
            return position.x >= 0 && position.y >= 0 && position.x < gridMap.GetLength(0) && position.y < gridMap.GetLength(1);
        }

        // ��ȡ�ڵ�ĸ��ڵ㣨����·���ؽ���
        private Vector2Int GetParent(Vector2Int node)
        {
            // ����JPS�����踸�ڵ��Ѿ�����¼����������ʵ���У�ͨ�����и��ڵ��ӳ��������ٸ��ڵ㡣
            return node; // ռλ��
        }

        // ʹ��JPS������Ծ���ھӽڵ㣨�Ż�����ͨ�ھӵı�����
        private List<Vector2Int> GetJumpNeighbors(Vector2Int node)
        {
            List<Vector2Int> neighbors = new List<Vector2Int>();

            // 8�������ϡ��¡������Լ��ĸ��ԽǷ���
            Vector2Int[] directions = {
                new Vector2Int(0, 1), // ��
                new Vector2Int(0, -1), // ��
                new Vector2Int(1, 0), // ��
                new Vector2Int(-1, 0), // ��
                new Vector2Int(1, 1), // ���϶Խ���
                new Vector2Int(-1, 1), // ���϶Խ���
                new Vector2Int(1, -1), // ���¶Խ���
                new Vector2Int(-1, -1) // ���¶Խ���
            };

            foreach (var dir in directions)
            {
                Vector2Int next = node + dir;

                if (IsInBounds(next) && IsWalkable(next))
                {
                    // JPS�����������Ŀ���������ڵ㣬ֱ�������ϰ������һ����Ծ��
                    neighbors.Add(next);
                }
            }

            return neighbors;
        }

        // ��ѡ����Unity�п��ӻ�·�����Զ���Ԥ���壩
        public void VisualizePath(List<Vector2Int> path)
        {
            foreach (Vector2Int point in path)
            {
                // ��·����ÿ���ڵ�λ������һ���Զ���Ŀ��Ԥ����
                Vector3 worldPos = new Vector3(point.x, 0, point.y);
                Instantiate(targetPrefab, worldPos, Quaternion.identity);
            }
        }
    }
}
