using System.Collections;
using UnityEngine;
using MapManagernamespace;
using System.Collections.Generic;

public class MapGeneratorWithItems : MonoBehaviour
{
    // ��ͼ�ߴ�
    int widthGen;
    int heightGen;
    int scaleGen;
    int[,] gridMapGen = null;   //�����ͼ

    // ��������
    Transform baseGridMap;  //�������θ�����
    public GameObject waterPrefab;  //ˮ����
    public GameObject grassPrefab;  //�ݵ���
    public GameObject mountainPrefab;  //ɽ����
    public float waterTerrainSize = 0.3f;   //ˮ����ռλ�ٷֱ�
    public float mountainTerrainSize = 0.6f;   //ɽ����ռλ�ٷֱ�

    // ��Դ����
    public GameObject[] itemPrefabs;  //����Ԥ��������
    public int itemCount = 5;         //���Ƶ������ɵ�����

    // ��Դ����
    //public GameObject[] resourcePrefabs; // ��ԴԤ��������
    //public int[] resourceScale; // ��Դ��С
    //public float resourcesTerrainSize = 0.3f;   //��Դ����ռλ�ٷֱ�

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
            //Debug.Log("��ͼ���ݱ��޸�,���µ�ͼʵ��");
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
        // ��ȡ��ͼ����
        gridMapGen = MapManager.gridMap;
        // ʵ������ͼ
        InstantiateMap();
        Debug.Log("ʵ������ͼ�ɹ�");

        // �ڲݵ���������ɵ���
        InstantiateItemsOnGrass();
    }

    // ʵ������ͼ
    void InstantiateMap()
    {
        DeleteChildren();  //ɾ����ǰ��ͼ

        // ����һ����������Ϊ��ͼ�ĸ�����
        baseGridMap = transform.Find("BaseGridMap");

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
                    WorldPosition = new Vector3(x, 0 - H, y);
                    GameObject instance = Instantiate(waterPrefab, WorldPosition, Quaternion.identity, baseGridMap);
                }
                else if (gridMapGen[x, y] == 0) //���ɲ�
                {
                    WorldPosition = new Vector3(x, 0, y);
                    GameObject instance = Instantiate(grassPrefab, WorldPosition, Quaternion.identity, baseGridMap);
                }
                else if (gridMapGen[x, y] == 1) //����ɽ
                {
                    WorldPosition = new Vector3(x, 0 + 2 * H, y);
                    GameObject instance = Instantiate(mountainPrefab, WorldPosition, Quaternion.identity, baseGridMap);
                }
            }
        }
    }

    // ɾ����ǰ��ͼ
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

    // �ڲݵ���������ɵ���
    void InstantiateItemsOnGrass()
    {
        // �洢�Ѿ����ɹ����ߵ�λ�ã������ظ�����
        HashSet<Vector2Int> usedPositions = new HashSet<Vector2Int>();

        int itemsGenerated = 0;

        // ������ɵ��ߣ�ֱ���ﵽָ������
        while (itemsGenerated < itemCount)
        {
            // ���ѡ��һ��λ��
            int x = Random.Range(0, widthGen);
            int y = Random.Range(0, heightGen);

            // ȷ����ǰλ���ǲݵز���û�е���������
            if (gridMapGen[x, y] == 0 && !usedPositions.Contains(new Vector2Int(x, y)))
            {
                Debug.Log($"�������ɵ��ߣ�λ�� ({x}, {y}) �ǲݵ���δ���ɹ�����");
                // ��¼��λ�������ɵ���
                usedPositions.Add(new Vector2Int(x, y));

                // ���ѡ��һ������
                GameObject itemPrefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];

                if (itemPrefab.GetComponent<ZiYuan>().CaiLiaoStye == CaiLiaoStye.MuCai)
                {
                    baseGridMap = GameObject.Find("res/ľ��").transform;
                }
                else if (itemPrefab.GetComponent<ZiYuan>().CaiLiaoStye == CaiLiaoStye.ShiCai)
                {
                    baseGridMap = GameObject.Find("res/ʯ��").transform;
                }
                else if (itemPrefab.GetComponent<ZiYuan>().CaiLiaoStye == CaiLiaoStye.JinShu)
                {
                    baseGridMap = GameObject.Find("res/����").transform;
                }

                // ���ɵ���
                Vector3 worldPosition = new Vector3(x, 0.5f, y);
                Instantiate(itemPrefab, worldPosition, Quaternion.identity, baseGridMap);

                itemsGenerated++;
            }
            else
            {
                Debug.Log($"����λ�� ({x}, {y})�����ǲݵػ������ɹ�����");
            }
        }
        Debug.Log($"���ɵ�������: {itemsGenerated}");
    }

    // ����ά�����ʽ��Ϊ�ַ�������� Debug.Log
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
