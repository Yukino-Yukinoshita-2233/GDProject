using UnityEngine;

// Dragon ��̳��� Monster�������ض����Ժ���Ϊ
public class RockDragon : Monster
{
    void Awake()
    {
        moveSpeed = 1.5f;
        damage = 20;
        maxHealth = 200f;
        currentHealth = maxHealth;
    }

    protected override void Attack()
    {
        Debug.Log("RockDragon is using a heavy attack!");
    }
}