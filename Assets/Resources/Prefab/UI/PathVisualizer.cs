using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用于显示路径的可视化组件
/// </summary>
public class PathVisualizer : MonoBehaviour
{
    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    /// <summary>
    /// 设置路径的点
    /// </summary>
    public void SetPath(List<Vector3> pathPoints)
    {
        if (pathPoints == null || pathPoints.Count == 0)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        lineRenderer.positionCount = pathPoints.Count;
        lineRenderer.SetPositions(pathPoints.ToArray());
    }

    /// <summary>
    /// 清除路径显示
    /// </summary>
    public void ClearPath()
    {
        lineRenderer.positionCount = 0;
    }
}
