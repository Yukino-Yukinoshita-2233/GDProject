using System.Collections;
using UnityEngine;
using MapManagernamespace;
using System.Collections.Generic;
using Unity.VisualScripting;

public class MapGeneratorWithItems : MonoBehaviour
{
    // 地图尺寸
    int widthGen;
    int heightGen;
    int[,] gridMapGen = null;   //网格地图

    // 基础地形
    public Transform MuCaiParent;  //木材父物体
    public Transform ShiCaiParent;  //石材父物体
    public Transform JinShuParent;  //金属父物体

    // 资源道具
    public GameObject[] itemPrefabs;  //道具预制体数组
    public int itemCount = 5;         //控制道具生成的数量


    private void Start()
    {
        widthGen = MapManager.width;
        heightGen = MapManager.height;
        // 获取地图数据
        gridMapGen = MapManager.gridMap;
    }

    private void LateUpdate()
    {
        // 在草地上随机位置生成道具
        SuppleMentCaiLiao();
    }

    //补充资源
    void SuppleMentCaiLiao()
    {
        if (MuCaiParent.childCount < 5)
        {
            InstantiateItemsOnGrass(MuCaiParent, itemPrefabs[0]);
            Debug.Log("生成材料:" + itemPrefabs[0].name);
            if (ShiCaiParent.childCount < 5)
            {
                InstantiateItemsOnGrass(ShiCaiParent, itemPrefabs[1]);
                Debug.Log("生成材料:" + itemPrefabs[1].name);
            }
            if (JinShuParent.childCount < 5)
            {
                InstantiateItemsOnGrass(JinShuParent, itemPrefabs[2]);
                Debug.Log("生成材料:" + itemPrefabs[2].name);
            }
        }


        // 在草地上生成道具
        void InstantiateItemsOnGrass(Transform CaiLiaoParent, GameObject itemPrefab)
        {

            // 随机选择一个位置
            int x = Random.Range(0, widthGen);
            int y = Random.Range(0, heightGen);

            // 确保当前位置是草地并且没有道具已生成
            if (gridMapGen[x, y] == 0)
            {
                // 生成道具
                Vector3 worldPosition = new Vector3(x, 0.5f, y);
                Instantiate(itemPrefab, worldPosition, Quaternion.identity, CaiLiaoParent);

                //if (itemPrefab.GetComponent<ZiYuan>().CaiLiaoStye == CaiLiaoStye.MuCai)
                //{
                //    // 生成道具
                //    Vector3 worldPosition = new Vector3(x, 0.5f, y);
                //    Instantiate(itemPrefab, worldPosition, Quaternion.identity, MuCaiParent);
                //}
                //else if (itemPrefab.GetComponent<ZiYuan>().CaiLiaoStye == CaiLiaoStye.ShiCai)
                //{
                //    // 生成道具
                //    Vector3 worldPosition = new Vector3(x, 0.5f, y);
                //    Instantiate(itemPrefab, worldPosition, Quaternion.identity, ShiCaiParent);
                //}
                //else if (itemPrefab.GetComponent<ZiYuan>().CaiLiaoStye == CaiLiaoStye.JinShu)
                //{
                //    // 生成道具
                //    Vector3 worldPosition = new Vector3(x, 0.5f, y);
                //    Instantiate(itemPrefab, worldPosition, Quaternion.identity, JinShuParent);
                //}

            }
        }
    }
}