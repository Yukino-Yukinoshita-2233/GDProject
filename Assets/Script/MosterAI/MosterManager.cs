using System.Collections.Generic;
using UnityEngine;
using MapManagernamespace;

public class MonsterManager : MonoBehaviour
{
    public Transform monsterParent; // ����ĸ�����
    public GameObject goblinPrefab; // �粼�ֵ�Ԥ�Ƽ�
    public GameObject rockDragonPrefab; // ������Ԥ�Ƽ�
    public GameObject flyDragonPrefab; // ������Ԥ�Ƽ�

    public int[,] gridMap; // ��ͼ����
    private List<Monster> monsters = new List<Monster>(); // �������й���
    public static Transform castle; // �Ǳ�
    void Start()
    {
        gridMap = MapManager.gridMap; // ��ȡ��ͼ����
        monsterParent = GameObject.Find("Monster").transform;
        castle = GameObject.Find("Building").transform.Find("Castle");
        //SpawnMonsters();
    }


    [ContextMenu("AddMonster")]
    void SpawnMonsters()
    {
        Vector2Int targetPos = new Vector2Int(
            Mathf.RoundToInt(castle.transform.position.x),
            Mathf.RoundToInt(castle.transform.position.z)
        );

        for (int i = 0; i < 5; i++) // �������5������
        {
            Vector2Int startPos = GetRandomPosition();
            SpawnMonster(startPos, targetPos);
        }
    }

    void SpawnMonster(Vector2Int startPos, Vector2Int targetPos)
    {
        GameObject monsterObject;
        Monster monster;

        // ���ѡ���������
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

        // ����Ѫ��
        HealthBarManager.Instance.CreateHealthBar(monsterObject);
    }

    void Update()
    {
        foreach (Monster monster in monsters)
        {
            monster.UpdateMonster();

        }
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
