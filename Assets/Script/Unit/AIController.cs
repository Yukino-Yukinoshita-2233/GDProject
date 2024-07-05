using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public Transform target; // 目标位置
    private MapGenerator mapGenerator;
    private AStarPathfinding pathfinding;

    void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();
        pathfinding = new AStarPathfinding();

    }

    //void Update()
    //{
    //    // 假设AI的起始位置为网格的左下角
    //    GridNode startNode = mapGenerator.grid[0, 0];
    //    // 获取目标节点
    //    GridNode targetNode = mapGenerator.grid[(int)target.position.x, (int)target.position.z];

    //    // 计算路径
    //    List<GridNode> path = pathfinding.FindPath(startNode, targetNode, mapGenerator.grid);

    //    if (path != null)
    //    {
    //        // 遍历路径，并将AI移动到下一个节点
    //        foreach (GridNode node in path)
    //        {
    //            // 这里可以实现AI移动逻辑，例如：
    //            transform.position = Vector3.MoveTowards(transform.position, node.WorldPosition, Time.deltaTime * 5);
    //        }
    //    }
    //}

    // 新增一个方法来接收网格作为参数
    public void PlanPath(float[,] grid)
    {
        if (grid == null)
        {
            Debug.LogError("Grid not initialized!");
            return;
        }

        // 在这里进行路径规划和移动逻辑
    }
}
