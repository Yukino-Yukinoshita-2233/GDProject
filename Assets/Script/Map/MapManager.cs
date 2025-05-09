using System;
using System.Collections;
using UnityEngine;


namespace MapManagernamespace
{
    public class MapManager : MonoBehaviour
    {
        // ����ʵ��
        private static MapManager instance;
        MapGenerator mapGenerator;
        // ��ͼ�ߴ�
        
        public static int width = 192;//73
        public static int height = 108;//45
        public static int scale = 12;
        public static float[,] noiseMap = new float[width, height];
        public static int[,] gridMap = new int[width, height];

        // ��������
        public GameObject waterPrefab;  //ˮ����
        public GameObject[] grassPrefab;  //�ݵ���
        public GameObject mountainPrefab;  //ɽ����
        public float waterTerrainSize = 0.3f;   //ˮ����ռλ�ٷֱ�
        public float mountainTerrainSize = 0.6f;   //ɽ����ռλ�ٷֱ�

        //��Դ����
        public GameObject[] resourcePrefabs; // ��ԴԤ��������
        public int[] resourceScale; // ��Դ��С
        public float resourcesTerrainSize = 0.3f;   //��Դ����ռλ�ٷֱ�

        public static bool isMapChange = false;
        [ContextMenu("TestMap")]
        public void TestMap()
        {
            // ���ɵ�ͼ
            Debug.Log("TestMap:���ɵ�ͼ����");
            GenerateMap(width, height, scale, waterTerrainSize, mountainTerrainSize);
            mapGenerator = GameObject.Find("Terrain").GetComponent<MapGenerator>();
            //mapGenerator.OnMapGenerated();
        }

        void Awake()
        {
            // ����Ƿ�����ʵ��
            if (instance == null)
            {
                instance = this;
                // ��ֹ�糡��ʱ�ظ�
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                // �������ʵ�����ڣ���������µ�
                Destroy(gameObject);
            }
            instance = new MapManager(width, height);
            Debug.Log("MapGenerator Start:���ɵ�ͼ����");
            GenerateMap(width, height, scale, waterTerrainSize, mountainTerrainSize);
            mapGenerator = GameObject.Find("Terrain").GetComponent<MapGenerator>();
            //mapGenerator.OnMapGenerated();

        }
        // ����ģʽ��ȷ�� MapManager ֻ����һ��ʵ��
        public static MapManager Instance
        {
            get
            {
                if (instance == null)
                {
                    //instance = new MapManager(192, 108);
                }
                return instance;
            }
        }
        // ˽�й��캯������ֹ�ⲿʵ����
        private MapManager(int widthT, int heightT)
        {
            width = widthT;
            height = heightT;
            // ��ʼ��������ͼ�������ͼ
            noiseMap = new float[width, height];
            gridMap = new int[width, height];
            Debug.Log("MapManager��ʼ��");
            Debug.Log(noiseMap.Length);
        }
        // ���ɵ�ͼ����������
        public void GenerateMap(int widthT, int heightT, int scaleT, float waterTerrainSizeT, float mountainTerrainSizeT)
        {

            Debug.Log("GenerateMap:��ʼ���ɵ�ͼ");
            noiseMap = GenerateNoiseMap(noiseMap, widthT, heightT, scaleT);
            gridMap = GenerategridMap(noiseMap, gridMap, waterTerrainSizeT, mountainTerrainSizeT);
            isMapChange = true;
            Debug.Log("GenerateMap:��ͼ�������");
        }

        // ����������ͼ
        private float[,] GenerateNoiseMap(float[,] noiseMapT, int width, int height, float scale)
        {
            Debug.Log("GenerateNoiseMap:��������������ͼ");

            if (noiseMapT == null)
            {
                Debug.Log("GenerateNoiseMap noiseMap has not been generated yet!");
            }

            // ��������ƫ��ֵ
            Vector2 offset = new Vector2(UnityEngine.Random.Range(0f, 1000f), UnityEngine.Random.Range(0f, 1000f));
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float sampleX = (x + offset.x) / scale;
                    float sampleZ = (y + offset.y) / scale;
                    float noiseValue = Mathf.PerlinNoise(sampleX, sampleZ);//�����㷨
                    noiseMapT[x, y] = noiseValue;
                }
            }
            return noiseMapT;
        }

        // �������β����浽�ڵ�����grid
        private int[,] GenerategridMap(float[,] noiseMapT, int[,] gridMapT, float waterTerrainSizeT, float mountainTerrainSizeT)
        {
            Debug.Log("GenerategridMap:�������������ͼ");
            if (noiseMapT == null)
            {
                Debug.Log("GenerategridMap noiseMap has not been generated yet!");
            }
            //�����µĵ�ͼ
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float sample = noiseMapT[x, y];

                    // ��������ֵѡ�����
                    if (sample < waterTerrainSizeT)//�����ϰ��� ˮ�ڵ�
                    {
                        gridMapT[x, y] = 3;
                    }
                    else if (sample > mountainTerrainSizeT)//�����ϰ��� ɽ�ڵ�
                    {
                        gridMapT[x, y] = 1;
                    }
                    else
                    {
                        gridMapT[x, y] = 0;  //���ɿ����� �ݽڵ�
                    }
                }
            }
            return gridMapT;
        }

        // ��ȡ��ͼ����
        public int[,] GetMap()
        {
            if (gridMap == null)
            {
                Debug.Log("GetMap gridMap has not been generated yet!");
                return null;
            }
            return gridMap;
        }
    }
}

public class PrintDebug : MonoBehaviour 
{
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


