using System.Linq;
using UnityEngine;

public class Goblin : Monster
{
    //private float lastAttackTime;  // ��һ�ι�����ʱ��
    //private float attackCooldownTimer;  // ������ȴ��ʱ��
    //AttackRangeDetector attackRangeDetector;
    // �粼�ֵĳ�ʼ��
    void Awake()
    {
        //moveSpeed = 1f;            // ���ø粼�ֵ��ƶ��ٶ�
        //damage = 10;               // ���ø粼�ֵĹ�����
        //maxHealth = 50f;           // ���ø粼�ֵ����Ѫ��
        //attackCooldown = 2.0f;     // ���ù�����ȴʱ��
        //currentHealth = maxHealth;

        // ��ȡ������Χ�������ע���¼�
        //attackRangeDetector = GetComponentInChildren<AttackRangeDetector>();
        //if (attackRangeDetector != null)
        //{
        //    attackRangeDetector.OnBuildingDetected += HandleBuildingDetected;
        //    attackRangeDetector.OnSoldierDetected += HandleSoldierDetected;
        //    attackRangeDetector.OnBuildingExited += HandleBuildingExit;
        //    attackRangeDetector.OnSoldierExited += HandleSoldierExit;
        //}

        // ��ȡѪ�����������Ѫ���仯
        Health health = gameObject.GetComponent<Health>();
        if (health != null)
        {
            health.SetHealth(maxHealth);
            //health.healthChange += TakeDamage;
        }

        // ��ȡ������Χ�����
        attackRangeDetector = GetComponentInChildren<AttackRangeDetector>();
        lastAttackTime = Time.time;  // ��ʼ������ʱ��

        //lastAttackTime = Time.time;  // ��ʼ������ʱ��
        //attackCooldownTimer = attackCooldown;  // ��ʼ��������ȴ��ʱ��
    }

    // ��������ļ���߼�
    protected override void HandleBuildingDetected(GameObject building)
    {
        if (!currentTargets.Contains(building))
        {
            currentTargets.Add(building);  // ���Ŀ�����µĽ������ӵ�����Ŀ���б�
            //building.GetComponent<Health>().TakeDamage(damage * 2);  // �粼�ֶԽ��������˫���˺�
        }
    }

    // ����ʿ���ļ���߼�
    protected override void HandleSoldierDetected(GameObject soldier)
    {
        if (!currentTargets.Contains(soldier))
        {
            currentTargets.Add(soldier);  // ���Ŀ�����µ�ʿ������ӵ�����Ŀ���б�
            //soldier.GetComponent<Health>().TakeDamage(damage);  // �粼�ֶ�ʿ����ɵ��˺�
        }
    }

    // �����߼�����Ե�ǰĿ����й�����
    //protected override void Attack()
    //{
        // ����Ƿ���Ϲ�����ȴ����
        //if (Time.time - lastAttackTime >= attackCooldown && currentTargets.Count != 0)
        //{
        //    GameObject target = currentTargets.First();
        //    if (target != null)
        //    {
        //        Health targetHealth = target.GetComponent<Health>();
        //        if (targetHealth != null)
        //        {
        //            Debug.Log("Goblin,Damage:" + damage);
        //            targetHealth.TakeDamage(damage);  // ��Ŀ������˺�
        //        }
        //    }

        //    lastAttackTime = Time.time;  // ������󹥻���ʱ��
        //}
    //}

    // ���������뿪������Χ���߼�
    protected override void HandleBuildingExit(GameObject building)
    {
        if (currentTargets.Contains(building))
        {
            currentTargets.Remove(building);  // ���Ŀ���ǽ�����Ƴ���
            if (currentTargets.Count == 0)
                currentState = State.Moving;  // ���û��Ŀ�꣬�ָ��ƶ�״̬
        }
    }

    // ����ʿ���뿪������Χ���߼�
    protected override void HandleSoldierExit(GameObject soldier)
    {
        if (currentTargets.Contains(soldier))
        {
            currentTargets.Remove(soldier);  // ���Ŀ����ʿ�����Ƴ���
            if (currentTargets.Count == 0)
                currentState = State.Moving;  // ���û��Ŀ�꣬�ָ��ƶ�״̬
        }
    }
}
