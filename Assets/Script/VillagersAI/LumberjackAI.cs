using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LumberjackAI : MonoBehaviour
{
    private NavMeshAgent agent;        // 控制AI移动的导航网格代理
    public Transform baseLocation;     // 基地的位置，伐木工会返回并交付资源
    public float detectionRange = 20f; // 检测资源的范围
    public float collectionTime = 2f;  // 采集资源所需的时间

    private GameObject targetResource; // 当前目标资源
    private bool hasWood = false;      // 表示是否采集到木材资源

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        StartCoroutine(BehaviorTree()); // 开始执行行为树
    }

    // 行为树主循环
    IEnumerator BehaviorTree()
    {
        while (true)
        {
            yield return IdleOrSearchForWood(); // 寻找木材资源
            yield return MoveToResource();      // 前往资源位置
            yield return CollectWood();         // 采集资源
            yield return MoveToBase();          // 返回基地
            yield return DeliverWood();         // 交付资源
        }
    }

    // 空闲或寻找木材资源
    IEnumerator IdleOrSearchForWood()
    {
        // 如果已经有木材资源，则跳过寻找资源的行为
        if (hasWood)
            yield break;

        // 查找最近的木材资源
        targetResource = FindNearestWood();
        if (targetResource == null)
        {
            // 未找到木材，进入等待状态
            Debug.Log("没有找到木材，等待中...");
            yield return new WaitForSeconds(1f);
        }
        yield break;
    }

    // 移动到资源位置
    IEnumerator MoveToResource()
    {
        // 如果没有资源或已经有木材，则不进行移动
        if (targetResource == null || hasWood)
            yield break;

        // 设置导航代理的目的地为资源位置
        agent.SetDestination(targetResource.transform.position);
        while (Vector3.Distance(transform.position, targetResource.transform.position) > agent.stoppingDistance)
        {
            yield return null; // 等待移动到达目标
        }
    }

    // 采集木材
    IEnumerator CollectWood()
    {
        // 如果没有资源或已经有木材，则跳过采集
        if (targetResource == null || hasWood)
            yield break;

        Debug.Log("采集中...");
        yield return new WaitForSeconds(collectionTime); // 等待采集完成

        Destroy(targetResource); // 销毁资源对象
        hasWood = true;          // 标记已拥有木材
        Debug.Log("采集完成");
    }

    // 返回基地
    IEnumerator MoveToBase()
    {
        // 如果没有木材，则不返回基地
        if (!hasWood)
            yield break;

        // 设置导航代理的目的地为基地位置
        agent.SetDestination(baseLocation.position);
        while (Vector3.Distance(transform.position, baseLocation.position) > agent.stoppingDistance)
        {
            yield return null; // 等待移动到达基地
        }
    }

    // 交付木材
    IEnumerator DeliverWood()
    {
        // 如果没有木材，则跳过交付
        if (!hasWood)
            yield break;

        Debug.Log("交付木材");
        hasWood = false; // 重置木材状态，准备下次采集
        yield break;
    }

    // 查找最近的木材资源
    GameObject FindNearestWood()
    {
        GameObject[] woodResources = GameObject.FindGameObjectsWithTag("Wood"); // 查找所有标记为 "Wood" 的资源对象
        GameObject nearestWood = null;      // 最近的木材对象
        float closestDistance = detectionRange; // 初始化最近距离为检测范围

        foreach (var wood in woodResources)
        {
            float distance = Vector3.Distance(transform.position, wood.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestWood = wood; // 更新最近木材对象
            }
        }
        return nearestWood; // 返回找到的最近木材对象
    }
}
