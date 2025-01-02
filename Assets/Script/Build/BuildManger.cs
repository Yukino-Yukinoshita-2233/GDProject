using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public enum BuildStyle
{
    DaBenYing,ChengQiang,FangYuTa,ZhuFang,MuCaiJGC,ShiCaiJGC,JinShuJGC
}
public class BuildManger : SingletonMono<BuildManger>
{
    public BuildData BuildData;

    /// <summary>
    /// /Ä¾²Ä Ê¯²Ä ½ðÊô
    /// </summary>
    /// <param name="style"></param>
    /// <returns></returns>
    public Vector3Int GetXiaoHao(BuildStyle style)
    {
        return new Vector3Int(BuildData.CaiLiaoRequirements[style].CailiaoXioahao[CaiLiaoStye.MuCai],
            BuildData.CaiLiaoRequirements[style].CailiaoXioahao[CaiLiaoStye.ShiCai],
            BuildData.CaiLiaoRequirements[style].CailiaoXioahao[CaiLiaoStye.JinShu]
            );
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
