using UnityEngine;

public class GridNode
{
    // ����
    public GameObject TerrainPrefab { get; set; }
    // ��������λ��
    public Vector3 WorldPosition { get; set; }

    // �Ƿ����ͨ��
    public bool IsWalkable { get; set; }

    // �ڵ��������е�X����
    public int GridX { get; set; }

    // �ڵ��������е�Z����
    public int GridZ { get; set; }

    // ���ڵ㣬����·������
    public GridNode Parent { get; set; }

    // ����㵽��ǰ�ڵ���ƶ��ɱ�
    public int GCost { get; set; }

    // ��ǰ�ڵ㵽�յ�Ĺ��Ƴɱ�
    public int HCost { get; set; }

    // �ܳɱ���GCost + HCost
    public int FCost => GCost + HCost;

    // ���캯������ʼ���ڵ�
    public GridNode(GameObject terrainPrefab, Vector3 worldPosition, bool isWalkable, int gridX, int gridZ)
    {
        TerrainPrefab = terrainPrefab;
        WorldPosition = worldPosition;
        IsWalkable = isWalkable;
        GridX = gridX;
        GridZ = gridZ;
    }
}
