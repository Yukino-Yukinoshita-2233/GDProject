using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildData", menuName = "BuildScriptData", order = 1)]
public class BuildData : ScriptableObject
{
    [SerializeField]
    private List<BuildStyle> _buildStyles = new List<BuildStyle>();
    [SerializeField]
    private List<CailiaoXioahaoItem> _materialRequirements = new List<CailiaoXioahaoItem>();

    public Dictionary<BuildStyle, CailiaoXioahaoItem> CaiLiaoRequirements
    {
        get
        {
            var dict = new Dictionary<BuildStyle, CailiaoXioahaoItem>();
            for (int i = 0; i < _buildStyles.Count; i++)
            {
                dict[_buildStyles[i]] = _materialRequirements[i];
            }
            return dict;
        }
        set
        {
            _buildStyles.Clear();
            _materialRequirements.Clear();
            foreach (var kvp in value)
            {
                _buildStyles.Add(kvp.Key);
                _materialRequirements.Add(kvp.Value);
            }
        }
    }


}


[System.Serializable]
public class CailiaoXioahaoItem
{
    public List<CaiLiaoItem> CailiaoXioahaoList;

    // 用于Inspector显示的辅助方法
    public Dictionary<CaiLiaoStye, int> CailiaoXioahao
    {
        get
        {
            var dict = new Dictionary<CaiLiaoStye, int>();
            foreach (var item in CailiaoXioahaoList)
            {
                dict[item.caiLiaoStyel] = item.Num;
            }
            return dict;
        }
        set
        {
            CailiaoXioahaoList = new List<CaiLiaoItem>();
            foreach (var kvp in value)
            {
                CailiaoXioahaoList.Add(new CaiLiaoItem { caiLiaoStyel = kvp.Key, Num = kvp.Value });
            }
        }
    }


}
[System.Serializable]
public class CaiLiaoItem
{
    public CaiLiaoStye caiLiaoStyel;
    public int Num;
}
