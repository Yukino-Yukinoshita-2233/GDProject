using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//------------------------------------挂在要拖动的UI上--------------------------------------
public class Drag : MonoBehaviour, IPointerDownHandler
{
    //要生成的物体
    public string Path;

    public Transform parent;
    //生成的物体
    private GameObject dragObj;
    //是否正在拖动
    private bool isDrag;

    public BuildStyle buildStyle;


    private void Start()
    {

        
    }

    private void Update()
    {
        if (isDrag)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                //dragObj.transform.position = hit.point;
                dragObj.transform.position = hit.point + new Vector3(hit.normal.x * dragObj.transform.localScale.x / 2,
    hit.normal.y * dragObj.transform.localScale.y / 2,
    hit.normal.z * dragObj.transform.localScale.z / 2);
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (dragObj.activeSelf)
                {
                    isDrag = false;
                    Destroy(dragObj);
                }

                Vector3Int Xioahao = BuildManger.Instance.GetXiaoHao(buildStyle);
                Debug.Log("消耗为 "+Xioahao);
                if (!CaiLiaoManager.Instance.GetUseCaiLiao(Xioahao))
                {
                    return;
                }

                var obj = Resources.Load(Path);
                GameObject TarGetObj = Instantiate(obj) as GameObject;
                TarGetObj.SetActive(true);
                TarGetObj.transform.position = dragObj.transform.position;
                TarGetObj.transform.rotation = dragObj.transform.rotation;
                TarGetObj.transform.SetParent(parent);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                // 获取当前物体的Quaternion表示的旋转
                Quaternion currentRotation = dragObj.transform.rotation;

                // 创建一个表示沿Y轴旋转10度的Quaternion
                Quaternion yAxisRotation = Quaternion.Euler(0f, 90f, 0f);

                // 将这个旋转应用到当前的旋转上
                dragObj.transform.rotation = currentRotation * yAxisRotation;
            }

            //Debug.Log(dragObj.GetComponent<BoxCollider>().enabled);
            //Debug.Log(dragObj.GetComponent<DragModel>().enabled);
        }
    }

    //鼠标按下生成物体
    public void OnPointerDown(PointerEventData eventData)
    {
        isDrag = true;
        //dragObj = Instantiate(prefab);
        //将资源加载到游戏进程中
        var obj = Resources.Load(Path+ "_Normal");
        //实例化一个资源到场景中
        dragObj = Instantiate(obj) as GameObject;
        dragObj.SetActive(true);

    }
}