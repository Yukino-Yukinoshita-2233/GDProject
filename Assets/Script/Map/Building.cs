using UnityEngine;

/// <summary>
/// Building 类，表示游戏中的建筑。
/// </summary>
public class Building
{
    // 建筑的名称
    public string Name { get; private set; }

    // 建筑的等级
    public int Level { get; private set; }

    // 建筑的位置
    public Vector2 Position { get; private set; }

    // 建筑的最大等级
    private const int MaxLevel = 3;

    /// <summary>
    /// Building 类的构造函数。
    /// </summary>
    /// <param name="name">建筑名称。</param>
    /// <param name="position">建筑位置。</param>
    public Building(string name, Vector2 position)
    {
        Name = name;
        Position = position;
        Level = 1;
    }

    /// <summary>
    /// 升级建筑等级。
    /// </summary>
    public void Upgrade()
    {
        if (Level < MaxLevel)
        {
            Level++;
            Debug.Log($"{Name} 升级到等级 {Level}");
        }
        else
        {
            Debug.Log($"{Name} 已经是最高等级 {MaxLevel}");
        }
    }
}
