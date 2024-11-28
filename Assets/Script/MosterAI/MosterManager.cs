using MapManagernamespace;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    Transform monsterParent; 
    public GameObject goblinPrefab; // 哥布林的预制件
    public GameObject rockDragonPrefab; // 岩龙的预制件
    public GameObject flyDragonPrefab; // 飞龙的预制件

    public int[,] gridMap; // 地图数据
    private List<Monster> monsters = new List<Monster>(); // 保存生成的怪物

    void Start()
    {
        gridMap = MapManager.gridMap; // 获取地图数据
        monsterParent = GameObject.Find("Monster").transform;

        //SpawnMonsters();
    }

    [ContextMenu("AddMonster")]
    // 随机生成怪物
    void SpawnMonsters()
    {
        for (int i = 0; i < 5; i++) // 随机生成5个怪物
        {
            Vector2Int startPos = GetRandomPosition();
            Vector2Int targetPos = GetRandomPosition();

            SpawnMonster(startPos, targetPos);
        }
    }

    // 生成单个怪物
    void SpawnMonster(Vector2Int startPos, Vector2Int targetPos)
    {
        GameObject monsterObject;
        Monster monster;

        if (Random.value > 0.5f)
        {
            monsterObject = Instantiate(goblinPrefab, new Vector3(startPos.x, 1, startPos.y), Quaternion.identity, monsterParent);
            monster = monsterObject.GetComponent<Goblin>();
        }
        else
        {
            monsterObject = Instantiate(rockDragonPrefab, new Vector3(startPos.x, 1, startPos.y), Quaternion.identity, monsterParent);
            monster = monsterObject.GetComponent<RockDragon>();
        }

        monster.Initialize(startPos, targetPos, gridMap);
        monsters.Add(monster);
    }

    void Update()
    {
        foreach (Monster monster in monsters)
        {
            monster.UpdateMonster();
        }
    }

    // 随机获取地图上的一个有效位置
    Vector2Int GetRandomPosition()
    {
        int x, y;
        do
        {
            x = Random.Range(0, gridMap.GetLength(0));
            y = Random.Range(0, gridMap.GetLength(1));
        } while (gridMap[x, y] != 0); // 只有值为0的空白位置才可行走

        return new Vector2Int(x, y);
    }
}
