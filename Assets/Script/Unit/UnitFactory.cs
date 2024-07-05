using System;

/// <summary>
/// UnitFactory 类，使用工厂模式创建不同类型的单位。
/// </summary>
public class UnitFactory
{
    /// <summary>
    /// 创建指定类型的单位。
    /// </summary>
    /// <param name="unitType">单位类型。</param>
    /// <returns>创建的单位实例。</returns>
    public static Unit CreateUnit(string unitType)
    {
        switch (unitType)
        {
            case "Villager":
                return new FriendlyUnit("Villager", 100, 10);
            case "Warrior":
                return new FriendlyUnit("Warrior", 200, 30);
            case "Archer":
                return new FriendlyUnit("Archer", 150, 20);
            case "Orc":
                return new EnemyUnit("Orc", 150, 25);
            case "Giant":
                return new EnemyUnit("Giant", 300, 50);
            case "Dragon":
                return new EnemyUnit("Dragon", 500, 70);
            default:
                throw new ArgumentException("未知的单位类型: " + unitType);
        }
    }
}
