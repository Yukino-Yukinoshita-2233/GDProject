using UnityEngine;
using static AttackRangeDetector;

public class AttackRangeDetector : MonoBehaviour
{
    public delegate void BuildingDetectedHandler(GameObject building);
    public delegate void MonsterDetectedHandler(GameObject monster);
    public delegate void SoldierDetectedHandler(GameObject soldier);
    public event BuildingDetectedHandler OnBuildingDetected;
    public event MonsterDetectedHandler OnMonsterDetected;
    public event SoldierDetectedHandler OnSoldierDetected;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 11)
        {
            GameObject building = other.gameObject;
            Debug.Log($"AttackRange detected Building: {building.name}");

            // �����¼���֪ͨ������
            OnBuildingDetected?.Invoke(building);
        }
        if (other.gameObject.layer == 15)
        {
            GameObject monster = other.gameObject;
            Debug.Log($"AttackRange detected Building: {monster.name}");

            // �����¼���֪ͨ������
            OnMonsterDetected?.Invoke(monster);
        }
        if (other.gameObject.layer == 16)
        {
            GameObject soldier = other.gameObject;
            Debug.Log($"AttackRange detected Building: {soldier.name}");

            // �����¼���֪ͨ������
            OnSoldierDetected?.Invoke(soldier);
        }
    }
    //void OnTriggerStay(Collider other)
    //{
    //    if (other.gameObject.layer == 11)
    //    {
    //        GameObject building = other.gameObject;
    //        Debug.Log($"AttackRange detected Building: {building.name}");

    //        // �����¼���֪ͨ������
    //        OnBuildingDetected?.Invoke(building);
    //    }
    //    if (other.gameObject.layer == 15)
    //    {
    //        GameObject monster = other.gameObject;
    //        Debug.Log($"AttackRange detected Building: {monster.name}");

    //        // �����¼���֪ͨ������
    //        OnMonsterDetected?.Invoke(monster);
    //    }
    //    if (other.gameObject.layer == 16)
    //    {
    //        GameObject soldier = other.gameObject;
    //        Debug.Log($"AttackRange detected Building: {soldier.name}");

    //        // �����¼���֪ͨ������
    //        OnSoldierDetected?.Invoke(soldier);
    //    }
    //}
}
