using System.Collections;
using UnityEngine;
using UnityEngineInternal;
using MapManagernamespace;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    // ��ͼ�ߴ�
    int widthGen;
    int heightGen;
    int scaleGen;
    int[,] gridMapGen = null;   //�����ͼ
    // ��������
    Transform baseGridMap;  //�������θ�����
    public GameObject waterPrefab;  //ˮ����
    public GameObject[] grassPrefab;  //�ݵ���
    public GameObject[] mountainPrefab;  //ɽ����
    //public float waterTerrainSize = 0.3f;   //ˮ����ռλ�ٷֱ�
    //public float mountainTerrainSize = 0.6f;   //ɽ����ռλ�ٷֱ�
    LayerMask GrassLayerMask;
    LayerMask WaterLayerMask;
    LayerMask MountainLayerMask;

    public GameObject Castle;   //�Ǳ�Ԥ����

    //��Դ����
    public GameObject[] resourcePrefabs; // ��ԴԤ��������
    public int[] resourceScale; // ��Դ��С
    public float resourcesTerrainSize = 0.3f;   //��Դ����ռλ�ٷֱ�


    // ��Դ����
    public GameObject[] itemPrefabs;  //����Ԥ��������
    public int itemCount = 5;         //���Ƶ������ɵ�����

    private void Start()
    {
        widthGen = MapManager.width;
        heightGen = MapManager.height;
        scaleGen = MapManager.scale;
        GrassLayerMask = LayerMask.GetMask("Grass");
        OnMapGenerated();
        CreateCastle();
    }


    private void LateUpdate()
    {
        if (MapManager.isMapChange)
        {
            Debug.Log("��ͼ���ݱ��޸�,���µ�ͼʵ��");
            widthGen = MapManager.width;
            heightGen = MapManager.height;
            scaleGen = MapManager.scale;
            gridMapGen = MapManager.gridMap;
            OnMapGenerated();
            MapManager.isMapChange = false;

            JPSMangaer.Instance.SetNode();
        }
    }
    public void OnMapGenerated()
    {
        //Debug.Log("OnMapGenerated:��ͼ������ɣ�����ִ�к�������");
        // ��ȡ��ͼ����
        //Print2DArray(MapManager.gridMap);//debug��ͼ����
        gridMapGen = MapManager.gridMap;
        //Debug.Log("��ȡ��ͼ���ݳɹ�");
        // ʵ������ͼ
        InstantiateMap();

        InstantiateItemsOnGrass();
        Debug.Log("ʵ������ͼ�ɹ�");
    }

    void InstantiateItemsOnGrass()
    {
        // �洢�Ѿ����ɹ����ߵ�λ�ã������ظ�����
        HashSet<Vector2Int> usedPositions = new HashSet<Vector2Int>();

        int itemsGenerated = 0;

        // ������ɵ��ߣ�ֱ���ﵽָ������
        while (itemsGenerated < itemCount)
        {
            // ���ѡ��һ��λ��
            int x = Random.Range(widthGen/3, widthGen-widthGen/3);
            int y = Random.Range(heightGen/3, heightGen-heightGen/3);

            // ȷ����ǰλ���ǲݵز���û�е���������
            if (gridMapGen[x, y] == 0 && !usedPositions.Contains(new Vector2Int(x, y)))
            {
                // ��¼��λ�������ɵ���
                usedPositions.Add(new Vector2Int(x, y));

                // ���ѡ��һ������
                GameObject itemPrefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];

                if (itemPrefab.GetComponent<ZiyuanSelf>().CaiLiaoStye == CaiLiaoStye.MuCai)
                {
                    baseGridMap = GameObject.Find("res/ľ��").transform;
                }
                else if (itemPrefab.GetComponent<ZiyuanSelf>().CaiLiaoStye == CaiLiaoStye.ShiCai)
                {
                    baseGridMap = GameObject.Find("res/ʯ��").transform;
                }
                else if (itemPrefab.GetComponent<ZiyuanSelf>().CaiLiaoStye == CaiLiaoStye.JinShu)
                {
                    baseGridMap = GameObject.Find("res/����").transform;
                }

                // ���ɵ���
                Vector3 worldPosition = new Vector3(x, 0.5f, y);
                Instantiate(itemPrefab, worldPosition, Quaternion.identity, baseGridMap);

                itemsGenerated++;
            }
        }
    }

    //�Ǳ�����
    void CreateCastle()
    {
        while (true)
        {
            int X = Random.Range(widthGen/3, widthGen*2/3);
            int Z = Random.Range(heightGen/3, heightGen*2/3);
            if (gridMapGen[X, Z] == 0)
            {
                if (gridMapGen[X - 1, Z] == 0 && gridMapGen[X + 1, Z] == 0 && gridMapGen[X, Z - 1] == 0 && gridMapGen[X, Z + 1] == 0)
                {
                    if (gridMapGen[X - 1, Z - 1] == 0 && gridMapGen[X + 1, Z + 1] == 0 && gridMapGen[X + 1, Z - 1] == 0 && gridMapGen[X - 1, Z + 1] == 0)
                    {
                        Castle.transform.position = new Vector3(X, 1, Z);
                        HealthBarManager.Instance.CreateHealthBar(Castle);
                        return;
                    }
                }

                //Castle.transform.position = new Vector3(X, 1, Z);
            }

        }
    }

    // �����Դ����
    void GenerateResources(int resourceIndex)
    {
        // ��������ƫ��ֵ
        Vector2 offset = new Vector2(UnityEngine.Random.Range(0f, 1000f), UnityEngine.Random.Range(0f, 1000f));

        for (int x = 0; x < widthGen; x++)
        {
            for (int y = 0; y < heightGen; y++)
            {

                if (gridMapGen[x, y] == 0) // ȷ����Դֻ�����ڿ���������
                {
                    float xCoord = ((float)x + offset.x) / widthGen * resourceScale[resourceIndex];
                    float yCoord = ((float)y + offset.y) / heightGen * resourceScale[resourceIndex];
                    float sample = Mathf.PerlinNoise(xCoord, yCoord);

                    if (sample > resourcesTerrainSize) // �趨��ֵ����ʾ����resourcesTerrainSize�ĵ�������Դ
                    {
                        Vector3 position = new Vector3(x, 1, y);
                        // ����Դ��Ϣ���浽�ڵ�����
                        // �ڴ�������Դ
                    }
                }
            }
        }
    }

    //ʵ������ͼ
    void InstantiateMap()
    {
        DeleteOldMap();//ɾ����ǰ��ͼ

        // ���ȼ���Ƿ�����Ϊ "BaseGridMap" ��������
        baseGridMap = transform.Find("BaseGridMap");

        // ���û�У�����һ���µ� GameObject ��Ϊ������
        if (baseGridMap == null)
        {
            baseGridMap = new GameObject("BaseGridMap").transform;
            baseGridMap.SetParent(transform);  // ����Ϊ��ǰ�����������
            baseGridMap.localPosition = Vector3.zero;  // �������λ��Ϊ��
            Debug.Log("InstantiateMap:����baseGridMap������ɹ�");
        }

        Vector3 WorldPosition;
        float H = 0.5f;
        // �����ڵ�����ʵ�������ж���
        for (int x = 0; x < widthGen; x++)
        {
            for (int y = 0; y < heightGen; y++)
            {

                if (gridMapGen[x, y] == 3) //����ˮ
                {
                    WorldPosition = new Vector3(x, 0-H/2, y);
                    GameObject instance = Instantiate(waterPrefab, WorldPosition, Quaternion.identity, baseGridMap);
                }
                else if (gridMapGen[x, y] == 0) //���ɲ�
                {
                    WorldPosition = new Vector3(x, 0, y);
                    if (x % 6 < 3)
                    {
                        if (y % 6 < 3)
                        {
                            GameObject instance = Instantiate(grassPrefab[0], WorldPosition, Quaternion.identity, baseGridMap);
                        }
                        else
                        {
                            GameObject instance = Instantiate(grassPrefab[1], WorldPosition, Quaternion.identity, baseGridMap);
                        }
                    }
                    else if (x % 6 >= 3)
                    {
                        if (y % 6 >= 3)
                        {
                                GameObject instance = Instantiate(grassPrefab[0], WorldPosition, Quaternion.identity, baseGridMap);

                        }
                        else
                        {
                                GameObject instance = Instantiate(grassPrefab[1], WorldPosition, Quaternion.identity, baseGridMap);
                        }
                        //instance.layer = GrassLayerMask;

                    }
                }
                else if (gridMapGen[x, y] == 1)    //����ɽ
                {
                    WorldPosition = new Vector3(x, 0 + H, y);
                    if (Random.Range(0, 10) < 1)
                    {
                        GameObject instance = Instantiate(mountainPrefab[1], WorldPosition, Quaternion.identity, baseGridMap);
                    }
                    else if (Random.Range(0, 10) > 8)
                    {
                        GameObject instance = Instantiate(mountainPrefab[2], WorldPosition, Quaternion.identity, baseGridMap);

                    }
                    else
                    {
                        GameObject instance = Instantiate(mountainPrefab[0], WorldPosition, Quaternion.identity, baseGridMap);
                    }
                }
            }
        }
        int backgroundMapX = 50;
        int backgroundMapY = 50;
        for (int i = -backgroundMapX; i < widthGen + backgroundMapX; i++)
        {
            for (int j = -backgroundMapY; j < heightGen + backgroundMapY; j++)
            {
                if (!(0 <= i && i < widthGen && 0 <= j && j < heightGen))
                {
                    WorldPosition = new Vector3(i, 0 - H / 2, j);
                    GameObject instance = Instantiate(waterPrefab, WorldPosition, Quaternion.identity, baseGridMap);
                }
            }

        }
    }

    //ɾ����ǰ��ͼ
    void DeleteOldMap()
    {
        // ���ȼ���Ƿ�����Ϊ "BaseGridMap" ��������
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
