using UnityEngine;

// Dragon ��̳��� Monster�������ض����Ժ���Ϊ
public class RockDragon : Monster
{
    private void Awake()
    {
        health = 30; // ���÷���������ֵ
        damage = 4;  // ���÷������˺�ֵ
        moveSpeed = 2f; // ���÷������ƶ��ٶ�
    }

    // ��д��������������ض��Ĺ�����Ϊ
    protected override void Attack()
    {
        base.Attack();
        Debug.Log("RockDragon deals " + damage + " damage.");
    }

    // ��д����������ִ���ض�����Ч��
    protected override void OnDeath()
    {
        base.OnDeath();
        Debug.Log("RockDragon death animation triggered.");
    }
}
