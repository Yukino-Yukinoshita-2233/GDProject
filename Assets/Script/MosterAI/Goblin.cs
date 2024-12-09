using UnityEngine;

// Dragon ��̳��� Monster�������ض����Ժ���Ϊ
public class Goblin : Monster
{
    void Awake()
    {
        moveSpeed = 2f;
        damage = 10;
        maxHealth = 50f;
        currentHealth = maxHealth;
    }

    protected override void Attack()
    {
        Debug.Log("Goblin is attacking!");
    }
}