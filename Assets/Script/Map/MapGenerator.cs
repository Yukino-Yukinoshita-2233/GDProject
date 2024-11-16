using System.Collections;
using UnityEngine;
using UnityEngineInternal;
using MapManagernamespace;

public class MapGenerator : MonoBehaviour
{
    // 地图尺寸
    public int widthGen;
    public int heightGen;
    public int scaleGen;
    int[,] gridMapGen = null;   //网格地图

    // 基础地形
    public GameObject waterPrefab;  //水地形
    public GameObject grassPrefab;  //草地形
    public GameObject mountainPrefab;  //山地形
    public float waterTerrainSize = 0.3f;   //水地形占位百分比
    public float mountainTerrainSize = 0.6f;   //山地形占位百分比

    //资源地形
    public GameObject[] resourcePrefabs; // 资源预制体数组
    public int[] resourceScale; // 资源大小
    public float resourcesTerrainSize = 0.3f;   //资源地形占位百分比

    private void Start()
    {
        widthGen = MapManager.width;
        heightGen = MapManager.height;
        scaleGen = MapManager.scale;

        OnMapGenerated();
    }


    private void LateUpdate()
    {
        if (MapManager.isMapChange)
        {
            Debug.Log("地图数据被修改,更新地图实例");
            widthGen = MapManager.width;
            heightGen = MapManager.height;
            scaleGen = MapManager.scale;
            gridMapGen = MapManager.gridMap;
            OnMapGenerated();
            MapManager.isMapChange = false;
        }
    }
    public void OnMapGenerated()
    {
        //Debug.Log("OnMapGenerated:地图生成完成，继续执行后续操作");
        // 获取地图数据
        //Print2DArray(MapManager.gridMap);//debug地图数据
        gridMapGen = MapManager.gridMap;
        //Debug.Log("获取地图数据成功");
        // 实例化地图
        InstantiateMap();
        Debug.Log("实例化地图成功");
    }

    // 随机资源生成
    /*
    void GenerateResources(int resourceIndex)
    {
        // 生成噪声偏移值
        Vector2 offset = new Vector2(UnityEngine.Random.Range(0f, 1000f), UnityEngine.Random.Range(0f, 1000f));

        for (int x = 0; x < widthGen; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridNode node = mapGrid[x, y];

                if (node.IsWalkable) // 确保资源只生成在可行走区域
                {
                    float xCoord = ((float)x + offset.x) / widthGen * resourceScale[resourceIndex];
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
    */

    //实例化地图
    void InstantiateMap()
    {
        DeleteChildren();//删除当前地图

        // 首先检查是否有名为 "BaseGridMap" 的子物体
        Transform baseGridMap = transform.Find("BaseGridMap");

        // 如果没有，创建一个新的 GameObject 作为子物体
        //if (baseGridMap == null)
        //{
        baseGridMap = new GameObject("BaseGridMap").transform;
        baseGridMap.SetParent(transform);  // 设置为当前物体的子物体
        baseGridMap.localPosition = Vector3.zero;  // 设置相对位置为零
        Debug.Log("InstantiateMap:创建baseGridMap子物体成功");
        //}

        Vector3 WorldPosition;
        float H = 0.5f;
        // 遍历节点网格并实例化所有对象
        for (int x = 0; x < widthGen; x++)
        {
            for (int y = 0; y < heightGen; y++)
            {

                if (gridMapGen[x, y] == -1) //生成水
                {
                    WorldPosition = new Vector3(x, 0 - H, y);
                    GameObject instance = Instantiate(waterPrefab, WorldPosition, Quaternion.identity, baseGridMap);
                    //instance.transform.parent = transform; // 设置为当前脚本所在游戏对象的子对象
                }
                else if (gridMapGen[x, y] == 0) //生成草
                {
                    WorldPosition = new Vector3(x, 0, y);
                    GameObject instance = Instantiate(grassPrefab, WorldPosition, Quaternion.identity, baseGridMap);
                    //instance.transform.parent = transform; // 设置为当前脚本所在游戏对象的子对象
                }
                else if (gridMapGen[x, y] == 1)    //生成山
                {
                    WorldPosition = new Vector3(x, 0 + 2 * H, y);
                    GameObject instance = Instantiate(mountainPrefab, WorldPosition, Quaternion.identity, baseGridMap);
                    //instance.transform.parent = transform; // 设置为当前脚本所在游戏对象的子对象

                }
            }
        }
    }

    //删除当前地图
    void DeleteChildren()
    {
        // 首先检查是否有名为 "BaseGridMap" 的子物体
        Transform baseGridMap = GameObject.Find("Terrain").transform.Find("BaseGridMap");

        if (baseGridMap != null)
        {
            //if (Application.isPlaying)
            //{
            //    Destroy(baseGridMap.gameObject);
            //    Debug.Log("DeleteChildren:删除当前地图成功");

            //}
            //else
            //{
            //    DestroyImmediate(baseGridMap.gameObject);
            //    Debug.Log("DeleteChildren:删除当前地图成功");

            //}
            if (Application.isPlaying)
            {
                for (int i = transform.childCount; i > 0; i--)
                {
                    Destroy(transform.GetChild(0).gameObject);
                    Debug.Log("Destroy succeed");
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
