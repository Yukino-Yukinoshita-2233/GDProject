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
    public int LevelNum = 1;
    void Start()
    {
        gridMap = MapManager.gridMap; // ��ȡ��ͼ����
        monsterParent = GameObject.Find("Monster").transform;
        castle = GameObject.Find("Building").transform.Find("Castle");
        //LevelNum = 1;
        SpawnMonsters();
    }
    void Update()
    {
        // ���������б��Ƴ������ٵĹ���
        monsters.RemoveAll(monster => monster == null);

        // ȷ�������б�Ϊ��ʱ�������ɹ���
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

        for (int i = 0; i < LevelNum; i++) // ������ɹ���
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

        // ���ѡ���������
        if (Random.value > 0.3f)
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


    Vector2Int GetRandomPosition()
    {
        int x = 0, y = 0;
        int i = Random.Range(0, 4); // i ��Ҫ�� 0 �� 3 ֮��

        do
        {
            switch (i)
            {
                case 0: // ѡ����߽�
                    x = 0;
                    y = Random.Range(0, gridMap.GetLength(1));
                    break;
                case 1: // ѡ���ϱ߽�
                    x = Random.Range(0, gridMap.GetLength(0));
                    y = 0;
                    break;
                case 2: // ѡ���ұ߽�
                    x = gridMap.GetLength(0) - 1; // ��ֹ����Խ��
                    y = Random.Range(0, gridMap.GetLength(1));
                    break;
                case 3: // ѡ���±߽�
                    x = Random.Range(0, gridMap.GetLength(0));
                    y = gridMap.GetLength(1) - 1; // ��ֹ����Խ��
                    break;
            }
        } while (gridMap[x, y] != 0); // ȷ��ѡ�е�λ���ǿ����ߵ�

        return new Vector2Int(x, y);
    }
}
