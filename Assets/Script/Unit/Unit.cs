using UnityEngine;

/// <summary>
/// ������� Unit����ʾ���е�λ��
/// </summary>
public abstract class Unit
{
    // ��λ������
    public string Name { get; private set; }

    // ��λ��Ѫ��
    public int Health { get; protected set; }

    // ��λ�Ĺ�����
    public int Attack { get; protected set; }

    // ��λ��λ��
    public Vector2 Position { get; set; }

    /// <summary>
    /// Unit ��Ĺ��캯����
    /// </summary>
    /// <param name="name">��λ���ơ�</param>
    /// <param name="health">��λѪ����</param>
    /// <param name="attack">��λ��������</param>
    public Unit(string name, int health, int attack)
    {
        Name = name;
        Health = health;
        Attack = attack;
    }

    /// <summary>
    /// ���󷽷� OnMove�������ƶ���λ��
    /// </summary>
    /// <param name="targetPosition">Ŀ��λ�á�</param>
    public abstract void OnMove(Vector2 targetPosition);

    /// <summary>
    /// ���󷽷� OnAttack�����ڹ���Ŀ�굥λ��
    /// </summary>
    /// <param name="target">Ŀ�굥λ��</param>
    public abstract void OnAttack(Unit target);
}

/// <summary>
/// FriendlyUnit �࣬�̳��� Unit����ʾ�ѷ���λ��
/// </summary>
public class FriendlyUnit : Unit
{
    public FriendlyUnit(string name, int health, int attack) : base(name, health, attack)
    {
    }

    public override void OnMove(Vector2 targetPosition)
    {
        // ʵ���ѷ���λ���ƶ��߼�
        Debug.Log($"{Name} �ƶ��� {targetPosition}");
    }

    public override void OnAttack(Unit target)
    {
        // ʵ���ѷ���λ�Ĺ����߼�
        Debug.Log($"{Name} ���� {target.Name}");
    }
}

/// <summary>
/// EnemyUnit �࣬�̳��� Unit����ʾ�з���λ��
/// </summary>
public class EnemyUnit : Unit
{
    public EnemyUnit(string name, int health, int attack) : base(name, health, attack)
    {
    }

    public override void OnMove(Vector2 targetPosition)
    {
        // ʵ�ֵз���λ���ƶ��߼�
        Debug.Log($"{Name} �ƶ��� {targetPosition}");
    }

    public override void OnAttack(Unit target)
    {
        // ʵ�ֵз���λ�Ĺ����߼�
        Debug.Log($"{Name} ���� {target.Name}");
    }
}
