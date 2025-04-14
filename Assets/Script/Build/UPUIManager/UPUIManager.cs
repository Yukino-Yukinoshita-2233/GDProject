using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UPUIManager : SingletonMono<UPUIManager>
{
    public Button Up_But;
    public GameObject Mask;
    public float xOffset;
    public float yOffset;

    private NpcToZiYuan buding;

    private void Start()
    {
        this.GetComponent<Canvas>().enabled = false;
        Up_But.onClick.AddListener(UpdataBuilding);
    }

    void UpdataBuilding()
    {
        if (CaiLiaoManager.Instance.GetUseCaiLiao(buding.buildUpLvData.upgradeResources))
        {
            buding.ShangXian = buding.buildUpLvData.upgradedStorageLimit;
        }

        Hide();
    }


    public void Show(GameObject Build)
    {
        buding = Build.GetComponent<NpcToZiYuan>();

        if (buding.ShangXian>= buding.buildUpLvData.upgradedStorageLimit)
        {
            Mask.SetActive(true);
            return;
        }

        this.GetComponent<Canvas>().enabled = true;
        Vector2 player2DPosition = Camera.main.WorldToScreenPoint(Build.transform.position);
        this.GetComponent<RectTransform>().position = player2DPosition + new Vector2(xOffset, yOffset);
        
        
        if (CaiLiaoManager.Instance.isUseCaiLiao(buding.buildUpLvData.upgradeResources))
        {
            Mask.SetActive(false);
        }
        else
        {
            Mask.SetActive(true);
        }
    }

    public void Hide()
    {
        buding = null;
        this.GetComponent<Canvas>().enabled = false;
    }
}
