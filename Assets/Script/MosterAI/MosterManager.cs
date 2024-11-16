using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapManagernamespace;
using System.IO;
using UnityEditor.Experimental.GraphView;

public class MosterManager : MonoBehaviour
{
    //MapManager MapManager;
    PrintDebug printDebug;
    public int[,] MonstergridMap = null;
    public Vector2Int MonsterStartPos;
    public Vector2Int MonstertargetPos;
    public List<Node> MonsterPathList;
    // Start is called before the first frame update
    void Start()
    {
        printDebug = new PrintDebug();
        MonstergridMap = MapManager.gridMap;
        Debug.Log("输出MonsterManager获取的地图数据:");
        printDebug.Print2DArray(MonstergridMap);
        GetAStarPath();
    }

    void GetAStarPath()
    {
        Debug.Log("MonsterMansger获取寻路路径");
        MonsterPathList = AStarPathfinding.FindPath(MonstergridMap, MonsterStartPos, MonstertargetPos);
        // 输出路径中的节点信息
        Debug.Log("Path found:");
        foreach (Node node in MonsterPathList)
        {
            Debug.Log(node);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }
}
