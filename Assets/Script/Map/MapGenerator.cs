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

    //��Դ����Ԥ�Ƽ�
    public GameObject forestPrefab; //ɭ����Դ����
    public GameObject minePrefab;   //������Դ����
    public GameObject farmPrefab;   //ʳ����Դ����
    
    public int forestResourcePosCount = 10;    //ɭ����Դ������
    public int forestresourceBlockSize = 5;   //ɭ����Դ���С
    //public int mineResourcePosCount = 10;    //������Դ������
    //public int mineresourceBlockSize = 5;   //������Դ���С
    //public int farmResourcePosCount = 10;    //ʳ����Դ������
    //public int farmresourceBlockSize = 5;   //ʳ����Դ���С


    //// ����Ԥ�Ƽ�
    //public GameObject baseCampPrefab;
    //public GameObject wallPrefab;
    //public GameObject towerPrefab;
    //public GameObject housePrefab;
    //public GameObject woodFactoryPrefab;
    //public GameObject stoneFactoryPrefab;
    //public GameObject metalFactoryPrefab;

    // �ڵ�����
    private GridNode[,] mapGrid;
    private GridNode[,] ResourcesGrid;
    // ������ͼ
    private float[,] noiseMap;
    // ����ƫ��ֵ
    private Vector2 offset;

    void Start()
    {
        GenerateMap();  //���ɵ���
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
        PlaceResources();

                CreateMap();

    }

    //�������β����浽�ڵ�����grid
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

                }
                else if(sample >= waterTerrainSize &&  sample <= 1.0f)
                {
                    terrainPrefab = grassPrefab;

                }
                else if (sample == 2.0f)
                {
                    terrainPrefab = forestPrefab;
                    isWalkable = false;
                }
                else if (sample == 3.0f)
                {
                    terrainPrefab = minePrefab;
                    isWalkable = false;
                }
                else if (sample == 4.0f)
                {
                    terrainPrefab = farmPrefab;
                    isWalkable = false;
                }



                // �������ε�ʵ��
                GameObject terrainInstance = Instantiate(terrainPrefab, position, Quaternion.identity);
                // �� terrainInstance ����Ϊ��ǰ�ű�������Ϸ������Ӷ���
                terrainInstance.transform.parent = transform;
                mapGrid[x, z] = new GridNode(terrainInstance, position, isWalkable, x, z);
            }
        }

    }


    // ����������ͼ
    float[,] GenerateNoiseMap(int width, int height, float scale)
    {
        // ��������ƫ��ֵ
        offset = new Vector2(Random.Range(0f, 1000f), Random.Range(0f, 1000f));

        float[,] noiseMap = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                float sampleX = (x + offset.x) / scale;
                float sampleZ = (z + offset.y) / scale;
                float noiseValue = Mathf.PerlinNoise(sampleX, sampleZ);
                noiseMap[x, z] = noiseValue;
            }
        }

        return noiseMap;
    }

    // �ڲݵ��ϻ�ȡ���λ��
    Vector3 GetRandomPositionOnGrass()
    {
        while (true)
        {
            int x = Random.Range(10, width-10);
            int z = Random.Range(10, height-10);
            if (noiseMap[x, z] >= waterTerrainSize && noiseMap[x, z] <= 1.0f)
            {
                return new Vector3(x, 0, z);
            }
        }
    }

    // ��������ϰ���
    void PlaceResources()
    {
        for (int i = 0; i < forestResourcePosCount; i++)
        {

            Vector3 position = GetRandomPositionOnGrass();
            int newforestresourceBlockSize = 0;
            // ������Դ�����ڵ�ÿ����
            for (int x = ((int)position.x - newforestresourceBlockSize/2); x <= (position.x + newforestresourceBlockSize/2); x++)
            {
                for (int z = ((int)position.z- newforestresourceBlockSize/2); z <= (position.z + newforestresourceBlockSize/2); z++)
                {
                    // ��������ͼ�ϱ����Դ���飨���磬���߶�ֵ���ӻ�����Ϊ�ض�ֵ��
                    noiseMap[x, z] = Random.Range(2,5); // ����Դ����ĸ߶�����Ϊ���ֵ
                }
            }
        }
    }

    //// ������ý���
    //void PlaceBuildings()
    //{
    //    for (int i = 0; i < 10; i++)
    //    {
    //        Vector3 position = GetRandomPositionOnGrass();
    //        Instantiate(baseCampPrefab, position, Quaternion.identity);
    //    }
    //}

}
