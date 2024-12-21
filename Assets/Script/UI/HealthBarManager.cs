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

            // 订阅目标单位的血量变化事件
            Health health = target.GetComponent<Health>();
            if (health != null)
            {
                health.healthChange += controller.UpdateHealth;  // 将血量变化事件与血条UI更新方法绑定
            }
        }
    }

    /// <summary>
    /// 移除血条
    /// </summary>
    public void RemoveHealthBar(GameObject target)
    {
        if (healthBars.ContainsKey(target))
        {
            // 取消血量变化事件订阅
            Health health = target.GetComponent<Health>();
            if (health != null)
            {
                health.healthChange -= healthBars[target].UpdateHealth; // 解除事件订阅
            }

            Destroy(healthBars[target].gameObject); // 销毁血条UI
            healthBars.Remove(target); // 从字典中移除
        }
    }
}
