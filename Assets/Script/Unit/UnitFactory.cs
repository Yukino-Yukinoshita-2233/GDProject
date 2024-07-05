using System;

/// <summary>
/// UnitFactory �࣬ʹ�ù���ģʽ������ͬ���͵ĵ�λ��
/// </summary>
public class UnitFactory
{
    /// <summary>
    /// ����ָ�����͵ĵ�λ��
    /// </summary>
    /// <param name="unitType">��λ���͡�</param>
    /// <returns>�����ĵ�λʵ����</returns>
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
                throw new ArgumentException("δ֪�ĵ�λ����: " + unitType);
        }
    }
}
