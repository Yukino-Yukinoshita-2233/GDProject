//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
//using UnityEngine;
//using MapManagernamespace;

//public class JPS : MonoBehaviour
//{
//    public Transform target; // 目标预制体
//    public float speed = 5f; // NPC移动速度
//    private Vector3[] path;  // 路径点
//    private int targetIndex = 0;
//    // 地图尺寸
//    int widthGen;
//    int heightGen;
//    int scaleGen;
//    int[,] gridMapGen = null;   //网格地图

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
//            Debug.Log("地图数据被修改,更新地图实例");
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

//            // 如果到达当前目标点，移动到下一个点
//            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
//            {
//                targetIndex++;
//            }
//        }
//    }

//    public void OnMapGenerated()
//    {
//        //Debug.Log("OnMapGenerated:地图生成完成，继续执行后续操作");
//        // 获取地图数据
//        //Print2DArray(MapManager.gridMap);//debug地图数据
//        gridMapGen = MapManager.gridMap;
//        //Debug.Log("获取地图数据成功");
//        // 实例化地图
//        InstantiateMap();
//        Debug.Log("实例化地图成功");
//    }

//    //实例化地图
//    void InstantiateMap()
//    {
//        DeleteChildren();//删除当前地图

//        // 首先检查是否有名为 "BaseGridMap" 的子物体
//        CaiLiaoParent = transform.Find("BaseGridMap");

//        // 如果没有，创建一个新的 GameObject 作为子物体
//        if (CaiLiaoParent == null)
//        {
//            CaiLiaoParent = new GameObject("BaseGridMap").transform;
//            CaiLiaoParent.SetParent(transform);  // 设置为当前物体的子物体
//            CaiLiaoParent.localPosition = Vector3.zero;  // 设置相对位置为零
//            Debug.Log("InstantiateMap:创建baseGridMap子物体成功");
//        }

//        Vector3 WorldPosition;
//        float H = 0.5f;
//        // 遍历节点网格并实例化所有对象
//        for (int x = 0; x < widthGen; x++)
//        {
//            for (int y = 0; y < heightGen; y++)
//            {

//                if (gridMapGen[x, y] == 3) //生成水
//                {
//                    WorldPosition = new Vector3(x, 0 - H, y);
//                    GameObject instance = Instantiate(waterPrefab, WorldPosition, Quaternion.identity, CaiLiaoParent);
//                }
//                else if (gridMapGen[x, y] == 0) //生成草
//                {
//                    WorldPosition = new Vector3(x, 0, y);
//                    GameObject instance = Instantiate(grassPrefab, WorldPosition, Quaternion.identity, CaiLiaoParent);
//                    //instance.layer = GrassLayerMask;

//                }
//                else if (gridMapGen[x, y] == 1)    //生成山
//                {
//                    WorldPosition = new Vector3(x, 0 + 2 * H, y);
//                    GameObject instance = Instantiate(mountainPrefab, WorldPosition, Quaternion.identity, CaiLiaoParent);

//                }
//            }
//        }
//    }
//    // 开始JPS路径搜索
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

//            // 找到路径终点
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

//            // 执行 JPS 搜索
//            Jump(currentNode, targetNode, openList, closedList);
//        }
//    }

//    // 执行 JPS 跳跃搜索
//    private void Jump(Node currentNode, Node targetNode, List<Node> openList, HashSet<Node> closedList)
//    {
//        // 4个方向: 上, 下, 左, 右
//        Vector2Int[] directions = new Vector2Int[]
//        {
//            new Vector2Int(0, 1), // 上
//            new Vector2Int(0, -1), // 下
//            new Vector2Int(-1, 0), // 左
//            new Vector2Int(1, 0) // 右
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

//    // 跳跃到目标节点
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

