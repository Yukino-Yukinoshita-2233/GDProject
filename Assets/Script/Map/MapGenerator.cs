using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public AIController aiController;

    // ��ͼ�ߴ�
    public int width = 100;
    public int height = 100;
    public float scale = 20f;

    // ����Ԥ�Ƽ�
    public GameObject grassPrefab;
    public GameObject watchPrefab;
    public GameObject forestPrefab;
    public GameObject minePrefab;
    public GameObject farmPrefab;

    //// �ϰ���Ԥ�Ƽ�
    //public GameObject stonePrefab;
    //public GameObject treePrefab;
    //public GameObject chestPrefab;
    //public GameObject cropPrefab;

    //// ����Ԥ�Ƽ�
    //public GameObject baseCampPrefab;
    //public GameObject wallPrefab;
    //public GameObject towerPrefab;
    //public GameObject housePrefab;
    //public GameObject woodFactoryPrefab;
    //public GameObject stoneFactoryPrefab;
    //public GameObject metalFactoryPrefab;

    // �ڵ�����
    private GridNode[,] grid;

    // �ṩ�������ʷ�������ȡ grid ����
    //public GridNode[,] GetGrid()
    //{
    //    return grid;
    //}

    // ������ͼ
    private float[,] noiseMap;

    void Start()
    {
        GenerateMap();
    }

    //void Update()
    //{
    //    GridNode[,] grid = MapGenerator.grid;
    //    if (grid == null)
    //    {
    //        Debug.LogError("Grid not initialized!");
    //        return;
    //    }

    //    AIController.PlanPath(grid);
    //}

    // ��ĳ�������е��� AIController ��·���滮������������������Ϊ����
    public void CallAIController()
    {
        float[,] grid = noiseMap; // ����������߼�������ʾ��Ϊ GenerateGrid()

        if (aiController != null)
        {
            aiController.PlanPath(grid); // ��������Ϊ�������ݸ� AIController
        }
        else
        {
            Debug.LogError("AIController reference is null!");
        }
    }

    // ���ɵ�ͼ
    void GenerateMap()
    {
        noiseMap = GenerateNoiseMap(width, height, scale);
        grid = new GridNode[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                float sample = noiseMap[x, z];
                Vector3 position = new Vector3(x, 0, z);
                bool isWalkable = true;
                GameObject terrainPrefab = null;

                // ��������ֵѡ�����
                if (sample < 0.2f)
                {
                    terrainPrefab = watchPrefab;
                    isWalkable = false; // ��������ͨ��
                }
                else if (sample < 0.4f)
                {
                    terrainPrefab = forestPrefab;
                }
                else if (sample < 0.6f)
                {
                    terrainPrefab = minePrefab;
                }
                else if (sample < 0.8f)
                {
                    terrainPrefab = farmPrefab;
                }
                else
                {
                    terrainPrefab = grassPrefab;
                }

                // �������ε�ʵ��
                GameObject terrainInstance = Instantiate(terrainPrefab, position, Quaternion.identity);
                // �� terrainInstance ����Ϊ��ǰ�ű�������Ϸ������Ӷ���
                terrainInstance.transform.parent = transform;
                grid[x, z] = new GridNode(position, isWalkable, x, z);
            }
        }

        //PlaceObstacles();
        //PlaceBuildings();
    }

    // ����������ͼ
    float[,] GenerateNoiseMap(int width, int height, float scale)
    {
        float[,] noiseMap = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                float sampleX = x / scale;
                float sampleZ = z / scale;
                float noiseValue = Mathf.PerlinNoise(sampleX, sampleZ);
                noiseMap[x, z] = noiseValue;
            }
        }

        return noiseMap;
    }

    //// ��������ϰ���
    //void PlaceObstacles()
    //{
    //    for (int i = 0; i < 50; i++)
    //    {
    //        Vector3 position = new Vector3(Random.Range(0, width), 0, Random.Range(0, height));
    //        Instantiate(stonePrefab, position, Quaternion.identity);
    //        grid[(int)position.x, (int)position.z].IsWalkable = false; // ��������ڵ�Ϊ����ͨ��
    //    }
    //}

    //// ������ý���
    //void PlaceBuildings()
    //{
    //    for (int i = 0; i < 10; i++)
    //    {
    //        Vector3 position = GetRandomPositionOnGrass();
    //        Instantiate(baseCampPrefab, position, Quaternion.identity);
    //    }
    //}

    // �ڲݵ��ϻ�ȡ���λ��
    Vector3 GetRandomPositionOnGrass()
    {
        while (true)
        {
            int x = Random.Range(0, width);
            int z = Random.Range(0, height);
            if (grid[x, z].IsWalkable && noiseMap[x, z] >= 0.8f)
            {
                return new Vector3(x, 0, z);
            }
        }
    }
}
