using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UIElements;

public class MapGenerator : MonoBehaviour
{
    public AIController aiController;

    // ��ͼ�ߴ�
    public int width = 100;
    public int height = 100;
    public float scale = 20f;

    // ��������Ԥ�Ƽ�
    public GameObject grassPrefab;  //�ݵ���
    public GameObject waterPrefab;  //ˮ����
    public float waterTerrainSize = 0.3f;   //ˮ����ռλ�ٷֱ�

    public GameObject[] resourcePrefabs; // ��ԴԤ��������
    public int[] resourceScale; // ��Դ��С
    public float resourcesTerrainSize = 0.3f;   //��Դ����ռλ�ٷֱ�

    // �ڵ�����
    private GridNode[,] mapGrid;

    // ������ͼ
    private float[,] noiseMap;

    void Start()
    {
        GenerateMap();  //���ɵ���
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // ���������
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // ��ȡ������� GameObject
                GameObject clickedObject = hit.collider.gameObject;
                // ͨ�� Collider ���жϵ�������ĸ�����
                for (int x = 0; x < width; x++)
                {
                    for (int z = 0; z < height; z++)
                    {
                        GridNode node = mapGrid[x, z];
                            
                        if (clickedObject.transform.position == node.WorldPosition)
                        {
                            // ���������Ϣ
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

    // ���ɵ�ͼ���ݶ�ά����
    void GenerateMap()
    {
        noiseMap = GenerateNoiseMap(width, height, scale);
        mapGrid = new GridNode[width, height];

        CreateMap(); // ������ͼ����

        for (int i = 0; i < resourcePrefabs.Length; i++)
        {
            GenerateResources(i); // ������Դ
        }

        InstantiateMap(); // ʵ������ͼ����Դ
    }

    // �������β����浽�ڵ�����grid
    private void CreateMap()
    {
        // ɾ���ɵĵ�ͼ
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            // ɾ��������
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        //�����µĵ�ͼ
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                float sample = noiseMap[x, z];
                Vector3 position = new Vector3(x, 0, z);
                bool isWalkable = true;
                GameObject terrainPrefab = null;

                // ��������ֵѡ�����
                if (sample < waterTerrainSize)
                {
                    terrainPrefab = waterPrefab;
                    isWalkable = false; // ��������ͨ��
                    // ���浽�ڵ�����
                    mapGrid[x, z] = new GridNode(terrainPrefab, position, isWalkable, x, z);

                }
                else
                {
                    terrainPrefab = grassPrefab;
                    isWalkable = true; // ��������ͨ��
                    // ���浽�ڵ�����
                    mapGrid[x, z] = new GridNode(terrainPrefab, position, isWalkable, x, z);

                }

            }
        }
    }

    // ����������ͼ
    float[,] GenerateNoiseMap(int width, int height, float scale)
    {
        // ��������ƫ��ֵ
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
        // ��������ƫ��ֵ
        Vector2 offset = new Vector2(Random.Range(0f, 1000f), Random.Range(0f, 1000f));

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

    void InstantiateMap()
    {
        // �����ڵ�����ʵ�������ж���
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridNode node = mapGrid[x, z];

                if (node.TerrainPrefab != null) // �����Ԥ�Ƽ���ʵ����
                {
                    GameObject instance = Instantiate(node.TerrainPrefab, node.WorldPosition, Quaternion.identity);
                    instance.transform.parent = transform; // ����Ϊ��ǰ�ű�������Ϸ������Ӷ���
                }
            }
        }
    }
}

