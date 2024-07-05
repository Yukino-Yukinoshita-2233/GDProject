using UnityEngine;

/// <summary>
/// Obstacle 类，表示地图上的障碍物。
/// </summary>
public class Obstacle
{
    // 障碍物的名称
    public string Name { get; private set; }

    // 障碍物的位置
    public Vector2 Position { get; private set; }

    /// <summary>
    /// Obstacle 类的构造函数。
    /// </summary>
    /// <param name="name">障碍物名称。</param>
    /// <param name="position">障碍物位置。</param>
    public Obstacle(string name, Vector2 position)
    {
        Name = name;
        Position = position;
    }
}
