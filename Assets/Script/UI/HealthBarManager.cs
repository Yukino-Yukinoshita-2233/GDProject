using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
/**
public class HealthBarManager : MonoBehaviour
{
    public static HealthBarManager Instance { get; private set; }

    [SerializeField] private GameObject healthBarPrefab; // 血条预制体
    [SerializeField] private RectTransform healthBarPanel; // 血条所在的 Panel
    private Dictionary<GameObject, HealthBar> healthBars = new Dictionary<GameObject, HealthBar>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        // 更新每个血条的位置
        foreach (var pair in healthBars)
        {
            UpdateHealthBarPosition(pair.Key, pair.Value);
        }
    }

    public void CreateHealthBar(GameObject entity)
    {
        if (entity == null) return;

        GameObject healthBarObj = Instantiate(healthBarPrefab, healthBarPanel);
        HealthBar healthBar = healthBarObj.GetComponent<HealthBar>();

        if (entity.TryGetComponent(out Monster monster))
        {
            healthBar.Initialize(monster);
        }
        else if (entity.TryGetComponent(out Soldier soldier))
        {
            healthBar.Initialize(soldier);
        }

        healthBars[entity] = healthBar;
    }

    public void RemoveHealthBar(GameObject entity)
    {
        if (entity == null || !healthBars.ContainsKey(entity)) return;

        Destroy(healthBars[entity].gameObject);
        healthBars.Remove(entity);
    }

    private void UpdateHealthBarPosition(GameObject entity, HealthBar healthBar)
    {
        if (entity == null) return;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(entity.transform.position + Vector3.up * 2); // 偏移位置
        if (screenPos.z > 0)
        {
            healthBar.SetPosition(screenPos);
        }
        else
        {
            healthBar.Hide();
        }
    }
}
**/