using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LumberjackAI : MonoBehaviour
{
    public int[,] map;                  // ��ͼ��ά���飺0 ��ʾ�����ߣ�1 ��ʾ�ϰ�
    public Vector2Int start;            // ��ľ����ʼλ��
    public Vector2Int baseLocation;     // ����λ��
    public float moveSpeed = 2f;        // �ƶ��ٶ�

    private List<Node> path;            // ��ǰ·��
    private Vector2Int targetResource;  // ��ǰĿ����Դλ��

    void Start()
    {
        // ͨ����Ϊ��ִ�з�ľ������Ϊ
        StartCoroutine(BehaviorTree());
    }

    IEnumerator BehaviorTree()
    {
        while (true)
        {
            yield return IdleOrSearchForWood();  // Ѱ��ľ����Դ
            yield return MoveToTarget(targetResource);  // ǰ����Դλ��
            yield return CollectWood();          // �ɼ���Դ
            yield return MoveToTarget(baseLocation); // ���ػ���
            yield return DeliverWood();          // ������Դ
        }
    }

    // ���������ľ����Դλ��
    IEnumerator IdleOrSearchForWood()
    {
        targetResource = FindNearestWood();
        yield break;
    }

    // �ƶ���Ŀ��λ��
    IEnumerator MoveToTarget(Vector2Int targetPosition)
    {
        path = AStarPathfinding.FindPath(map, start, targetPosition); // ʹ��A*�㷨����·��

        foreach (var node in path)
        {
            Vector3 worldPos = new Vector3(node.Position.x, 0, node.Position.y);
            while (Vector3.Distance(transform.position, worldPos) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, worldPos, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }
        start = targetPosition; // ���·�ľ����ǰλ��
    }

    // �ɼ�ľ��
    IEnumerator CollectWood()
    {
        Debug.Log("�ɼ����");
        yield return new WaitForSeconds(2f); // ģ��ɼ�ʱ��
    }

    // ����ľ��
    IEnumerator DeliverWood()
    {
        Debug.Log("�������");
        yield break;
    }

    // ���������ľ����Դλ��
    Vector2Int FindNearestWood()
    {
        // ��������ľ����Դ�����걣���� woodPositions �б���
        List<Vector2Int> woodPositions = new List<Vector2Int>(); // �����б���Ҫ�ڳ����г�ʼ��
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
