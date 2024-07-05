using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public AIController aiController;

    // 地图尺寸
    public int width = 100;
    public int height = 100;
    public float scale = 20f;

    // 地形预制件
    public GameObject grassPrefab;
    public GameObject watchPrefab;
    public GameObject forestPrefab;
    public GameObject minePrefab;
    public GameObject farmPrefab;

    //// 障碍物预制件
    //public GameObject stonePrefab;
    //public GameObject treePrefab;
    //public GameObject chestPrefab;
    //public GameObject cropPrefab;

    //// 建筑预制件
    //public GameObject baseCampPrefab;
    //public GameObject wallPrefab;
    //public GameObject towerPrefab;
    //public GameObject housePrefab;
    //public GameObject woodFactoryPrefab;
    //public GameObject stoneFactoryPrefab;
    //public GameObject metalFactoryPrefab;

    // 节点网格
    private GridNode[,] grid;

    // 提供公共访问方法来获取 grid 属性
    //public GridNode[,] GetGrid()
    //{
    //    return grid;
    //}

    // 噪声地图
    private float[,] noiseMap;

    void Start()
    {
        GenerateMap();
    }

    //void Update()
    //{
    //    GridNode[,] grid = MapGenerator.grid;
    //    if (grid == null)
    //    {
    //        Debug.LogError("Grid not initialized!");
    //        return;
    //    }

    //    AIController.PlanPath(grid);
    //}

    // 在某个方法中调用 AIController 的路径规划方法，并传递网格作为参数
    public void CallAIController()
    {
        float[,] grid = noiseMap; // 生成网格的逻辑，这里示意为 GenerateGrid()

        if (aiController != null)
        {
            aiController.PlanPath(grid); // 将网格作为参数传递给 AIController
        }
        else
        {
            Debug.LogError("AIController reference is null!");
        }
    }

    // 生成地图
    void GenerateMap()
    {
        noiseMap = GenerateNoiseMap(width, height, scale);
        grid = new GridNode[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                float sample = noiseMap[x, z];
                Vector3 position = new Vector3(x, 0, z);
                bool isWalkable = true;
                GameObject terrainPrefab = null;

                // 根据噪声值选择地形
                if (sample < 0.2f)
                {
                    terrainPrefab = watchPrefab;
                    isWalkable = false; // 河流不可通行
                }
                else if (sample < 0.4f)
                {
                    terrainPrefab = forestPrefab;
                }
                else if (sample < 0.6f)
                {
                    terrainPrefab = minePrefab;
                }
                else if (sample < 0.8f)
                {
                    terrainPrefab = farmPrefab;
                }
                else
                {
                    terrainPrefab = grassPrefab;
                }

                // 创建地形的实例
                GameObject terrainInstance = Instantiate(terrainPrefab, position, Quaternion.identity);
                // 将 terrainInstance 设置为当前脚本所在游戏对象的子对象
                terrainInstance.transform.parent = transform;
                grid[x, z] = new GridNode(position, isWalkable, x, z);
            }
        }

        //PlaceObstacles();
        //PlaceBuildings();
    }

    // 生成噪声地图
    float[,] GenerateNoiseMap(int width, int height, float scale)
    {
        float[,] noiseMap = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                float sampleX = x / scale;
                float sampleZ = z / scale;
                float noiseValue = Mathf.PerlinNoise(sampleX, sampleZ);
                noiseMap[x, z] = noiseValue;
            }
        }

        return noiseMap;
    }

    //// 随机放置障碍物
    //void PlaceObstacles()
    //{
    //    for (int i = 0; i < 50; i++)
    //    {
    //        Vector3 position = new Vector3(Random.Range(0, width), 0, Random.Range(0, height));
    //        Instantiate(stonePrefab, position, Quaternion.identity);
    //        grid[(int)position.x, (int)position.z].IsWalkable = false; // 更新网格节点为不可通行
    //    }
    //}

    //// 随机放置建筑
    //void PlaceBuildings()
    //{
    //    for (int i = 0; i < 10; i++)
    //    {
    //        Vector3 position = GetRandomPositionOnGrass();
    //        Instantiate(baseCampPrefab, position, Quaternion.identity);
    //    }
    //}

    // 在草地上获取随机位置
    Vector3 GetRandomPositionOnGrass()
    {
        while (true)
        {
            int x = Random.Range(0, width);
            int z = Random.Range(0, height);
            if (grid[x, z].IsWalkable && noiseMap[x, z] >= 0.8f)
            {
                return new Vector3(x, 0, z);
            }
        }
    }
}
