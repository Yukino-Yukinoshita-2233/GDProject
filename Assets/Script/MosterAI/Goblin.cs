using UnityEngine;

// Dragon ��̳��� Monster�������ض����Ժ���Ϊ
public class Goblin : Monster
{
    void Awake()
    {
        moveSpeed = 2f;
        damage = 10;
        maxHealth = 50f;
        attackCooldown = 2.0f;
        currentHealth = maxHealth;
        // �ҵ��������ϵ�AttackRangeDetector�ű�
        AttackRangeDetector attackRangeDetector = GetComponentInChildren<AttackRangeDetector>();
        Health health = gameObject.GetComponent<Health>();

        // ���������崥�����¼�
        if (attackRangeDetector != null)
        {
            attackRangeDetector.OnBuildingDetected += HandleBuildingDetected;
            attackRangeDetector.OnSoldierDetected += HandleSoldierDetected;
        }

        // �������崥�����¼�
        if (health != null)
        {
            health.healthChange += TakeDamage;
        }

    }

    protected override void Attack()
    {
        //Debug.Log("Goblin is attacking!");
    }



}