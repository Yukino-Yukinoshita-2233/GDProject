using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcToZiYuan : MonoBehaviour
{
    public CaiLiaoStye CaiLiaoStye;
    public Transform NPC;
    private Transform Parent;
    private Vector3 OldPos;
    private bool isGoZiYuan = true;
    bool isGoZiyuanEnd = false;
    private bool isGoHome = false;
    bool isGoHomeEnd = false;
    Vector3Int addCaiLiao;
    // Start is called before the first frame update
    void Start()
    {
        OldPos = NPC.transform.position;
        if (CaiLiaoStye == CaiLiaoStye.MuCai)
        {
            Parent = GameObject.Find("res/木材").transform;
            addCaiLiao = new Vector3Int(1, 0, 0);
        }
        else if (CaiLiaoStye == CaiLiaoStye.ShiCai)
        {
            Parent = GameObject.Find("res/石材").transform;
            addCaiLiao = new Vector3Int(0, 1, 0);
        }
        else if (CaiLiaoStye == CaiLiaoStye.JinShu)
        {
            Parent = GameObject.Find("res/金属").transform;
            addCaiLiao = new Vector3Int(0, 0, 1);
        }
    }

    float t =0;
    public float targetTime = 1.5f;
    Transform CaiLiao=null;
    // Update is called once per frame
    void Update()
    {
        if (isGoZiYuan==true&&isGoZiyuanEnd==false)
        {
            //Debug.Log("设置目的地");
            CaiLiao=FindCaiLiao();
            if (CaiLiao != null)
            {
                SetNpcToCaiLiao(CaiLiao);
                isGoZiYuan = false;
            }
        }
        else if (isGoZiYuan == false && isGoZiyuanEnd == false)
        {
            //Debug.Log("行走到目的地");
            if (NPC.GetComponent<NPC>().isMove == false)
            {
                //Debug.Log("到达目的地 " + t);
                t += Time.deltaTime;
                if (t >= targetTime)
                {
                    //Debug.Log("时间到了 " + t);
                    Destroy(CaiLiao.gameObject);
                    CaiLiao = null;
                    isGoZiyuanEnd = true;
                    isGoHome = true;
                    t = 0;
                }

            }
        }

        if (isGoHome==true&& isGoZiyuanEnd == true)
        {
            //Debug.Log("设置回家路");
            NPC.GetComponent<NPC>().SetPostion(OldPos);

            isGoHome = false;
        }
        else if (isGoHome == false && isGoZiyuanEnd == true)
        {
            //Debug.Log("回到回家路");
            if (NPC.GetComponent<NPC>().isMove == false)
            {
                t += Time.deltaTime;
                if (t >= targetTime)
                {
                    isGoZiYuan = true;
                    isGoZiyuanEnd = false;
                    isGoHome = false;
                    isGoHomeEnd = false;

                    CaiLiaoManager.Instance.AddCaiLiao(addCaiLiao);

                    t = 0;
                }

                //Debug.Log("到达回家路");
            }
        }
    }

    Transform FindCaiLiao()
    {
        if (Parent.childCount == 0)
            return null;

        foreach (Transform item in Parent)
        {
            if (item.GetComponent<ZiYuan>().isLock == false)
            {
                item.GetComponent<ZiYuan>().isLock=true;
                return item;
            }
        }

        return null;
    }

    void SetNpcToCaiLiao(Transform Cailiao)
    {
        NPC.GetComponent<NPC>().SetPostion(Cailiao.position);
    }
}
