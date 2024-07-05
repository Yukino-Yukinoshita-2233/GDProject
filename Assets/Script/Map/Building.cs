using UnityEngine;

/// <summary>
/// Building �࣬��ʾ��Ϸ�еĽ�����
/// </summary>
public class Building
{
    // ����������
    public string Name { get; private set; }

    // �����ĵȼ�
    public int Level { get; private set; }

    // ������λ��
    public Vector2 Position { get; private set; }

    // ���������ȼ�
    private const int MaxLevel = 3;

    /// <summary>
    /// Building ��Ĺ��캯����
    /// </summary>
    /// <param name="name">�������ơ�</param>
    /// <param name="position">����λ�á�</param>
    public Building(string name, Vector2 position)
    {
        Name = name;
        Position = position;
        Level = 1;
    }

    /// <summary>
    /// ���������ȼ���
    /// </summary>
    public void Upgrade()
    {
        if (Level < MaxLevel)
        {
            Level++;
            Debug.Log($"{Name} �������ȼ� {Level}");
        }
        else
        {
            Debug.Log($"{Name} �Ѿ�����ߵȼ� {MaxLevel}");
        }
    }
}
