using UnityEngine;

/// <summary>
/// ResourceManager 类，管理游戏中的资源。
/// 继承自 Singleton<ResourceManager> 以确保只有一个实例。
/// </summary>
public class ResourceManager : Singleton<ResourceManager>
{
    // 资源数量
    private int food;
    private int wood;
    private int stone;
    private int metal;

    /// <summary>
    /// 增加资源数量。
    /// </summary>
    /// <param name="type">资源类型。</param>
    /// <param name="amount">增加的数量。</param>
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

        // 更新资源显示
        UIManager.Instance.UpdateResourceDisplay(food, wood, stone, metal);
    }

    /// <summary>
    /// 获取资源数量。
    /// </summary>
    /// <param name="type">资源类型。</param>
    /// <returns>当前资源数量。</returns>
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
/// ResourceType 枚举，表示不同类型的资源。
/// </summary>
public enum ResourceType
{
    Food,   // 食材
    Wood,   // 木材
    Stone,  // 石材
    Metal   // 金属
}
