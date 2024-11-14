using System;
using System.Collections;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    // ����ʵ��
    private static MapManager instance;
    // ��ͼ�ߴ�
    public int width;
    public int height;
    public int scale;
    public float[,] noiseMap;
    public static int[,] gridMap;


    // ����ģʽ��ȷ�� MapManager ֻ����һ��ʵ��
    public static MapManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new MapManager(192,108);
            }
            return instance;
        }
    }
    // ˽�й��캯������ֹ�ⲿʵ����
    private MapManager(int widthT, int heightT)
    {
        this.width = widthT;
        this.height = heightT;
        // ��ʼ��������ͼ�������ͼ
        noiseMap = new float[this.width, this.height];
        gridMap = new int[this.width, this.height];
    }
    // ���ɵ�ͼ����������
    public void GenerateMap(int widthT, int heightT, int scaleT, float waterTerrainSizeT, float mountainTerrainSizeT)
    {

        Debug.Log("GenerateMap:��ʼ���ɵ�ͼ");
        noiseMap = GenerateNoiseMap(noiseMap, widthT, heightT, scaleT);
        gridMap = GenerategridMap(noiseMap, gridMap, waterTerrainSizeT, mountainTerrainSizeT);
        Debug.Log("GenerateMap:��ͼ�������");
    }

    // ����������ͼ
    private float[,] GenerateNoiseMap(float[,] noiseMapT, int width, int height, float scale)
    {
        Debug.Log("GenerateNoiseMap:��������������ͼ");

        if (noiseMapT == null)
        {
            Debug.Log("GenerateNoiseMap noiseMap has not been generated yet!");
        }

        // ��������ƫ��ֵ
        Vector2 offset = new Vector2(UnityEngine.Random.Range(0f, 1000f), UnityEngine.Random.Range(0f, 1000f));
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float sampleX = (x + offset.x) / scale;
                float sampleZ = (y + offset.y) / scale;
                float noiseValue = Mathf.PerlinNoise(sampleX, sampleZ);//�����㷨
                noiseMapT[x, y] = noiseValue;
            }
        }
        return noiseMapT;
    }

    // �������β����浽�ڵ�����grid
    private int[,] GenerategridMap(float[,] noiseMapT, int[,] gridMapT, float waterTerrainSizeT, float mountainTerrainSizeT)
    {
        Debug.Log("GenerategridMap:�������������ͼ");
        if (noiseMapT == null)
        {
            Debug.Log("GenerategridMap noiseMap has not been generated yet!");
        }
        //�����µĵ�ͼ
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float sample = noiseMapT[x, y];

                // ��������ֵѡ�����
                if (sample < waterTerrainSizeT)//����ˮ�ڵ�
                {
                    gridMapT[x, y] = -1;
                }
                else if (sample > mountainTerrainSizeT)//����ɽ�ڵ�
                {
                    gridMapT[x, y] = 1;
                }
                else
                {
                    gridMapT[x, y] = 0;//���ɲݽڵ�
                }
            }
        }
        return gridMapT;
    }

    // ��ȡ��ͼ����
    public int[,] GetMap()
    {
        if (gridMap == null)
        {
            Debug.Log("GetMap gridMap has not been generated yet!");
            return null;
        }
        return gridMap;
    }
}





