using UnityEngine;

// Dragon ��̳��� Monster�������ض����Ժ���Ϊ
public class FlyDragon : Monster
{
    private void Awake()
    {
        health = 10; // ���÷���������ֵ
        damage = 6;  // ���÷������˺�ֵ
        moveSpeed = 4f; // ���÷������ƶ��ٶ�
    }

    // ��д��������������ض��Ĺ�����Ϊ
    protected override void Attack()
    {
        base.Attack();
        Debug.Log("FlyDragon deals " + damage + " damage.");
    }

    // ��д����������ִ���ض�����Ч��
    protected override void OnDeath()
    {
        base.OnDeath();
        Debug.Log("FlyDragon death animation triggered.");
    }
}
