using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    public Slider healthSlider; // Ѫ��UI���
    private GameObject target;  // Ŀ�굥λ
    private Vector3 offset = new Vector3(0, 0, 0.7f); // Ѫ��λ��ƫ�ƣ�Ŀ�굥λͷ����

    /// <summary>
    /// ���ø����Ŀ�굥λ
    /// </summary>
    public void SetTarget(GameObject target)
    {
        this.target = target;
    }

    /// <summary>
    /// ����Ѫ��Ѫ����ʾ
    /// </summary>
    public void UpdateHealth(float healthRatio)
    {
        if (healthSlider != null)
        {
            healthSlider.value = Mathf.Clamp01(healthRatio); // ȷ��Ѫ��������0��1֮��
        }
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // ����Ѫ��λ�ã�ʹ�����Ŀ�굥λ��������ͷ��
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(target.transform.position + offset);
        transform.position = screenPosition;
    }
}
