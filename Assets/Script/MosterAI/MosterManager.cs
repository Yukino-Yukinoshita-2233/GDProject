using MapManagernamespace;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    Transform monsterParent; 
    public GameObject goblinPrefab; // �粼�ֵ�Ԥ�Ƽ�
    public GameObject rockDragonPrefab; // ������Ԥ�Ƽ�
    public GameObject flyDragonPrefab; // ������Ԥ�Ƽ�

    public int[,] gridMap; // ��ͼ����
    private List<Monster> monsters = new List<Monster>(); // �������ɵĹ���
    public GameObject Castle;   //�Ǳ�
    void Start()
    {
        gridMap = MapManager.gridMap; // ��ȡ��ͼ����
        monsterParent = GameObject.Find("Monster").transform;
        //Castle = GameObject.Find("Castle");//
        //SpawnMonsters();
    }

    [ContextMenu("AddMonster")]
    // ������ɹ���
    void SpawnMonsters()
    {
        Vector2Int targetPos = new Vector2Int(Mathf.RoundToInt(Castle.transform.position.x), Mathf.RoundToInt(Castle.transform.position.z));
        //Debug.Log(targetPos);
        for (int i = 0; i < 5; i++) // �������5������
        {
            Vector2Int startPos = GetRandomPosition();
            //Vector2Int targetPos = GetRandomPosition();

            SpawnMonster(startPos, targetPos);
        }
    }

    // ���ɵ�������
    void SpawnMonster(Vector2Int startPos, Vector2Int targetPos)
    {
        GameObject monsterObject;
        Monster monster;

        if (Random.value > 0.5f)
        {
            monsterObject = Instantiate(goblinPrefab, new Vector3(startPos.x, 1, startPos.y), Quaternion.identity, monsterParent);
            monster = monsterObject.GetComponent<Goblin>();
            HealthBarManager.Instance.CreateHealthBar(monsterObject);
        }
        else
        {
            monsterObject = Instantiate(rockDragonPrefab, new Vector3(startPos.x, 1, startPos.y), Quaternion.identity, monsterParent);
            monster = monsterObject.GetComponent<RockDragon>();
            HealthBarManager.Instance.CreateHealthBar(monsterObject);
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

    // �����ȡ��ͼ�ϵ�һ����Чλ��
    Vector2Int GetRandomPosition()
    {
        int x, y;
        do
        {
            x = Random.Range(0, gridMap.GetLength(0));
            y = Random.Range(0, gridMap.GetLength(1));
        } while (gridMap[x, y] != 0); // ֻ��ֵΪ0�Ŀհ�λ�òſ�����

        return new Vector2Int(x, y);
    }
}
