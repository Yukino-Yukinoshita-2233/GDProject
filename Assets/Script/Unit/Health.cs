using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100; // 最大生命值
    public float currentHealth;  // 当前生命值

    [Header("Health Bar Settings")]
    public GameObject healthBarPrefab; // 血条预制体
    private GameObject healthBarInstance; // 血条实例
    private Image healthBarFill;         // 血条填充部分
    private RectTransform canvas;       // 血条所在的 Canvas

    public delegate void HealthChange(float health);
    public event HealthChange healthChange;

    private void Start()
    {
        currentHealth = maxHealth;
        HealthBarManager healthBarManager = gameObject.transform.GetComponentInParent<HealthBarManager>();
        if (healthBarInstance != null )
        {
            healthBarPrefab = healthBarManager.healthBarPrefab;
        }
        // 初始化血条
        //InitializeHealthBar();
    }

    private void Update()
    {
        // 更新血条位置
        if (healthBarInstance != null)
        {
            UpdateHealthBarPosition();
        }
    }

    /// <summary>
    /// 初始化血条。
    /// </summary>
    private void InitializeHealthBar()
    {
        if (healthBarPrefab == null)
        {
            Debug.LogWarning($"HealthBarPrefab is not assigned for {gameObject.name}!");
            return;
        }

        canvas = FindObjectOfType<Canvas>().GetComponent<RectTransform>();
        if (canvas == null)
        {
            Debug.LogError("No Canvas found in the scene!");
            return;
        }

    }

    /// <summary>
    /// 更新血条的位置。
    /// </summary>
    private void UpdateHealthBarPosition()
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2); // 偏移
        healthBarInstance.transform.position = screenPosition;

        if (screenPosition.z > 0)
        {
            healthBarInstance.SetActive(true);
        }
        else
        {
            healthBarInstance.SetActive(false);
        }
    }

    /// <summary>
    /// 处理受到的伤害。
    /// </summary>
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage! Current health: {currentHealth}");

        // 更新血条填充
        //UpdateHealthBarFill();

        healthChange?.Invoke(currentHealth / maxHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// 更新血条填充状态。
    /// </summary>
    private void UpdateHealthBarFill()
    {
        if (healthBarFill != null)
        {
            //Debug.Log(currentHealth + maxHealth + currentHealth / maxHealth);
            healthBarFill.fillAmount = Mathf.Clamp01((float)currentHealth / maxHealth);
        }
    }

    /// <summary>
    /// 处理死亡逻辑。
    /// </summary>
    private void Die()
    {
        Debug.Log($"{gameObject.name} has died!");

        // 销毁血条
        if (healthBarInstance != null)
        {
            Destroy(healthBarInstance);
        }

        // 销毁对象
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        // 清理血条实例
        if (healthBarInstance != null)
        {
            Destroy(healthBarInstance);
        }
    }

    public float GetHealth()
    {
        return currentHealth;
    }

    public void SetHealth(float health)
    {
        maxHealth = health;
        currentHealth = health;
    }
}
