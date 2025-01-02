using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//------------------------------------����Ҫ�϶���UI��--------------------------------------
public class Drag : MonoBehaviour, IPointerDownHandler
{
    //Ҫ���ɵ�����
    public string Path;

    public Transform parent;
    //���ɵ�����
    private GameObject dragObj;
    //�Ƿ������϶�
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
                Debug.Log("����Ϊ "+Xioahao);
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
                // ��ȡ��ǰ�����Quaternion��ʾ����ת
                Quaternion currentRotation = dragObj.transform.rotation;

                // ����һ����ʾ��Y����ת10�ȵ�Quaternion
                Quaternion yAxisRotation = Quaternion.Euler(0f, 90f, 0f);

                // �������תӦ�õ���ǰ����ת��
                dragObj.transform.rotation = currentRotation * yAxisRotation;
            }

            //Debug.Log(dragObj.GetComponent<BoxCollider>().enabled);
            //Debug.Log(dragObj.GetComponent<DragModel>().enabled);
        }
    }

    //��갴����������
    public void OnPointerDown(PointerEventData eventData)
    {
        isDrag = true;
        //dragObj = Instantiate(prefab);
        //����Դ���ص���Ϸ������
        var obj = Resources.Load(Path+ "_Normal");
        //ʵ����һ����Դ��������
        dragObj = Instantiate(obj) as GameObject;
        dragObj.SetActive(true);

    }
}