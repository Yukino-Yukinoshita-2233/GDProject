using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ʾ·���Ŀ��ӻ����
/// </summary>
public class PathVisualizer : MonoBehaviour
{
    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    /// <summary>
    /// ����·���ĵ�
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
    /// ���·����ʾ
    /// </summary>
    public void ClearPath()
    {
        lineRenderer.positionCount = 0;
    }
}
