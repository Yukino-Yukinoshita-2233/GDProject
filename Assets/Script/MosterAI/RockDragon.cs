using UnityEngine;

// Dragon ��̳��� Monster�������ض����Ժ���Ϊ
public class RockDragon : Monster
{
    void Awake()
    {
        moveSpeed = 1f;
        damage = 20;
        maxHealth = 200f;
        currentHealth = maxHealth;
        attackCooldown = 1.0f;

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
        //Debug.Log("RockDragon is using a heavy attack!");
    }
}