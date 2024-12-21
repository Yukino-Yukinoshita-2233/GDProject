using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100; // �������ֵ
    public float currentHealth;  // ��ǰ����ֵ

    [Header("Health Bar Settings")]
    public GameObject healthBarPrefab; // Ѫ��Ԥ����
    private GameObject healthBarInstance; // Ѫ��ʵ��
    private Image healthBarFill;         // Ѫ����䲿��
    private RectTransform canvas;       // Ѫ�����ڵ� Canvas

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
        // ��ʼ��Ѫ��
        //InitializeHealthBar();
    }

    private void Update()
    {
        // ����Ѫ��λ��
        if (healthBarInstance != null)
        {
            UpdateHealthBarPosition();
        }
    }

    /// <summary>
    /// ��ʼ��Ѫ����
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
    /// ����Ѫ����λ�á�
    /// </summary>
    private void UpdateHealthBarPosition()
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2); // ƫ��
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
    /// �����ܵ����˺���
    /// </summary>
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage! Current health: {currentHealth}");

        // ����Ѫ�����
        //UpdateHealthBarFill();

        healthChange?.Invoke(currentHealth / maxHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// ����Ѫ�����״̬��
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
    /// ���������߼���
    /// </summary>
    private void Die()
    {
        Debug.Log($"{gameObject.name} has died!");

        // ����Ѫ��
        if (healthBarInstance != null)
        {
            Destroy(healthBarInstance);
        }

        // ���ٶ���
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        // ����Ѫ��ʵ��
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
