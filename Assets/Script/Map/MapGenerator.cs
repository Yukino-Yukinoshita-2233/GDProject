using System.Collections;
using UnityEngine;


public class MapGenerator : MonoBehaviour
{
    // ��ͼ�ߴ�
    public int width = 30;
    public int height = 20;
    public int scale = 20;
    int[,] gridMap = null;   //�����ͼ

    // ��������
    public GameObject grassPrefab;  //�ݵ���
    public GameObject waterPrefab;  //ˮ����
    public float waterTerrainSize = 0.3f;   //ˮ����ռλ�ٷֱ�
    public float mountainTerrainSize = 0.3f;   //ɽ����ռλ�ٷֱ�

    public GameObject[] resourcePrefabs; // ��ԴԤ��������
    public int[] resourceScale; // ��Դ��С
    public float resourcesTerrainSize = 0.3f;   //��Դ����ռλ�ٷֱ�

    private void Start()
    {
        // ����������ɵ��¼�
        //MapManager.Instance.OnMapGenerated += OnMapGenerated;
        //Debug.Log("����������ɵ��¼�");
        // ���ɵ�ͼ
        Debug.Log("MapGenerator Start");
        MapManager.Instance.GenerateMap(width, height, scale, waterTerrainSize, mountainTerrainSize);
        //StartCoroutine(OnMapGenerated());
        Debug.Log("MapGenerator Start:���ɵ�ͼ");
        OnMapGenerated();

    }

    [ContextMenu("TestMap")]
    public void TestMap()
    {
        Debug.Log("TestMap");
        // ����������ɵ��¼�
        //MapManager.Instance.OnMapGenerated += OnMapGenerated;
        // ���ɵ�ͼ
        MapManager.Instance.GenerateMap(width, height, scale, waterTerrainSize, mountainTerrainSize);
        //StartCoroutine(OnMapGenerated());
        OnMapGenerated();
    }
    public void OnMapGenerated()
    {
        Debug.Log("OnMapGenerated:��ͼ������ɣ�����ִ�к�������");
        // �ȴ���ͼ�������
        //yield return new WaitUntil(() => gridMap != null && gridMap.Length > 0);

        // ��ȡ��ͼ����
        gridMap = MapManager.Instance.GetMap();
        Print2DArray(gridMap);
        Print2DArray(MapManager.gridMap);

        Debug.Log("��ȡ��ͼ���ݳɹ�");

        if (gridMap != null)
        {
            Debug.Log("Map width: " + gridMap.GetLength(0));
            Debug.Log("Map height: " + gridMap.GetLength(1));
        }

        // ʵ������ͼ
        //InstantiateMap();
    }

    // ��ȡ��ͼ����
    public int[,] SetMap(int[,] gridMapT)
    {
        Debug.Log("SetMap����˵�ͼ����");
        if (gridMapT == null)
        {
            Debug.Log("GetMap gridMap has not been generated yet!");
            return null;
        }
        gridMap = gridMapT;
        return gridMap;
    }

    /*
    void GenerateResources(int resourceIndex)
    {
        // ��������ƫ��ֵ
        Vector2 offset = new Vector2(UnityEngine.Random.Range(0f, 1000f), UnityEngine.Random.Range(0f, 1000f));

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridNode node = mapGrid[x, y];

                if (node.IsWalkable) // ȷ����Դֻ�����ڿ���������
                {
                    float xCoord = ((float)x + offset.x) / width * resourceScale[resourceIndex];
                    float yCoord = ((float)y + offset.y) / height * resourceScale[resourceIndex];
                    float sample = Mathf.PerlinNoise(xCoord, yCoord);

                    if (sample > resourcesTerrainSize) // �趨��ֵ����ʾ����resourcesTerrainSize�ĵ�������Դ
                    {
                        Vector3 position = new Vector3(x, 0, y);
                        // ����Դ��Ϣ���浽�ڵ�����
                        mapGrid[x, y] = new GridNode(resourcePrefabs[resourceIndex], position, true, x, y);
                    }
                }
            }
        }
    }
    */

    //ʵ������ͼ
    void InstantiateMap()
    {
        DeleteChildren();

        Vector3 WorldPosition;
        float H = 0.5f;
        // �����ڵ�����ʵ�������ж���
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                if (gridMap[x,y] == 0) //���ɲ�
                {
                    WorldPosition = new Vector3(x, 0 - H, y);
                    GameObject instance = Instantiate(waterPrefab, WorldPosition, Quaternion.identity);
                    instance.transform.parent = transform; // ����Ϊ��ǰ�ű�������Ϸ������Ӷ���
                }
                else if (gridMap[x,y] == -1) //����ˮ
                {
                    WorldPosition = new Vector3(x, 0, y);
                    GameObject instance = Instantiate(grassPrefab, WorldPosition, Quaternion.identity);
                    instance.transform.parent = transform; // ����Ϊ��ǰ�ű�������Ϸ������Ӷ���
                }
                else if (gridMap[x, y] == 1)    //����ɽ
                {
                    WorldPosition = new Vector3(x, 0+H, y);
                    GameObject instance = Instantiate(grassPrefab, WorldPosition, Quaternion.identity);
                    instance.transform.parent = transform; // ����Ϊ��ǰ�ű�������Ϸ������Ӷ���

                }
            }
        }
    }

    //ɾ����ǰ��ͼ
    void DeleteChildren()
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
