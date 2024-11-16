using System.Collections;
using UnityEngine;
using UnityEngineInternal;
using MapManagernamespace;

public class MapGenerator : MonoBehaviour
{
    // ��ͼ�ߴ�
    public int width = 30;
    public int height = 20;
    public int scale = 20;
    int[,] gridMapTest = null;   //�����ͼ

    // ��������
    public GameObject waterPrefab;  //ˮ����
    public GameObject grassPrefab;  //�ݵ���
    public GameObject mountainPrefab;  //ɽ����
    public float waterTerrainSize = 0.3f;   //ˮ����ռλ�ٷֱ�
    public float mountainTerrainSize = 0.3f;   //ɽ����ռλ�ٷֱ�

    //��Դ����
    public GameObject[] resourcePrefabs; // ��ԴԤ��������
    public int[] resourceScale; // ��Դ��С
    public float resourcesTerrainSize = 0.3f;   //��Դ����ռλ�ٷֱ�

    private void Start()
    {
        // ���ɵ�ͼ
        Debug.Log("MapGenerator Start:���ɵ�ͼ����");
        MapManager.Instance.GenerateMap(width, height, scale, waterTerrainSize, mountainTerrainSize);
        OnMapGenerated();
    }

    [ContextMenu("TestMap")]
    public void TestMap()
    {

        // ���ɵ�ͼ
        Debug.Log("TestMap:���ɵ�ͼ����");
        MapManager.Instance.GenerateMap(width, height, scale, waterTerrainSize, mountainTerrainSize);

        OnMapGenerated();
    }

    private void LateUpdate()
    {
        if (gridMapTest != MapManager.gridMap)
        {
            Debug.Log("��ͼ���ݱ��޸�,���µ�ͼʵ��");
            gridMapTest = MapManager.gridMap;
            OnMapGenerated();
        }
    }
    public void OnMapGenerated()
    {
        //Debug.Log("OnMapGenerated:��ͼ������ɣ�����ִ�к�������");
        // ��ȡ��ͼ����
        //Print2DArray(MapManager.gridMap);//debug��ͼ����
        gridMapTest = MapManager.gridMap;
        //Debug.Log("��ȡ��ͼ���ݳɹ�");
        // ʵ������ͼ
        InstantiateMap();
        Debug.Log("ʵ������ͼ�ɹ�");
    }

    // �����Դ����
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
        DeleteChildren();//ɾ����ǰ��ͼ

        // ���ȼ���Ƿ�����Ϊ "BaseGridMap" ��������
        Transform baseGridMap = transform.Find("BaseGridMap");

        // ���û�У�����һ���µ� GameObject ��Ϊ������
        //if (baseGridMap == null)
        //{
            baseGridMap = new GameObject("BaseGridMap").transform;
            baseGridMap.SetParent(transform);  // ����Ϊ��ǰ�����������
            baseGridMap.localPosition = Vector3.zero;  // �������λ��Ϊ��
            Debug.Log("InstantiateMap:����baseGridMap������ɹ�");
        //}

        Vector3 WorldPosition;
        float H = 0.5f;
        // �����ڵ�����ʵ�������ж���
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                if (gridMapTest[x, y] == -1) //����ˮ
                {
                    WorldPosition = new Vector3(x, 0 - H, y);
                    GameObject instance = Instantiate(waterPrefab, WorldPosition, Quaternion.identity, baseGridMap);
                    //instance.transform.parent = transform; // ����Ϊ��ǰ�ű�������Ϸ������Ӷ���
                }
                else if (gridMapTest[x, y] == 0) //���ɲ�
                {
                    WorldPosition = new Vector3(x, 0, y);
                    GameObject instance = Instantiate(grassPrefab, WorldPosition, Quaternion.identity, baseGridMap);
                    //instance.transform.parent = transform; // ����Ϊ��ǰ�ű�������Ϸ������Ӷ���
                }
                else if (gridMapTest[x, y] == 1)    //����ɽ
                {
                    WorldPosition = new Vector3(x, 0 + 2 * H, y);
                    GameObject instance = Instantiate(mountainPrefab, WorldPosition, Quaternion.identity, baseGridMap);
                    //instance.transform.parent = transform; // ����Ϊ��ǰ�ű�������Ϸ������Ӷ���

                }
            }
        }
    }

    //ɾ����ǰ��ͼ
    void DeleteChildren()
    {
        // ���ȼ���Ƿ�����Ϊ "BaseGridMap" ��������
        Transform baseGridMap = transform.Find("BaseGridMap");

        if (baseGridMap != null)
        {
            if(Application.isPlaying)
            {
                Destroy(baseGridMap.gameObject);
                Debug.Log("DeleteChildren:ɾ����ǰ��ͼ�ɹ�");

            }
            else 
            { 
                DestroyImmediate(baseGridMap.gameObject);
                Debug.Log("DeleteChildren:ɾ����ǰ��ͼ�ɹ�");

            }
        }
        //if (Application.isPlaying)
        //{
        //    for (int i = transform.childCount; i > 0; i--)
        //    {
        //        Destroy(transform.GetChild(0).gameObject);
        //        Debug.Log("Destroy succeed");
        //    }
        //}
        //else
        //{
        //    for (int i = transform.childCount; i > 0; i--)
        //    {
        //        DestroyImmediate(transform.GetChild(0).gameObject);
        //    }
        //}
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
