using System.Collections.Generic;
using UnityEngine;

public class HealthBarManager : MonoBehaviour
{
    public static HealthBarManager Instance;

    public GameObject healthBarPrefab; // 血条的预制件
    public Transform canvasTransform; // UI画布的Transform

    // 用于保存每个单位及其对应的血条控制器
    private Dictionary<GameObject, HealthBarController> healthBars = new Dictionary<GameObject, HealthBarController>();

    void Awake()
    {
        // 单例模式，确保只有一个实例
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 创建血条
    /// </summary>
    public void CreateHealthBar(GameObject target)
    {
        if (healthBars.ContainsKey(target))
            return;

        GameObject healthBarObject = Instantiate(healthBarPrefab, canvasTransform);
        HealthBarController controller = healthBarObject.GetComponent<HealthBarController>();

        if (controller != null)
        {
            controller.SetTarget(target);
            healthBars.Add(target, controller);
        }
    }

    /// <summary>
    /// 更新血条
    /// </summary>
    public void UpdateHealthBar(GameObject target, float healthRatio)
    {
        if (healthBars.ContainsKey(target))
        {
            healthBars[target].UpdateHealth(healthRatio);
        }
    }

    /// <summary>
    /// 移除血条
    /// </summary>
    public void RemoveHealthBar(GameObject target)
    {
        if (healthBars.ContainsKey(target))
        {
            Destroy(healthBars[target].gameObject); // 销毁血条UI
            healthBars.Remove(target); // 从字典中移除
        }
    }
}
