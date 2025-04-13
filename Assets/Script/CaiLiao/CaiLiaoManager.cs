using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CaiLiaoStye
{
    MuCai,ShiCai,JinShu
}

public class CaiLiaoManager : SingletonMono<CaiLiaoManager>
{
    public int MuCaiCount = 50;
    public int ShiCaiCount = 50;
    public int JinShuCount = 50;

    public Text MuCaiCount_Text;
    public Text ShiCaiCount_Text;
    public Text JinShuCount_Text;

    public bool GetUseCaiLiao(Vector3Int xiaohao)
    {
        if((MuCaiCount - xiaohao.x) <0|| (ShiCaiCount - xiaohao.y) < 0|| (JinShuCount - xiaohao.z) < 0)
        {
            return false;
        }
        else
        {
            MuCaiCount -= xiaohao.x;
            ShiCaiCount -= xiaohao.y;
            JinShuCount -= xiaohao.z;

            UpdateCaiLiao();

            return true;
        }


    }

    public bool isUseCaiLiao(Vector3Int xiaohao)
    {
        if ((MuCaiCount - xiaohao.x) < 0 || (ShiCaiCount - xiaohao.y) < 0 || (JinShuCount - xiaohao.z) < 0)
        {
            return false;
        }
        else
        {
            return true;
        }


    }

    public void AddCaiLiao(Vector3Int Cailiao)
    {
        MuCaiCount += Cailiao.x;
        ShiCaiCount += Cailiao.y;
        JinShuCount += Cailiao.z;

        UpdateCaiLiao();
    }

    private void UpdateCaiLiao()
    {
        MuCaiCount_Text.text = MuCaiCount.ToString();
        ShiCaiCount_Text.text = ShiCaiCount.ToString();
        JinShuCount_Text.text = JinShuCount.ToString();
    }
}
