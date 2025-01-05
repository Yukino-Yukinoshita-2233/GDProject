using System.Collections.Generic;
using UnityEngine;
using MapManagernamespace;

public class MonsterManager : MonoBehaviour
{
    public Transform monsterParent; // 怪物的父物体
    public GameObject goblinPrefab; // 哥布林的预制件
    public GameObject rockDragonPrefab; // 岩龙的预制件
    public GameObject flyDragonPrefab; // 飞龙的预制件

    public int[,] gridMap; // 地图数据
    private List<Monster> monsters = new List<Monster>(); // 保存所有怪物
    public static Transform castle; // 城堡
    public int LevelNum = 1;
    void Start()
    {
        gridMap = MapManager.gridMap; // 获取地图数据
        monsterParent = GameObject.Find("Monster").transform;
        castle = GameObject.Find("Building").transform.Find("Castle");
        LevelNum = 1;
        SpawnMonsters();
    }
    void Update()
    {
        // 遍历怪物列表，移除已销毁的怪物
        monsters.RemoveAll(monster => monster == null);

        // 确保怪物列表为空时重新生成怪物
        if (monsters.Count <= 0)
        {
            SpawnMonsters();
        }

        foreach (Monster monster in monsters)
        {
            if (monster != null)
            {
                monster.UpdateMonster();
            }
        }
    }

    [ContextMenu("AddMonster")]
    void SpawnMonsters()
    {
        Debug.Log("level" + LevelNum);

        Vector2Int targetPos = new Vector2Int(
            Mathf.RoundToInt(castle.transform.position.x),
            Mathf.RoundToInt(castle.transform.position.z)
        );

        for (int i = 0; i < LevelNum; i++) // 随机生成怪物
        {
            Vector2Int startPos = GetRandomPosition();
            SpawnMonster(startPos, targetPos);
        }
        LevelNum++;

    }

    void SpawnMonster(Vector2Int startPos, Vector2Int targetPos)
    {
        GameObject monsterObject;
        Monster monster;

        // 随机选择怪物类型
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

        // 创建血条
        HealthBarManager.Instance.CreateHealthBar(monsterObject);
    }


    Vector2Int GetRandomPosition()
    {
        int x, y;
        do
        {
            x = Random.Range(0, gridMap.GetLength(0));
            y = Random.Range(0, gridMap.GetLength(1));
        } while (gridMap[x, y] != 0);

        return new Vector2Int(x, y);
    }
}
