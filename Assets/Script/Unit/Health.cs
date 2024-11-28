using UnityEngine;

/// <summary>
/// 敌人血量管理类。
/// </summary>
public class Health : MonoBehaviour
{
    public int maxHealth = 100; // 最大生命值
    private int currentHealth; // 当前生命值

    private void Start()
    {
        currentHealth = maxHealth;
    }

    /// <summary>
    /// 处理受到的伤害。
    /// </summary>
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage! Current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// 处理死亡逻辑。
    /// </summary>
    private void Die()
    {
        Debug.Log($"{gameObject.name} has died!");
        Destroy(gameObject); // 销毁对象
    }
}
