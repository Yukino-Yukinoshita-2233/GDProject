using System.Collections.Generic;
using UnityEngine;

public class AttackRangeDetector : MonoBehaviour
{
    // 委托声明，分别用于建筑物、怪物和士兵的检测
    public delegate void BuildingDetectedHandler(GameObject building);
    public delegate void MonsterDetectedHandler(GameObject monster);
    public delegate void SoldierDetectedHandler(GameObject soldier);

    // 事件声明，用于通知其他组件或类有对象进入攻击范围
    public event BuildingDetectedHandler OnBuildingDetected;
    public event MonsterDetectedHandler OnMonsterDetected;
    public event SoldierDetectedHandler OnSoldierDetected;

    // 事件声明，用于通知其他组件或类有对象离开攻击范围
    public event BuildingDetectedHandler OnBuildingExited;
    public event MonsterDetectedHandler OnMonsterExited;
    public event SoldierDetectedHandler OnSoldierExited;


    public List<GameObject> MonsterAttackList = new List<GameObject>();
    public List<GameObject> SoldierAttackList = new List<GameObject>();

    private void Start()
    {

    }
    // 当有对象进入触发器范围时调用
    void OnTriggerEnter(Collider other)
    {
        // 检测到建筑物（假设建筑物的层级为11）
        if (other.gameObject.layer == 11)
        {
            GameObject building = other.gameObject;
            //Debug.Log($"AttackRange detected Building: {building.name}");
            MonsterAttackList.Add(building);
            //OnBuildingDetected?.Invoke(building); // 触发建筑物进入事件
        }

        // 检测到怪物（假设怪物的层级为15）
        if (other.gameObject.layer == 15)
        {
            GameObject monster = other.gameObject;
            //Debug.Log($"AttackRange detected Monster: {monster.name}");
            SoldierAttackList.Add(monster);
            //OnMonsterDetected?.Invoke(monster); // 触发怪物进入事件
        }

        // 检测到士兵（假设士兵的层级为16）
        if (other.gameObject.layer == 16)
        {
            GameObject soldier = other.gameObject;
            //Debug.Log($"AttackRange detected Soldier: {soldier.name}");
            MonsterAttackList.Add(soldier);
            //OnSoldierDetected?.Invoke(soldier); // 触发士兵进入事件
        }
    }

    // 当有对象离开触发器范围时调用
    void OnTriggerExit(Collider other)
    {
        // 检测到建筑物离开（假设建筑物的层级为11）
        if (other.gameObject.layer == 11)
        {
            GameObject building = other.gameObject;
            //Debug.Log($"AttackRange exited Building: {building.name}");
            MonsterAttackList.Remove(building);

            //OnBuildingExited?.Invoke(building); // 触发建筑物离开事件
        }

        // 检测到怪物离开（假设怪物的层级为15）
        if (other.gameObject.layer == 15)
        {
            GameObject monster = other.gameObject;
            //Debug.Log($"AttackRange exited Monster: {monster.name}");
            SoldierAttackList.Remove(monster);
            //OnMonsterExited?.Invoke(monster); // 触发怪物离开事件
        }

        // 检测到士兵离开（假设士兵的层级为16）
        if (other.gameObject.layer == 16)
        {
            GameObject soldier = other.gameObject;
            //Debug.Log($"AttackRange exited Soldier: {soldier.name}");
            MonsterAttackList.Remove(soldier);
            //OnSoldierExited?.Invoke(soldier); // 触发士兵离开事件
        }
    }
}
