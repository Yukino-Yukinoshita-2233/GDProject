using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public Transform target; // Ŀ��λ��
    private MapGenerator mapGenerator;
    private AStarPathfinding pathfinding;

    void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();
        pathfinding = new AStarPathfinding();

    }

    //void Update()
    //{
    //    // ����AI����ʼλ��Ϊ��������½�
    //    GridNode startNode = mapGenerator.grid[0, 0];
    //    // ��ȡĿ��ڵ�
    //    GridNode targetNode = mapGenerator.grid[(int)target.position.x, (int)target.position.z];

    //    // ����·��
    //    List<GridNode> path = pathfinding.FindPath(startNode, targetNode, mapGenerator.grid);

    //    if (path != null)
    //    {
    //        // ����·��������AI�ƶ�����һ���ڵ�
    //        foreach (GridNode node in path)
    //        {
    //            // �������ʵ��AI�ƶ��߼������磺
    //            transform.position = Vector3.MoveTowards(transform.position, node.WorldPosition, Time.deltaTime * 5);
    //        }
    //    }
    //}

    // ����һ������������������Ϊ����
    public void PlanPath(float[,] grid)
    {
        if (grid == null)
        {
            Debug.LogError("Grid not initialized!");
            return;
        }

        // ���������·���滮���ƶ��߼�
    }
}
