using System.Collections.Generic;
using UnityEngine;

public class AttackRangeDetector : MonoBehaviour
{
    // ί���������ֱ����ڽ���������ʿ���ļ��
    public delegate void BuildingDetectedHandler(GameObject building);
    public delegate void MonsterDetectedHandler(GameObject monster);
    public delegate void SoldierDetectedHandler(GameObject soldier);

    // �¼�����������֪ͨ������������ж�����빥����Χ
    public event BuildingDetectedHandler OnBuildingDetected;
    public event MonsterDetectedHandler OnMonsterDetected;
    public event SoldierDetectedHandler OnSoldierDetected;

    // �¼�����������֪ͨ������������ж����뿪������Χ
    public event BuildingDetectedHandler OnBuildingExited;
    public event MonsterDetectedHandler OnMonsterExited;
    public event SoldierDetectedHandler OnSoldierExited;


    public List<GameObject> MonsterAttackList = new List<GameObject>();
    public List<GameObject> SoldierAttackList = new List<GameObject>();

    private void Start()
    {

    }
    // ���ж�����봥������Χʱ����
    void OnTriggerEnter(Collider other)
    {
        // ��⵽��������轨����Ĳ㼶Ϊ11��
        if (other.gameObject.layer == 11)
        {
            GameObject building = other.gameObject;
            //Debug.Log($"AttackRange detected Building: {building.name}");
            MonsterAttackList.Add(building);
            //OnBuildingDetected?.Invoke(building); // ��������������¼�
        }

        // ��⵽����������Ĳ㼶Ϊ15��
        if (other.gameObject.layer == 15)
        {
            GameObject monster = other.gameObject;
            //Debug.Log($"AttackRange detected Monster: {monster.name}");
            SoldierAttackList.Add(monster);
            //OnMonsterDetected?.Invoke(monster); // ������������¼�
        }

        // ��⵽ʿ��������ʿ���Ĳ㼶Ϊ16��
        if (other.gameObject.layer == 16)
        {
            GameObject soldier = other.gameObject;
            //Debug.Log($"AttackRange detected Soldier: {soldier.name}");
            MonsterAttackList.Add(soldier);
            //OnSoldierDetected?.Invoke(soldier); // ����ʿ�������¼�
        }
    }

    // ���ж����뿪��������Χʱ����
    void OnTriggerExit(Collider other)
    {
        // ��⵽�������뿪�����轨����Ĳ㼶Ϊ11��
        if (other.gameObject.layer == 11)
        {
            GameObject building = other.gameObject;
            //Debug.Log($"AttackRange exited Building: {building.name}");
            MonsterAttackList.Remove(building);

            //OnBuildingExited?.Invoke(building); // �����������뿪�¼�
        }

        // ��⵽�����뿪���������Ĳ㼶Ϊ15��
        if (other.gameObject.layer == 15)
        {
            GameObject monster = other.gameObject;
            //Debug.Log($"AttackRange exited Monster: {monster.name}");
            SoldierAttackList.Remove(monster);
            //OnMonsterExited?.Invoke(monster); // ���������뿪�¼�
        }

        // ��⵽ʿ���뿪������ʿ���Ĳ㼶Ϊ16��
        if (other.gameObject.layer == 16)
        {
            GameObject soldier = other.gameObject;
            //Debug.Log($"AttackRange exited Soldier: {soldier.name}");
            MonsterAttackList.Remove(soldier);
            //OnSoldierExited?.Invoke(soldier); // ����ʿ���뿪�¼�
        }
    }
}
