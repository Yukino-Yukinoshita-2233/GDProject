using UnityEngine;

/// <summary>
/// ����Ѫ�������ࡣ
/// </summary>
public class Health : MonoBehaviour
{
    public int maxHealth = 100; // �������ֵ
    private int currentHealth; // ��ǰ����ֵ

    private void Start()
    {
        currentHealth = maxHealth;
    }

    /// <summary>
    /// �����ܵ����˺���
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
    /// ���������߼���
    /// </summary>
    private void Die()
    {
        Debug.Log($"{gameObject.name} has died!");
        Destroy(gameObject); // ���ٶ���
    }
}
