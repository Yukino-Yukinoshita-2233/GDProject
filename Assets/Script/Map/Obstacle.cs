using UnityEngine;

/// <summary>
/// Obstacle �࣬��ʾ��ͼ�ϵ��ϰ��
/// </summary>
public class Obstacle
{
    // �ϰ��������
    public string Name { get; private set; }

    // �ϰ����λ��
    public Vector2 Position { get; private set; }

    /// <summary>
    /// Obstacle ��Ĺ��캯����
    /// </summary>
    /// <param name="name">�ϰ������ơ�</param>
    /// <param name="position">�ϰ���λ�á�</param>
    public Obstacle(string name, Vector2 position)
    {
        Name = name;
        Position = position;
    }
}
