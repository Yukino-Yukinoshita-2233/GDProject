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

    public GameObject[] resourcePrefabs; // 资源预制体数组
    public int[] resourceScale; // 资源大小
    public float resourcesTerrainSize = 0.3f;   //资源地形占位百分比

    // 节点网格
    private GridNode[,] mapGrid;

    // 噪声地图
    private float[,] noiseMap;

    void Start()
    {
        GenerateMap();  //生成地形
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 检测左键点击
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // 获取被点击的 GameObject
                GameObject clickedObject = hit.collider.gameObject;
                // 通过 Collider 来判断点击的是哪个方块
                for (int x = 0; x < width; x++)
                {
                    for (int z = 0; z < height; z++)
                    {
                        GridNode node = mapGrid[x, z];
                            
                        if (clickedObject.transform.position == node.WorldPosition)
                        {
                            // 输出方块信息
                            Debug.Log($"Clicked on: {node.TerrainPrefab.name}, IsWalkable: {node.IsWalkable}, Grid Position: ({node.GridX}, {node.GridZ})");
                            break;
                        }
                    }
                }
            }
        }
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

        CreateMap(); // 创建地图基础

        for (int i = 0; i < resourcePrefabs.Length; i++)
        {
            GenerateResources(i); // 生成资源
        }

        InstantiateMap(); // 实例化地图和资源
    }

    // 创建地形并保存到节点网格grid
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
                    // 保存到节点网格
                    mapGrid[x, z] = new GridNode(terrainPrefab, position, isWalkable, x, z);

                }
                else
                {
                    terrainPrefab = grassPrefab;
                    isWalkable = true; // 河流不可通行
                    // 保存到节点网格
                    mapGrid[x, z] = new GridNode(terrainPrefab, position, isWalkable, x, z);

                }

            }
        }
    }

    // 生成噪声地图
    float[,] GenerateNoiseMap(int width, int height, float scale)
    {
        // 生成噪声偏移值
        Vector2 offset = new Vector2(Random.Range(0f, 1000f), Random.Range(0f, 1000f));

        float[,] noiseMap = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float sampleX = (x + offset.x) / scale;
                float sampleZ = (y + offset.y) / scale;
                float noiseValue = Mathf.PerlinNoise(sampleX, sampleZ);
                noiseMap[x, y] = noiseValue;
            }
        }

        return noiseMap;
    }

    void GenerateResources(int resourceIndex)
    {
        // 生成噪声偏移值
        Vector2 offset = new Vector2(Random.Range(0f, 1000f), Random.Range(0f, 1000f));

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridNode node = mapGrid[x, y];

                if (node.IsWalkable) // 确保资源只生成在可行走区域
                {
                    float xCoord = ((float)x + offset.x) / width * resourceScale[resourceIndex];
                    float yCoord = ((float)y + offset.y) / height * resourceScale[resourceIndex];
                    float sample = Mathf.PerlinNoise(xCoord, yCoord);

                    if (sample > resourcesTerrainSize) // 设定阈值，表示大于resourcesTerrainSize的点生成资源
                    {
                        Vector3 position = new Vector3(x, 0, y);
                        // 将资源信息保存到节点网格
                        mapGrid[x, y] = new GridNode(resourcePrefabs[resourceIndex], position, true, x, y);
                    }
                }
            }
        }
    }

    void InstantiateMap()
    {
        // 遍历节点网格并实例化所有对象
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridNode node = mapGrid[x, z];

                if (node.TerrainPrefab != null) // 如果有预制件，实例化
                {
                    GameObject instance = Instantiate(node.TerrainPrefab, node.WorldPosition, Quaternion.identity);
                    instance.transform.parent = transform; // 设置为当前脚本所在游戏对象的子对象
                }
            }
        }
    }
}

