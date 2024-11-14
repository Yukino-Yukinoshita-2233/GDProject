using System;
using System.Collections;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    MapGenerator _mapGenerator;
    // 单例实例
    private static GameManager _instance;
    // 地图尺寸
    public int width;
    public int height;
    public int scale;
    public float[,] noiseMap;
    public static int[,] gridMap;
    // 基础地形
    public GameObject grassPrefab;  //草地形
    public GameObject waterPrefab;  //水地形
    public float waterTerrainSize;   //水地形占位百分比
    public float mountainTerrainSize;   //山地形占位百分比

    public GameObject LoadgameObject;
    private static MapManager instance; 

    // 单例模式，确保 MapManager 只存在一个实例
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
    // 私有构造函数，防止外部实例化
    private MapManager(int widthT, int heightT)
    {
        Debug.Log("MapManager");
        this.width = widthT;
        this.height = heightT;

        // 初始化噪声地图和网格地图
        noiseMap = new float[this.width, this.height];
        gridMap = new int[this.width, this.height];

        //
        LoadgameObject = GameObject.Find("Terrain");
        // 从 Resources 文件夹加载预制体
        grassPrefab = Resources.Load<GameObject>("Prefab/GroundCube/GrassCube");
        waterPrefab = Resources.Load<GameObject>("Prefab/GroundCube/WaterCube");
        //Debug.Log("waterPrefab.name.mm" + waterPrefab.name);
        //Debug.Log("grassPrefab.name" + grassPrefab.name);

        // 这里可以根据需要初始化内容
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

    //    // 初始化噪声地图和网格地图
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
    // 生成地图并保存数据
    public void GenerateMap(int widthT, int heightT, int scaleT, float waterTerrainSizeT, float mountainTerrainSizeT)
    {
        //noiseMap = new float[widthT, heightT];
        //gridMap = new int[widthT, heightT];

        Debug.Log("GenerateMap:开始生成地图");
        //Debug.Log(widthT + "," + heightT + "," + scaleT + "," + waterTerrainSizeT + "," + mountainTerrainSizeT);
        //Debug.Log("waterPrefab.name.GenMap" + waterPrefab.name);
        //Debug.Log("grassPrefab.name" + grassPrefab.name);

        noiseMap = GenerateNoiseMap(noiseMap, widthT, heightT, scaleT);
        gridMap = GenerategridMap(noiseMap, gridMap, waterTerrainSizeT, mountainTerrainSizeT);

        InstantiateMap();
        Debug.Log("GenerateMap:地图生成完成");
        //_mapGenerator.SetMap(gridMap);
        //SendMessage("OnMapGenerated", null);
    }

    // 生成噪声地图
    private float[,] GenerateNoiseMap(float[,] noiseMapT, int width, int height, float scale)
    {
        Debug.Log("GenerateNoiseMap");

        if (noiseMapT == null)
        {
            Debug.Log("GenerateNoiseMap noiseMap has not been generated yet!");
        }

        // 生成噪声偏移值
        Vector2 offset = new Vector2(UnityEngine.Random.Range(0f, 1000f), UnityEngine.Random.Range(0f, 1000f));
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float sampleX = (x + offset.x) / scale;
                float sampleZ = (y + offset.y) / scale;
                float noiseValue = Mathf.PerlinNoise(sampleX, sampleZ);//柏林算法
                noiseMapT[x, y] = noiseValue;
            }
        }
        return noiseMapT;
    }

    // 创建地形并保存到节点网格grid
    private int[,] GenerategridMap(float[,] noiseMapT, int[,] gridMapT, float waterTerrainSizeT, float mountainTerrainSizeT)
    {
        Debug.Log("GenerategridMap");

        if (noiseMapT == null)
        {
            Debug.Log("GenerategridMap noiseMap has not been generated yet!");
        }

        //生成新的地图
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float sample = noiseMapT[x, y];

                // 根据噪声值选择地形
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

    //实例化地图
    void InstantiateMap()
    {
        Debug.Log("InstantiateMap");
        //Debug.Log("waterPrefab.name"+waterPrefab.name);
        //Debug.Log("grassPrefab.name"+grassPrefab.name);
        //StartCoroutine(DeleteChildren());
        DeleteChildren();
        Vector3 WorldPosition;
        float H = 0.5f;
        // 遍历节点网格并实例化所有对象
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                if (gridMap[x, y] == 0) //生成草
                {
                    //Debug.Log("grassPrefab.name" + grassPrefab.name);

                    WorldPosition = new Vector3(x, 0 , y);
                    GameObject instance = Instantiate(grassPrefab, WorldPosition, Quaternion.identity);
                    instance.transform.parent = LoadgameObject.transform; // 设置为当前脚本所在游戏对象的子对象
                }
                else if (gridMap[x, y] == -1) //生成水
                {
                    //Debug.Log("waterPrefab.name" + waterPrefab.name);

                    WorldPosition = new Vector3(x, 0 - H, y);
                    GameObject instance = Instantiate(waterPrefab, WorldPosition, Quaternion.identity);
                    instance.transform.parent = LoadgameObject.transform; // 设置为当前脚本所在游戏对象的子对象
                }
                else if (gridMap[x, y] == 1)    //生成山
                {
                    //Debug.Log("grassPrefab.name" + grassPrefab.name);

                    WorldPosition = new Vector3(x, 0 + H, y);
                    GameObject instance = Instantiate(grassPrefab, WorldPosition, Quaternion.identity);
                    instance.transform.parent = LoadgameObject.transform; // 设置为当前脚本所在游戏对象的子对象

                }
            }
        }
    }

    //删除当前地图
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






    // 获取地图数据
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


    //将二维数组格式化为字符串输出到 Debug.Log
    public void Print2DArray<T>(T[,] array)
    {
        string result = "";
        int rows = array.GetLength(0);
        int cols = array.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                result += array[i, j] + "\t"; // 使用制表符对齐列
            }
            result += "\n"; // 每行结束后换行
        }

        Debug.Log(result);
    }

}





