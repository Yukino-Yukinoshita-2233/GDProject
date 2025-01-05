using System.Collections;
using UnityEngine;
using MapManagernamespace;
using System.Collections.Generic;
using Unity.VisualScripting;

public class MapGeneratorWithItems : MonoBehaviour
{
    // ��ͼ�ߴ�
    int widthGen;
    int heightGen;
    int[,] gridMapGen = null;   //�����ͼ

    // ��������
    public Transform MuCaiParent;  //ľ�ĸ�����
    public Transform ShiCaiParent;  //ʯ�ĸ�����
    public Transform JinShuParent;  //����������

    // ��Դ����
    public GameObject[] itemPrefabs;  //����Ԥ��������
    public int itemCount = 5;         //���Ƶ������ɵ�����


    private void Start()
    {
        widthGen = MapManager.width;
        heightGen = MapManager.height;
        // ��ȡ��ͼ����
        gridMapGen = MapManager.gridMap;
    }

    private void LateUpdate()
    {
        // �ڲݵ������λ�����ɵ���
        SuppleMentCaiLiao();
    }

    //������Դ
    void SuppleMentCaiLiao()
    {
        if (MuCaiParent.childCount < 5)
        {
            InstantiateItemsOnGrass(MuCaiParent, itemPrefabs[0]);
            Debug.Log("���ɲ���:" + itemPrefabs[0].name);
            if (ShiCaiParent.childCount < 5)
            {
                InstantiateItemsOnGrass(ShiCaiParent, itemPrefabs[1]);
                Debug.Log("���ɲ���:" + itemPrefabs[1].name);
            }
            if (JinShuParent.childCount < 5)
            {
                InstantiateItemsOnGrass(JinShuParent, itemPrefabs[2]);
                Debug.Log("���ɲ���:" + itemPrefabs[2].name);
            }
        }


        // �ڲݵ������ɵ���
        void InstantiateItemsOnGrass(Transform CaiLiaoParent, GameObject itemPrefab)
        {

            // ���ѡ��һ��λ��
            int x = Random.Range(0, widthGen);
            int y = Random.Range(0, heightGen);

            // ȷ����ǰλ���ǲݵز���û�е���������
            if (gridMapGen[x, y] == 0)
            {
                // ���ɵ���
                Vector3 worldPosition = new Vector3(x, 0.5f, y);
                Instantiate(itemPrefab, worldPosition, Quaternion.identity, CaiLiaoParent);

                //if (itemPrefab.GetComponent<ZiYuan>().CaiLiaoStye == CaiLiaoStye.MuCai)
                //{
                //    // ���ɵ���
                //    Vector3 worldPosition = new Vector3(x, 0.5f, y);
                //    Instantiate(itemPrefab, worldPosition, Quaternion.identity, MuCaiParent);
                //}
                //else if (itemPrefab.GetComponent<ZiYuan>().CaiLiaoStye == CaiLiaoStye.ShiCai)
                //{
                //    // ���ɵ���
                //    Vector3 worldPosition = new Vector3(x, 0.5f, y);
                //    Instantiate(itemPrefab, worldPosition, Quaternion.identity, ShiCaiParent);
                //}
                //else if (itemPrefab.GetComponent<ZiYuan>().CaiLiaoStye == CaiLiaoStye.JinShu)
                //{
                //    // ���ɵ���
                //    Vector3 worldPosition = new Vector3(x, 0.5f, y);
                //    Instantiate(itemPrefab, worldPosition, Quaternion.identity, JinShuParent);
                //}

            }
        }
    }
}