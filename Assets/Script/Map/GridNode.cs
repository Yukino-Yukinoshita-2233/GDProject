using UnityEngine;

public class GridNode
{
    // 世界坐标位置
    public Vector3 WorldPosition { get; private set; }

    // 是否可以通行
    public bool IsWalkable { get; set; }

    // 节点在网格中的X坐标
    public int GridX { get; private set; }

    // 节点在网格中的Z坐标
    public int GridZ { get; private set; }

    // 父节点，用于路径回溯
    public GridNode Parent { get; set; }

    // 从起点到当前节点的移动成本
    public int GCost { get; set; }

    // 当前节点到终点的估计成本
    public int HCost { get; set; }

    // 总成本：GCost + HCost
    public int FCost => GCost + HCost;

    // 构造函数，初始化节点
    public GridNode(Vector3 worldPosition, bool isWalkable, int gridX, int gridZ)
    {
        WorldPosition = worldPosition;
        IsWalkable = isWalkable;
        GridX = gridX;
        GridZ = gridZ;
    }
}
