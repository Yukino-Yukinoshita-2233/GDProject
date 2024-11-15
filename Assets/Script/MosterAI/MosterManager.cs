using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MosterManager : MonoBehaviour
{
    public int[,] MonstergridMap = null;
    public Vector2Int MonsterStartPos;
    public Vector2Int MonstertargetPos;
    public List<Node> MonsterPathList;
    // Start is called before the first frame update
    void Start()
    {
        MonstergridMap = MapManager.gridMap;
        GetAStarPath();
    }

    void GetAStarPath()
    {
        MonsterPathList = AStarPathfinding.FindPath(MonstergridMap, MonsterStartPos, MonstertargetPos);
        Debug.Log(MonsterPathList);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }
}