//        // 执行跳跃，直到找到关键点或者停止
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

        public Transform targetPrefab; // 自定义的目标预制体（NPC的目标）

        private Vector2Int start; // NPC的起始位置
        private Vector2Int goal; // 目标位置（目标点）

        private void Start()
        {
            mapManager = MapManager.Instance;
            gridMap = mapManager.GetMap();

            // 示例起始位置和目标位置（根据需要设置）
            start = new Vector2Int(10, 10); // 替换为实际的起点位置
            goal = new Vector2Int(40, 30); // 替换为实际的目标位置

            List<Vector2Int> path = FindPath(start, goal);
            foreach (Vector2Int node in path)
            {
                Debug.Log("路径节点: " + node);
            }
        }

        // 使用JPS算法查找路径
        public List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
        {
            List<Vector2Int> path = new List<Vector2Int>();

            // 如果起点或目标无效或被阻挡，返回空路径
            if (!IsWalkable(start) || !IsWalkable(goal) || !IsInBounds(start) || !IsInBounds(goal))
            {
                Debug.Log("无效的起点或目标位置。");
                return path;
            }

            List<Vector2Int> openList = new List<Vector2Int>();
            HashSet<Vector2Int> closedList = new HashSet<Vector2Int>();

            openList.Add(start);

            while (openList.Count > 0)
            {
                // 按照优先级排序（此处简单处理）
                openList.Sort((a, b) => (a - goal).sqrMagnitude.CompareTo((b - goal).sqrMagnitude));
                Vector2Int current = openList[0];
                openList.RemoveAt(0);

                if (current == goal)
                {
                    // 找到目标，重建路径
                    while (current != start)
                    {
                        path.Add(current);
                        current = GetParent(current);
                    }
                    path.Reverse();
                    return path;
                }

                closedList.Add(current);

                // 使用JPS生成邻居节点
                List<Vector2Int> neighbors = GetJumpNeighbors(current);

                foreach (Vector2Int neighbor in neighbors)
                {
                    if (closedList.Contains(neighbor) || !IsWalkable(neighbor))
                        continue;

                    openList.Add(neighbor);
                }
            }

            return path; // 如果没有找到路径，返回空路径
        }

        // 检查位置是否可行走（草地）
        private bool IsWalkable(Vector2Int position)
        {
            return gridMap[position.x, position.y] == 0; // 0代表草地
        }

        // 检查位置是否在地图范围内
        private bool IsInBounds(Vector2Int position)
        {
            return position.x >= 0 && position.y >= 0 && position.x < gridMap.GetLength(0) && position.y < gridMap.GetLength(1);
        }

        // 获取节点的父节点（用于路径重建）
        private Vector2Int GetParent(Vector2Int node)
        {
            // 对于JPS，假设父节点已经被记录或计算出来。实际中，通常会有父节点的映射表来跟踪父节点。
            return node; // 占位符
        }

        // 使用JPS生成跳跃的邻居节点（优化了普通邻居的遍历）
        private List<Vector2Int> GetJumpNeighbors(Vector2Int node)
        {
            List<Vector2Int> neighbors = new List<Vector2Int>();

            // 8个方向：上、下、左、右以及四个对角方向
            Vector2Int[] directions = {
                new Vector2Int(0, 1), // 上
                new Vector2Int(0, -1), // 下
                new Vector2Int(1, 0), // 右
                new Vector2Int(-1, 0), // 左
                new Vector2Int(1, 1), // 右上对角线
                new Vector2Int(-1, 1), // 左上对角线
                new Vector2Int(1, -1), // 右下对角线
                new Vector2Int(-1, -1) // 左下对角线
            };

            foreach (var dir in directions)
            {
                Vector2Int next = node + dir;

                if (IsInBounds(next) && IsWalkable(next))
                {
                    // JPS会跳过连续的可行走区域节点，直接跳到障碍物或另一个跳跃点
                    neighbors.Add(next);
                }
            }

            return neighbors;
        }

        // 可选：在Unity中可视化路径（自定义预制体）
        public void VisualizePath(List<Vector2Int> path)
        {
            foreach (Vector2Int point in path)
            {
                // 在路径的每个节点位置生成一个自定义目标预制体
                Vector3 worldPos = new Vector3(point.x, 0, point.y);
                Instantiate(targetPrefab, worldPos, Quaternion.identity);
            }
        }
    }
}
