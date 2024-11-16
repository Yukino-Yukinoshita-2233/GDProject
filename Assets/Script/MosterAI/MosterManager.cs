using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapManagernamespace;

public class MosterManager : MonoBehaviour
{
    MapManager MapManager;
    public int[,] MonstergridMap = null;
    public Vector2Int MonsterStartPos;
    public Vector2Int MonstertargetPos;
    public List<Node> MonsterPathList;
    // Start is called before the first frame update
    void Start()
    {
        //MapManager = MapManager.Instance;
        MonstergridMap = MapManager.gridMap;
        Debug.Log("���MonsterManager��ȡ�ĵ�ͼ����:");
        //MapManager.Print2DArray(MonstergridMap);
        GetAStarPath();
    }

    void GetAStarPath()
    {
        Debug.Log("MonsterMansger��ȡѰ··��");
        MonsterPathList = AStarPathfinding.FindPath(MonstergridMap, MonsterStartPos, MonstertargetPos);
        Debug.Log(MonsterPathList);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }
}
