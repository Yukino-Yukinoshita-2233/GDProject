using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tile 类，表示地图上的一个单元格。
/// </summary>
public class Tile : MonoBehaviour
{
    // 单元格的地形类型
    public TerrainType Terrain { get; private set; }

    // 单元格是否有障碍物
    public bool HasObstacle { get; private set; }

    /// <summary>
    /// Tile 类的构造函数。
    /// </summary>
    /// <param name="terrain">地形类型。</param>
    /// <param name="hasObstacle">是否有障碍物。</param>
    public Tile(TerrainType terrain, bool hasObstacle)
    {
        Terrain = terrain;
        HasObstacle = hasObstacle;
    }
}

/// <summary>
/// TerrainType 枚举，表示不同类型的地形。
/// </summary>
public enum TerrainType
{
    Plain,  // 平原
    River,  // 河流
    Forest, // 森林
    Mine,   // 矿场
    Farm    // 农场
}
