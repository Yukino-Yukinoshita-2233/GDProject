using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 士兵管理器，管理所有士兵对象。
/// </summary>
public class SoldierManager : MonoBehaviour
{
    public static SoldierManager Instance; // 单例模式

    public List<Soldier> soldiers = new List<Soldier>(); // 士兵队列

    Transform soldierParent; // 挂载士兵的父对象

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (soldierParent == null)
        {
            soldierParent = GameObject.Find("Soldier").transform;
            Debug.Log("设置 SoldierManager 的 soldierParent 变量为挂载士兵的父对象");
        }
    }

    private void Start()
    {
        soldierParent = GameObject.Find("Soldier").transform;

        RegisterAllSoldiers();
    }

    /// <summary>
    /// 注册 SoldierParent 下的所有子对象为士兵。
    /// </summary>
    public void RegisterAllSoldiers()
    {
        soldiers.Clear(); // 清空之前的士兵列表

        if (soldierParent != null)
        {
            foreach (Transform child in soldierParent)
            {
                Soldier soldier = child.GetComponent<Soldier>();
                if (soldier != null)
                {
                    soldiers.Add(soldier); // 注册士兵
                }
                else
                {
                    Debug.LogWarning($"子对象 {child.name} 没有 Soldier 脚本，已跳过。");
                }
            }
        }

        Debug.Log($"已注册 {soldiers.Count} 个士兵。");
    }

    /// <summary>
    /// 获取当前注册的所有士兵。
    /// </summary>
    public List<Soldier> GetAllSoldiers()
    {
        return soldiers;
    }

    /// <summary>
    /// 动态注册新士兵。
    /// </summary>
    public void RegisterNewSoldier(Soldier soldier)
    {
        if (soldier != null && !soldiers.Contains(soldier))
        {
            soldiers.Add(soldier);
            Debug.Log($"新士兵 {soldier.name} 已注册！");
        }
    }

    /// <summary>
    /// 注销已移除的士兵。
    /// </summary>
    public void UnregisterSoldier(Soldier soldier)
    {
        if (soldier != null && soldiers.Contains(soldier))
        {
            soldiers.Remove(soldier);
            Debug.Log($"士兵 {soldier.name} 已注销！");
        }
    }
}
