using UnityEngine;

// Goblin ��̳��� Monster�������ض����Ժ���Ϊ
public class Goblin : Monster
{
    private void Awake()
    {
        health = 20; // ���ø粼�ֵ�����ֵ
        damage = 2;  // ���ø粼�ֵ��˺�ֵ
        moveSpeed = 3f; // ���ø粼�ֵ��ƶ��ٶ�
    }

    // ��д��������������ض��Ĺ�����Ϊ
    protected override void Attack()
    {
        base.Attack();
        //Debug.Log("Goblin deals " + damage + " damage.");
    }

    // ��д����������ִ���ض�����Ч��
    protected override void OnDeath()
    {
        base.OnDeath();
        //Debug.Log("Goblin death animation triggered.");
    }
}
