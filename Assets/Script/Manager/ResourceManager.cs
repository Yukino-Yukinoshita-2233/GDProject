using UnityEngine;

/// <summary>
/// ResourceManager �࣬������Ϸ�е���Դ��
/// �̳��� Singleton<ResourceManager> ��ȷ��ֻ��һ��ʵ����
/// </summary>
public class ResourceManager : Singleton<ResourceManager>
{
    // ��Դ����
    private int food;
    private int wood;
    private int stone;
    private int metal;

    /// <summary>
    /// ������Դ������
    /// </summary>
    /// <param name="type">��Դ���͡�</param>
    /// <param name="amount">���ӵ�������</param>
    public void AddResource(ResourceType type, int amount)
    {
        switch (type)
        {
            case ResourceType.Food:
                food += amount;
                break;
            case ResourceType.Wood:
                wood += amount;
                break;
            case ResourceType.Stone:
                stone += amount;
                break;
            case ResourceType.Metal:
                metal += amount;
                break;
        }

        // ������Դ��ʾ
        UIManager.Instance.UpdateResourceDisplay(food, wood, stone, metal);
    }

    /// <summary>
    /// ��ȡ��Դ������
    /// </summary>
    /// <param name="type">��Դ���͡�</param>
    /// <returns>��ǰ��Դ������</returns>
    public int GetResource(ResourceType type)
    {
        return type switch
        {
            ResourceType.Food => food,
            ResourceType.Wood => wood,
            ResourceType.Stone => stone,
            ResourceType.Metal => metal,
            _ => 0,
        };
    }
}

/// <summary>
/// ResourceType ö�٣���ʾ��ͬ���͵���Դ��
/// </summary>
public enum ResourceType
{
    Food,   // ʳ��
    Wood,   // ľ��
    Stone,  // ʯ��
    Metal   // ����
}
