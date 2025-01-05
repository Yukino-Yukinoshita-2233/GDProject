using System.Collections;
using UnityEngine;
using MapManagernamespace;
using System.Collections.Generic;

public class MapGeneratorWithItems : MonoBehaviour
{
    // 地图尺寸
    int widthGen;
    int heightGen;
    int scaleGen;
    int[,] gridMapGen = null;   //网格地图

    // 基础地形
    Transform baseGridMap;  //基础地形父物体
    public GameObject waterPrefab;  //水地形
    public GameObject grassPrefab;  //草地形
    public GameObject mountainPrefab;  //山地形
    public float waterTerrainSize = 0.3f;   //水地形占位百分比
    public float mountainTerrainSize = 0.6f;   //山地形占位百分比

    // 资源道具
    public GameObject[] itemPrefabs;  //道具预制体数组
    public int itemCount = 5;         //控制道具生成的数量

    // 资源地形
    //public GameObject[] resourcePrefabs; // 资源预制体数组
    //public int[] resourceScale; // 资源大小
    //public float resourcesTerrainSize = 0.3f;   //资源地形占位百分比

    private void Start()
    {
        //widthGen = MapManager.width;
        //heightGen = MapManager.height;
        //scaleGen = MapManager.scale;
        //OnMapGenerated();
    }

    private void LateUpdate()
    {
        //Debug.Log(MapManager.isMapChange);
        if (MapManager.isMapChange)
        {
            //Debug.Log("地图数据被修改,更新地图实例");
            //widthGen = MapManager.width;
            //heightGen = MapManager.height;
            //scaleGen = MapManager.scale;
            //gridMapGen = MapManager.gridMap;
            //OnMapGenerated();
            //MapManager.isMapChange = false;
            InstantiateItemsOnGrass();
        }
    }

    public void OnMapGenerated()
    {
        // 获取地图数据
        gridMapGen = MapManager.gridMap;
        // 实例化地图
        InstantiateMap();
        Debug.Log("实例化地图成功");

        // 在草地上随机生成道具
        InstantiateItemsOnGrass();
    }

    // 实例化地图
    void InstantiateMap()
    {
        DeleteChildren();  //删除当前地图

        // 创建一个空物体作为地图的父物体
        baseGridMap = transform.Find("BaseGridMap");

        if (baseGridMap == null)
        {
            baseGridMap = new GameObject("BaseGridMap").transform;
            baseGridMap.SetParent(transform);  // 设置为当前物体的子物体
            baseGridMap.localPosition = Vector3.zero;  // 设置相对位置为零
            Debug.Log("InstantiateMap:创建baseGridMap子物体成功");
        }

        Vector3 WorldPosition;
        float H = 0.5f;

        // 遍历节点网格并实例化所有对象
        for (int x = 0; x < widthGen; x++)
        {
            for (int y = 0; y < heightGen; y++)
            {
                if (gridMapGen[x, y] == 3) //生成水
                {
                    WorldPosition = new Vector3(x, 0 - H, y);
                    GameObject instance = Instantiate(waterPrefab, WorldPosition, Quaternion.identity, baseGridMap);
                }
                else if (gridMapGen[x, y] == 0) //生成草
                {
                    WorldPosition = new Vector3(x, 0, y);
                    GameObject instance = Instantiate(grassPrefab, WorldPosition, Quaternion.identity, baseGridMap);
                }
                else if (gridMapGen[x, y] == 1) //生成山
                {
                    WorldPosition = new Vector3(x, 0 + 2 * H, y);
                    GameObject instance = Instantiate(mountainPrefab, WorldPosition, Quaternion.identity, baseGridMap);
                }
            }
        }
    }

    // 删除当前地图
    void DeleteChildren()
    {
        baseGridMap = GameObject.Find("Terrain").transform.Find("BaseGridMap");

        if (baseGridMap != null)
        {
            if (Application.isPlaying)
            {
                for (int i = transform.childCount; i > 0; i--)
                {
                    Destroy(transform.GetChild(0).gameObject);
                }
            }
            else
            {
                for (int i = transform.childCount; i > 0; i--)
                {
                    DestroyImmediate(transform.GetChild(0).gameObject);
                }
            }
        }
    }

    // 在草地上随机生成道具
    void InstantiateItemsOnGrass()
    {
        // 存储已经生成过道具的位置，避免重复生成
        HashSet<Vector2Int> usedPositions = new HashSet<Vector2Int>();

        int itemsGenerated = 0;

        // 随机生成道具，直到达到指定数量
        while (itemsGenerated < itemCount)
        {
            // 随机选择一个位置
            int x = Random.Range(0, widthGen);
            int y = Random.Range(0, heightGen);

            // 确保当前位置是草地并且没有道具已生成
            if (gridMapGen[x, y] == 0 && !usedPositions.Contains(new Vector2Int(x, y)))
            {
                Debug.Log($"尝试生成道具：位置 ({x}, {y}) 是草地且未生成过道具");
                // 记录该位置已生成道具
                usedPositions.Add(new Vector2Int(x, y));

                // 随机选择一个道具
                GameObject itemPrefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];

                if (itemPrefab.GetComponent<ZiYuan>().CaiLiaoStye == CaiLiaoStye.MuCai)
                {
                    baseGridMap = GameObject.Find("res/木材").transform;
                }
                else if (itemPrefab.GetComponent<ZiYuan>().CaiLiaoStye == CaiLiaoStye.ShiCai)
                {
                    baseGridMap = GameObject.Find("res/石材").transform;
                }
                else if (itemPrefab.GetComponent<ZiYuan>().CaiLiaoStye == CaiLiaoStye.JinShu)
                {
                    baseGridMap = GameObject.Find("res/金属").transform;
                }

                // 生成道具
                Vector3 worldPosition = new Vector3(x, 0.5f, y);
                Instantiate(itemPrefab, worldPosition, Quaternion.identity, baseGridMap);

                itemsGenerated++;
            }
            else
            {
                Debug.Log($"跳过位置 ({x}, {y})，不是草地或已生成过道具");
            }
        }
        Debug.Log($"生成道具数量: {itemsGenerated}");
    }

    // 将二维数组格式化为字符串输出到 Debug.Log
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
