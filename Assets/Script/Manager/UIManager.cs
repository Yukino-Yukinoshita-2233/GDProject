using UnityEngine;

/// <summary>
/// UIManager �࣬������Ϸ���û����档
/// �̳��� Singleton<UIManager> ��ȷ��ֻ��һ��ʵ����
/// </summary>
public class UIManager : Singleton<UIManager>
{
    /// <summary>
    /// ������Դ��ʾ���档
    /// </summary>
    /// <param name="food">��ǰʳ��������</param>
    /// <param name="wood">��ǰľ��������</param>
    /// <param name="stone">��ǰʯ��������</param>
    /// <param name="metal">��ǰ����������</param>
    public void UpdateResourceDisplay(int food, int wood, int stone, int metal)
    {
        // ���½�����ʾ����Դ����
        Debug.Log($"������Դ��ʾ: ʳ��={food}, ľ��={wood}, ʯ��={stone}, ����={metal}");
    }

    /// <summary>
    /// ��ʾ��λ��Ϣ��
    /// </summary>
    /// <param name="unit">Ҫ��ʾ��Ϣ�ĵ�λ��</param>
    public void ShowUnitInfo(Unit unit)
    {
        // ��ʾָ����λ����Ϣ
        Debug.Log($"��ʾ��λ��Ϣ: ����={unit.Name}, Ѫ��={unit.Health}, ������={unit.Attack}");
    }
}
