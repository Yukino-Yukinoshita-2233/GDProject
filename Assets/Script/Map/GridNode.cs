using UnityEngine;

public class GridNode
{
    // ��������λ��
    public Vector3 WorldPosition { get; private set; }

    // �Ƿ����ͨ��
    public bool IsWalkable { get; set; }

    // �ڵ��������е�X����
    public int GridX { get; private set; }

    // �ڵ��������е�Z����
    public int GridZ { get; private set; }

    // ���ڵ㣬����·������
    public GridNode Parent { get; set; }

    // ����㵽��ǰ�ڵ���ƶ��ɱ�
    public int GCost { get; set; }

    // ��ǰ�ڵ㵽�յ�Ĺ��Ƴɱ�
    public int HCost { get; set; }

    // �ܳɱ���GCost + HCost
    public int FCost => GCost + HCost;

    // ���캯������ʼ���ڵ�
    public GridNode(Vector3 worldPosition, bool isWalkable, int gridX, int gridZ)
    {
        WorldPosition = worldPosition;
        IsWalkable = isWalkable;
        GridX = gridX;
        GridZ = gridZ;
    }
}
