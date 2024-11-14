using System;
using System.Collections;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    MapGenerator _mapGenerator;
    // ����ʵ��
    private static GameManager _instance;
    // ��ͼ�ߴ�
    public int width;
    public int height;
    public int scale;
    public float[,] noiseMap;
    public static int[,] gridMap;
    // ��������
    public GameObject grassPrefab;  //�ݵ���
    public GameObject waterPrefab;  //ˮ����
    public float waterTerrainSize;   //ˮ����ռλ�ٷֱ�
    public float mountainTerrainSize;   //ɽ����ռλ�ٷֱ�

    public GameObject LoadgameObject;
    private static MapManager instance; 

    // ����ģʽ��ȷ�� MapManager ֻ����һ��ʵ��
    public static MapManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.Log("MapManager Instance");

                instance = new MapManager(192,108);
            }
            return instance;
        }
    }
    // ˽�й��캯������ֹ�ⲿʵ����
    private MapManager(int widthT, int heightT)
    {
        Debug.Log("MapManager");
        this.width = widthT;
        this.height = heightT;

        // ��ʼ��������ͼ�������ͼ
        noiseMap = new float[this.width, this.height];
        gridMap = new int[this.width, this.height];

        //
        LoadgameObject = GameObject.Find("Terrain");
        // �� Resources �ļ��м���Ԥ����
        grassPrefab = Resources.Load<GameObject>("Prefab/GroundCube/GrassCube");
        waterPrefab = Resources.Load<GameObject>("Prefab/GroundCube/WaterCube");
        //Debug.Log("waterPrefab.name.mm" + waterPrefab.name);
        //Debug.Log("grassPrefab.name" + grassPrefab.name);

        // ������Ը�����Ҫ��ʼ������
        //for (int x = 0; x < this.width; x++)
        //{
        //    for (int y = 0; y < this.height; y++)
        //    {
        //        noiseMap[x, y] = Mathf.PerlinNoise(x * 0.1f, y * 0.1f);
        //        gridMap[x, y] = 0;
        //    }
        //}
    }

    //private void Start()
    //{

    //    // ��ʼ��������ͼ�������ͼ
    //    noiseMap = new float[this.width, this.height];
    //    gridMap = new int[this.width, this.height];

    //    //GenerateMap(width, height, scale, waterTerrainSize, mountainTerrainSize);
    //}

    [ContextMenu("TestMap")]
    void TestMap()
    {
        Debug.Log("TestMap");
        GenerateMap(width, height, scale, waterTerrainSize, mountainTerrainSize);

    }
    // ���ɵ�ͼ����������
    public void GenerateMap(int widthT, int heightT, int scaleT, float waterTerrainSizeT, float mountainTerrainSizeT)
    {
        //noiseMap = new float[widthT, heightT];
        //gridMap = new int[widthT, heightT];

        Debug.Log("GenerateMap:��ʼ���ɵ�ͼ");
        //Debug.Log(widthT + "," + heightT + "," + scaleT + "," + waterTerrainSizeT + "," + mountainTerrainSizeT);
        //Debug.Log("waterPrefab.name.GenMap" + waterPrefab.name);
        //Debug.Log("grassPrefab.name" + grassPrefab.name);

        noiseMap = GenerateNoiseMap(noiseMap, widthT, heightT, scaleT);
        gridMap = GenerategridMap(noiseMap, gridMap, waterTerrainSizeT, mountainTerrainSizeT);

        InstantiateMap();
        Debug.Log("GenerateMap:��ͼ�������");
        //_mapGenerator.SetMap(gridMap);
        //SendMessage("OnMapGenerated", null);
    }

    // ����������ͼ
    private float[,] GenerateNoiseMap(float[,] noiseMapT, int width, int height, float scale)
    {
        Debug.Log("GenerateNoiseMap");

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
        Debug.Log("GenerategridMap");

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
                if (sample < waterTerrainSizeT)
                {
                    gridMapT[x, y] = -1;
                }
                else if (sample > mountainTerrainSizeT)
                {
                    gridMapT[x, y] = 1;
                }
                else
                {
                    gridMapT[x, y] = 0;
                }
            }
        }
        return gridMapT;
    }

    //ʵ������ͼ
    void InstantiateMap()
    {
        Debug.Log("InstantiateMap");
        //Debug.Log("waterPrefab.name"+waterPrefab.name);
        //Debug.Log("grassPrefab.name"+grassPrefab.name);
        //StartCoroutine(DeleteChildren());
        DeleteChildren();
        Vector3 WorldPosition;
        float H = 0.5f;
        // �����ڵ�����ʵ�������ж���
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                if (gridMap[x, y] == 0) //���ɲ�
                {
                    //Debug.Log("grassPrefab.name" + grassPrefab.name);

                    WorldPosition = new Vector3(x, 0 , y);
                    GameObject instance = Instantiate(grassPrefab, WorldPosition, Quaternion.identity);
                    instance.transform.parent = LoadgameObject.transform; // ����Ϊ��ǰ�ű�������Ϸ������Ӷ���
                }
                else if (gridMap[x, y] == -1) //����ˮ
                {
                    //Debug.Log("waterPrefab.name" + waterPrefab.name);

                    WorldPosition = new Vector3(x, 0 - H, y);
                    GameObject instance = Instantiate(waterPrefab, WorldPosition, Quaternion.identity);
                    instance.transform.parent = LoadgameObject.transform; // ����Ϊ��ǰ�ű�������Ϸ������Ӷ���
                }
                else if (gridMap[x, y] == 1)    //����ɽ
                {
                    //Debug.Log("grassPrefab.name" + grassPrefab.name);

                    WorldPosition = new Vector3(x, 0 + H, y);
                    GameObject instance = Instantiate(grassPrefab, WorldPosition, Quaternion.identity);
                    instance.transform.parent = LoadgameObject.transform; // ����Ϊ��ǰ�ű�������Ϸ������Ӷ���

                }
            }
        }
    }

    //ɾ����ǰ��ͼ
    public void DeleteChildren() //IEnumerator
    {
        Debug.Log("DeleteChildren");

        //Debug.Log(gameObjectL.name);
        //if (LoadgameObject.transform.childCount > 0)
        //{
        //    if (Application.isPlaying)
        //    {
        //        Debug.Log("DeleteChildren.isPlaying");

        //        for (int i = LoadgameObject.transform.childCount; i > 0; i--)
        //        {
        //            Destroy(LoadgameObject.transform.GetChild(0).gameObject);
        //        }
        //    }
        //    else
        //    {
        //        Debug.Log("DeleteChildren.noPlaying");

        //        for (int i = LoadgameObject.transform.childCount; i > 0; i--)
        //        {
        //            DestroyImmediate(LoadgameObject.transform.GetChild(0).gameObject);
        //        }
        //    }
        //}

        if (LoadgameObject != null && LoadgameObject.transform.childCount > 0)
        {
            if (Application.isPlaying)
            {
                Debug.Log("DeleteChildren.isPlaying");

                for (int i = LoadgameObject.transform.childCount - 1; i >= 0; i--)
                {
                    Destroy(LoadgameObject.transform.GetChild(i).gameObject);
                }
            }
            else
            {
                Debug.Log("DeleteChildren.noPlaying");

                for (int i = LoadgameObject.transform.childCount - 1; i >= 0; i--)
                {
                    DestroyImmediate(LoadgameObject.transform.GetChild(i).gameObject);
                }
            }
        }
        //yield return null;
    }






    // ��ȡ��ͼ����
    public int[,] GetMap()
    {
        if (gridMap == null)
        {
            Debug.Log("GetMap gridMap has not been generated yet!");
            return null;
        }
        Print2DArray(gridMap);

        return gridMap;
    }


    //����ά�����ʽ��Ϊ�ַ�������� Debug.Log
    public void Print2DArray<T>(T[,] array)
    {
        string result = "";
        int rows = array.GetLength(0);
        int cols = array.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                result += array[i, j] + "\t"; // ʹ���Ʊ��������
            }
            result += "\n"; // ÿ�н�������
        }

        Debug.Log(result);
    }

}





