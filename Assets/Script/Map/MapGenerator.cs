using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UIElements;

public class MapGenerator : MonoBehaviour
{
    public AIController aiController;

    // 地图尺寸
    public int width = 100;
    public int height = 100;
    public float scale = 20f;

    // 基础地形预制件
    public GameObject grassPrefab;  //草地形
    public GameObject waterPrefab;  //水地形
    public float waterTerrainSize = 0.3f;   //水地形占位百分比

    //资源地形预制件
    public GameObject forestPrefab; //森林资源地形
    public GameObject minePrefab;   //矿物资源地形
    public GameObject farmPrefab;   //食物资源地形
    
    public int forestResourcePosCount = 10;    //森林资源点数量
    public int forestresourceBlockSize = 5;   //森林资源点大小
    //public int mineResourcePosCount = 10;    //矿物资源点数量
    //public int mineresourceBlockSize = 5;   //矿物资源点大小
    //public int farmResourcePosCount = 10;    //食物资源点数量
    //public int farmresourceBlockSize = 5;   //食物资源点大小


    //// 建筑预制件
    //public GameObject baseCampPrefab;
    //public GameObject wallPrefab;
    //public GameObject towerPrefab;
    //public GameObject housePrefab;
    //public GameObject woodFactoryPrefab;
    //public GameObject stoneFactoryPrefab;
    //public GameObject metalFactoryPrefab;

    // 节点网格
    private GridNode[,] mapGrid;
    private GridNode[,] ResourcesGrid;
    // 噪声地图
    private float[,] noiseMap;
    // 噪声偏移值
    private Vector2 offset;

    void Start()
    {
        GenerateMap();  //生成地形
    }

    [ContextMenu("TestMap")]
    private void TestMap()
    {
        GenerateMap();

    }

    // 生成地图数据二维数组
    void GenerateMap()
    {
        noiseMap = GenerateNoiseMap(width, height, scale);
        mapGrid = new GridNode[width, height];
        PlaceResources();

                CreateMap();

    }

    //创建地形并保存到节点网格grid
    private void CreateMap()
    {
        // 删除旧的地图
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            // 删除子物体
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        //生成新的地图
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                float sample = noiseMap[x, z];
                Vector3 position = new Vector3(x, 0, z);
                bool isWalkable = true;
                GameObject terrainPrefab = null;

                // 根据噪声值选择地形



                if (sample < waterTerrainSize)
                {
                    terrainPrefab = waterPrefab;
                    isWalkable = false; // 河流不可通行

                }
                else if(sample >= waterTerrainSize &&  sample <= 1.0f)
                {
                    terrainPrefab = grassPrefab;

                }
                else if (sample == 2.0f)
                {
                    terrainPrefab = forestPrefab;
                    isWalkable = false;
                }
                else if (sample == 3.0f)
                {
                    terrainPrefab = minePrefab;
                    isWalkable = false;
                }
                else if (sample == 4.0f)
                {
                    terrainPrefab = farmPrefab;
                    isWalkable = false;
                }



                // 创建地形的实例
                GameObject terrainInstance = Instantiate(terrainPrefab, position, Quaternion.identity);
                // 将 terrainInstance 设置为当前脚本所在游戏对象的子对象
                terrainInstance.transform.parent = transform;
                mapGrid[x, z] = new GridNode(terrainInstance, position, isWalkable, x, z);
            }
        }

    }


    // 生成噪声地图
    float[,] GenerateNoiseMap(int width, int height, float scale)
    {
        // 生成噪声偏移值
        offset = new Vector2(Random.Range(0f, 1000f), Random.Range(0f, 1000f));

        float[,] noiseMap = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                float sampleX = (x + offset.x) / scale;
                float sampleZ = (z + offset.y) / scale;
                float noiseValue = Mathf.PerlinNoise(sampleX, sampleZ);
                noiseMap[x, z] = noiseValue;
            }
        }

        return noiseMap;
    }

    // 在草地上获取随机位置
    Vector3 GetRandomPositionOnGrass()
    {
        while (true)
        {
            int x = Random.Range(10, width-10);
            int z = Random.Range(10, height-10);
            if (noiseMap[x, z] >= waterTerrainSize && noiseMap[x, z] <= 1.0f)
            {
                return new Vector3(x, 0, z);
            }
        }
    }

    // 随机放置障碍物
    void PlaceResources()
    {
        for (int i = 0; i < forestResourcePosCount; i++)
        {

            Vector3 position = GetRandomPositionOnGrass();
            int newforestresourceBlockSize = 0;
            // 遍历资源区块内的每个点
            for (int x = ((int)position.x - newforestresourceBlockSize/2); x <= (position.x + newforestresourceBlockSize/2); x++)
            {
                for (int z = ((int)position.z- newforestresourceBlockSize/2); z <= (position.z + newforestresourceBlockSize/2); z++)
                {
                    // 在噪声地图上标记资源区块（例如，将高度值增加或设置为特定值）
                    noiseMap[x, z] = Random.Range(2,5); // 将资源区块的高度设置为最大值
                }
            }
        }
    }

    //// 随机放置建筑
    //void PlaceBuildings()
    //{
    //    for (int i = 0; i < 10; i++)
    //    {
    //        Vector3 position = GetRandomPositionOnGrass();
    //        Instantiate(baseCampPrefab, position, Quaternion.identity);
    //    }
    //}

}
