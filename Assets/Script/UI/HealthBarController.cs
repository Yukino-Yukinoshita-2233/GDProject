using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    public Slider healthSlider; // 血条UI组件
    private GameObject target;  // 目标单位
    private Vector3 offset = new Vector3(0, 0, 0.7f); // 血条位置偏移（目标单位头顶）

    /// <summary>
    /// 设置跟随的目标单位
    /// </summary>
    public void SetTarget(GameObject target)
    {
        this.target = target;
    }

    /// <summary>
    /// 更新血条血量显示
    /// </summary>
    public void UpdateHealth(float healthRatio)
    {
        if (healthSlider != null)
        {
            healthSlider.value = Mathf.Clamp01(healthRatio); // 确保血量比例在0到1之间
        }
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // 更新血条位置，使其跟随目标单位并保持在头顶
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(target.transform.position + offset);
        transform.position = screenPosition;
    }
}
