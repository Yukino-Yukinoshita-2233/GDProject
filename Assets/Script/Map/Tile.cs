using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tile �࣬��ʾ��ͼ�ϵ�һ����Ԫ��
/// </summary>
public class Tile : MonoBehaviour
{
    // ��Ԫ��ĵ�������
    public TerrainType Terrain { get; private set; }

    // ��Ԫ���Ƿ����ϰ���
    public bool HasObstacle { get; private set; }

    /// <summary>
    /// Tile ��Ĺ��캯����
    /// </summary>
    /// <param name="terrain">�������͡�</param>
    /// <param name="hasObstacle">�Ƿ����ϰ��</param>
    public Tile(TerrainType terrain, bool hasObstacle)
    {
        Terrain = terrain;
        HasObstacle = hasObstacle;
    }
}

/// <summary>
/// TerrainType ö�٣���ʾ��ͬ���͵ĵ��Ρ�
/// </summary>
public enum TerrainType
{
    Plain,  // ƽԭ
    River,  // ����
    Forest, // ɭ��
    Mine,   // ��
    Farm    // ũ��
}
