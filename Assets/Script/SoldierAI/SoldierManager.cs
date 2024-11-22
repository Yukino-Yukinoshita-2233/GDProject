using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ʿ������������������ʿ������
/// </summary>
public class SoldierManager : MonoBehaviour
{
    public static SoldierManager Instance; // ����ģʽ

    public List<Soldier> soldiers = new List<Soldier>(); // ʿ������

    Transform soldierParent; // ����ʿ���ĸ�����

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
            Debug.Log("���� SoldierManager �� soldierParent ����Ϊ����ʿ���ĸ�����");
        }
    }

    private void Start()
    {
        soldierParent = GameObject.Find("Soldier").transform;

        RegisterAllSoldiers();
    }

    /// <summary>
    /// ע�� SoldierParent �µ������Ӷ���Ϊʿ����
    /// </summary>
    public void RegisterAllSoldiers()
    {
        soldiers.Clear(); // ���֮ǰ��ʿ���б�

        if (soldierParent != null)
        {
            foreach (Transform child in soldierParent)
            {
                Soldier soldier = child.GetComponent<Soldier>();
                if (soldier != null)
                {
                    soldiers.Add(soldier); // ע��ʿ��
                }
                else
                {
                    Debug.LogWarning($"�Ӷ��� {child.name} û�� Soldier �ű�����������");
                }
            }
        }

        Debug.Log($"��ע�� {soldiers.Count} ��ʿ����");
    }

    /// <summary>
    /// ��ȡ��ǰע�������ʿ����
    /// </summary>
    public List<Soldier> GetAllSoldiers()
    {
        return soldiers;
    }

    /// <summary>
    /// ��̬ע����ʿ����
    /// </summary>
    public void RegisterNewSoldier(Soldier soldier)
    {
        if (soldier != null && !soldiers.Contains(soldier))
        {
            soldiers.Add(soldier);
            Debug.Log($"��ʿ�� {soldier.name} ��ע�ᣡ");
        }
    }

    /// <summary>
    /// ע�����Ƴ���ʿ����
    /// </summary>
    public void UnregisterSoldier(Soldier soldier)
    {
        if (soldier != null && soldiers.Contains(soldier))
        {
            soldiers.Remove(soldier);
            Debug.Log($"ʿ�� {soldier.name} ��ע����");
        }
    }
}
