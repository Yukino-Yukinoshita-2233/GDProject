using System.Collections;
using UnityEngine;
using UnityEngineInternal;
using MapManagernamespace;

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
    public GameObject grassPrefab;  //�ݵ���
    public GameObject mountainPrefab;  //ɽ����
    public float waterTerrainSize = 0.3f;   //ˮ����ռλ�ٷֱ�
    public float mountainTerrainSize = 0.6f;   //ɽ����ռλ�ٷֱ�
    LayerMask GrassLayerMask;
    LayerMask WaterLayerMask;
    LayerMask MountainLayerMask;

    public GameObject Castle;   //�Ǳ�Ԥ����

    //��Դ����
    public GameObject[] resourcePrefabs; // ��ԴԤ��������
    public int[] resourceScale; // ��Դ��С
    public float resourcesTerrainSize = 0.3f;   //��Դ����ռλ�ٷֱ�

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
        Debug.Log("ʵ������ͼ�ɹ�");
    }

    //�Ǳ�����
    void CreateCastle()
    {
        while (true)
        {
            int X = Random.Range(1, widthGen-1);
            int Z = Random.Range(1, heightGen-1);
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

                if (gridMapGen[x,y] == 0) // ȷ����Դֻ�����ڿ���������
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
        DeleteChildren();//ɾ����ǰ��ͼ

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
                    WorldPosition = new Vector3(x, 0 - H, y);
                    GameObject instance = Instantiate(waterPrefab, WorldPosition, Quaternion.identity, baseGridMap);
                }
                else if (gridMapGen[x, y] == 0) //���ɲ�
                {
                    WorldPosition = new Vector3(x, 0, y);
                    GameObject instance = Instantiate(grassPrefab, WorldPosition, Quaternion.identity, baseGridMap);
                    //instance.layer = GrassLayerMask;

                }
                else if (gridMapGen[x, y] == 1)    //����ɽ
                {
                    WorldPosition = new Vector3(x, 0 + 2 * H, y);
                    GameObject instance = Instantiate(mountainPrefab, WorldPosition, Quaternion.identity, baseGridMap);

                }
            }
        }
    }

    //ɾ����ǰ��ͼ
    void DeleteChildren()
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
