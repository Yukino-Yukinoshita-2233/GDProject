using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100; // �������ֵ
    private int currentHealth;  // ��ǰ����ֵ

    [Header("Health Bar Settings")]
    public GameObject healthBarPrefab; // Ѫ��Ԥ����
    private GameObject healthBarInstance; // Ѫ��ʵ��
    private Image healthBarFill;         // Ѫ����䲿��
    private RectTransform canvas;       // Ѫ�����ڵ� Canvas

    public delegate void HealthChange(int health);
    public event HealthChange healthChange;
    private void Start()
    {
        currentHealth = maxHealth;

        // ��ʼ��Ѫ��
        InitializeHealthBar();
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

        // ����Ѫ��ʵ��
        healthBarInstance = Instantiate(healthBarPrefab, canvas);
        healthBarFill = healthBarInstance.transform.Find("Fill").GetComponent<Image>();

        if (healthBarFill == null)
        {
            Debug.LogError("HealthBar prefab is missing 'Fill' Image component!");
            Destroy(healthBarInstance);
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
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage! Current health: {currentHealth}");

        // ����Ѫ�����
        UpdateHealthBarFill();

        healthChange?.Invoke(currentHealth);
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
}
