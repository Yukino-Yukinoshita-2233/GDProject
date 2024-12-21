using System.Collections.Generic;
using UnityEngine;

public class HealthBarManager : MonoBehaviour
{
    public static HealthBarManager Instance;

    public GameObject healthBarPrefab; // Ѫ����Ԥ�Ƽ�
    public Transform canvasTransform; // UI������Transform

    // ���ڱ���ÿ����λ�����Ӧ��Ѫ��������
    private Dictionary<GameObject, HealthBarController> healthBars = new Dictionary<GameObject, HealthBarController>();

    void Awake()
    {
        // ����ģʽ��ȷ��ֻ��һ��ʵ��
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
    /// ����Ѫ��
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

            // ����Ŀ�굥λ��Ѫ���仯�¼�
            Health health = target.GetComponent<Health>();
            if (health != null)
            {
                health.healthChange += controller.UpdateHealth;  // ��Ѫ���仯�¼���Ѫ��UI���·�����
            }
        }
    }

    /// <summary>
    /// �Ƴ�Ѫ��
    /// </summary>
    public void RemoveHealthBar(GameObject target)
    {
        if (healthBars.ContainsKey(target))
        {
            // ȡ��Ѫ���仯�¼�����
            Health health = target.GetComponent<Health>();
            if (health != null)
            {
                health.healthChange -= healthBars[target].UpdateHealth; // ����¼�����
            }

            Destroy(healthBars[target].gameObject); // ����Ѫ��UI
            healthBars.Remove(target); // ���ֵ����Ƴ�
        }
    }
}
