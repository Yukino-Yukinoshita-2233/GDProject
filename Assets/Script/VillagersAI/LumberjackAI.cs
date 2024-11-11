using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LumberjackAI : MonoBehaviour
{
    public int[,] map;                  // 地图二维数组：0 表示可行走，1 表示障碍
    public Vector2Int start;            // 伐木工起始位置
    public Vector2Int baseLocation;     // 基地位置
    public float moveSpeed = 2f;        // 移动速度

    private List<Node> path;            // 当前路径
    private Vector2Int targetResource;  // 当前目标资源位置

    void Start()
    {
        // 通过行为树执行伐木工的行为
        StartCoroutine(BehaviorTree());
    }

    IEnumerator BehaviorTree()
    {
        while (true)
        {
            yield return IdleOrSearchForWood();  // 寻找木材资源
            yield return MoveToTarget(targetResource);  // 前往资源位置
            yield return CollectWood();          // 采集资源
            yield return MoveToTarget(baseLocation); // 返回基地
            yield return DeliverWood();          // 交付资源
        }
    }

    // 查找最近的木材资源位置
    IEnumerator IdleOrSearchForWood()
    {
        targetResource = FindNearestWood();
        yield break;
    }

    // 移动到目标位置
    IEnumerator MoveToTarget(Vector2Int targetPosition)
    {
        path = AStarPathfinding.FindPath(map, start, targetPosition); // 使用A*算法计算路径

        foreach (var node in path)
        {
            Vector3 worldPos = new Vector3(node.Position.x, 0, node.Position.y);
            while (Vector3.Distance(transform.position, worldPos) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, worldPos, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }
        start = targetPosition; // 更新伐木工当前位置
    }

    // 采集木材
    IEnumerator CollectWood()
    {
        Debug.Log("采集完成");
        yield return new WaitForSeconds(2f); // 模拟采集时间
    }

    // 交付木材
    IEnumerator DeliverWood()
    {
        Debug.Log("交付完成");
        yield break;
    }

    // 查找最近的木材资源位置
    Vector2Int FindNearestWood()
    {
        // 假设所有木材资源的坐标保存在 woodPositions 列表中
        List<Vector2Int> woodPositions = new List<Vector2Int>(); // 例子列表，需要在场景中初始化
        Vector2Int nearest = woodPositions[0];
        int closestDistance = int.MaxValue;

        foreach (var woodPos in woodPositions)
        {
            int distance = AStarPathfinding.GetDistance(start, woodPos);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearest = woodPos;
            }
        }
        return nearest;
    }
}
