using UnityEngine;

/// <summary>
/// UIManager 类，管理游戏的用户界面。
/// 继承自 Singleton<UIManager> 以确保只有一个实例。
/// </summary>
public class UIManager : Singleton<UIManager>
{
    /// <summary>
    /// 更新资源显示界面。
    /// </summary>
    /// <param name="food">当前食材数量。</param>
    /// <param name="wood">当前木材数量。</param>
    /// <param name="stone">当前石材数量。</param>
    /// <param name="metal">当前金属数量。</param>
    public void UpdateResourceDisplay(int food, int wood, int stone, int metal)
    {
        // 更新界面显示的资源数量
        Debug.Log($"更新资源显示: 食材={food}, 木材={wood}, 石材={stone}, 金属={metal}");
    }

    /// <summary>
    /// 显示单位信息。
    /// </summary>
    /// <param name="unit">要显示信息的单位。</param>
    public void ShowUnitInfo(Unit unit)
    {
        // 显示指定单位的信息
        Debug.Log($"显示单位信息: 名称={unit.Name}, 血量={unit.Health}, 攻击力={unit.Attack}");
    }
}
