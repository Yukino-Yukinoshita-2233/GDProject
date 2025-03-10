using System;
using System.Collections;
using UnityEngine;


namespace MapManagernamespace
{
    public class MapManager : MonoBehaviour
    {
        // 单例实例
        private static MapManager instance;
        MapGenerator mapGenerator;
        // 地图尺寸
        
        public static int width = 192;//73
        public static int height = 108;//45
        public static int scale = 12;
        public static float[,] noiseMap = new float[width, height];
        public static int[,] gridMap = new int[width, height];

        // 基础地形
        public GameObject waterPrefab;  //水地形
        public GameObject[] grassPrefab;  //草地形
        public GameObject mountainPrefab;  //山地形
        public float waterTerrainSize = 0.3f;   //水地形占位百分比
        public float mountainTerrainSize = 0.6f;   //山地形占位百分比

        //资源地形
        public GameObject[] resourcePrefabs; // 资源预制体数组
        public int[] resourceScale; // 资源大小
        public float resourcesTerrainSize = 0.3f;   //资源地形占位百分比

        public static bool isMapChange = false;
        [ContextMenu("TestMap")]
        public void TestMap()
        {
            // 生成地图
            Debug.Log("TestMap:生成地图数组");
            GenerateMap(width, height, scale, waterTerrainSize, mountainTerrainSize);
            mapGenerator = GameObject.Find("Terrain").GetComponent<MapGenerator>();
            //mapGenerator.OnMapGenerated();
        }

        void Awake()
        {
            // 检查是否已有实例
            if (instance == null)
            {
                instance = this;
                // 防止跨场景时重复
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                // 如果已有实例存在，销毁这个新的
                Destroy(gameObject);
            }
            instance = new MapManager(width, height);
            Debug.Log("MapGenerator Start:生成地图数组");
            GenerateMap(width, height, scale, waterTerrainSize, mountainTerrainSize);
            mapGenerator = GameObject.Find("Terrain").GetComponent<MapGenerator>();
            //mapGenerator.OnMapGenerated();

        }
        // 单例模式，确保 MapManager 只存在一个实例
        public static MapManager Instance
        {
            get
            {
                if (instance == null)
                {
                    //instance = new MapManager(192, 108);
                }
                return instance;
            }
        }
        // 私有构造函数，防止外部实例化
        private MapManager(int widthT, int heightT)
        {
            width = widthT;
            height = heightT;
            // 初始化噪声地图和网格地图
            noiseMap = new float[width, height];
            gridMap = new int[width, height];
            Debug.Log("MapManager初始化");
            Debug.Log(noiseMap.Length);
        }
        // 生成地图并保存数据
        public void GenerateMap(int widthT, int heightT, int scaleT, float waterTerrainSizeT, float mountainTerrainSizeT)
        {

            Debug.Log("GenerateMap:开始生成地图");
            noiseMap = GenerateNoiseMap(noiseMap, widthT, heightT, scaleT);
            gridMap = GenerategridMap(noiseMap, gridMap, waterTerrainSizeT, mountainTerrainSizeT);
            isMapChange = true;
            Debug.Log("GenerateMap:地图生成完成");
        }

        // 生成噪声地图
        private float[,] GenerateNoiseMap(float[,] noiseMapT, int width, int height, float scale)
        {
            Debug.Log("GenerateNoiseMap:正在生成噪声地图");

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
            Debug.Log("GenerategridMap:正在生成网格地图");
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
                    if (sample < waterTerrainSizeT)//生成障碍物 水节点
                    {
                        gridMapT[x, y] = 3;
                    }
                    else if (sample > mountainTerrainSizeT)//生成障碍物 山节点
                    {
                        gridMapT[x, y] = 1;
                    }
                    else
                    {
                        gridMapT[x, y] = 0;  //生成可行走 草节点
                    }
                }
            }
            return gridMapT;
        }

        // 获取地图数据
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
}

public class PrintDebug : MonoBehaviour 
{
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


