using UnityEngine;

// Dragon 类继承自 Monster，包含特定属性和行为
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