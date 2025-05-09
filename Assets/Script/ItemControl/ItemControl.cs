using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowItem
{
    public ItemStyle Type;
    public GameObject UI;

    // 构造函数，用于初始化ShowItem对象
    public ShowItem(ItemStyle type, GameObject ui)
    {
        this.Type = type;
        this.UI = ui;
    }
}

public class ItemControl : SingletonMono<ItemControl>
{
    public ShowItem[] Items;
    public int Count = 7;
    public int index = int.MaxValue;
    public Transform ItemParent;

    public void AddItem(GameObject item)
    {
        if (IsFull())
        {
            return;
        }

        AddShowItem(item);

    }

    public void UseItem()
    {
        Debug.Log(Items[index]);
        if (Items[index] == null)
            return;

        AddBuff(Items[index].Type);

        Image item_img = Items[index].UI.transform.Find("Item").GetComponent<Image>();
        item_img.sprite = null;
        item_img.color = new Color(1,1,1,0);

        Items[index] = null;
    }

    void AddBuff(ItemStyle style)
    {
        if(style== ItemStyle.RenShen|| style == ItemStyle.GouQi)
        {

        }
        else if(style == ItemStyle.Stone)
        {

        }
        else
        {

        }
    }

    GameObject OldItemUI = null;
    public void SelectItem(GameObject UI)
    {
        GameObject bg = UI.GetComponent<Item_UI>().bg;
        bg.SetActive(false);
        if (OldItemUI != null)
        {
            GameObject Oldbg = OldItemUI.GetComponent<Item_UI>().bg;
            Oldbg.SetActive(true);
        }


        OldItemUI = UI;

        index = UI.transform.GetSiblingIndex();
        
    }

    public bool IsApple()
    {
        if (Items[index] == null)
            return false;
        if (Items.Length < index) return false;
        if (Items[index].Type == ItemStyle.Stone)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool IsFull()
    {
        foreach (var item in Items)
        {
            if (item == null)
            {
                return false;
            }
        }

        return true;
    }

    void AddShowItem(GameObject item)
    {
        for (int i = 0; i < Items.Length; i++)
        {
            if (Items[i] == null)
            {
                Image item_img = ItemParent.GetChild(i).Find("Item").GetComponent<Image>();
                item_img.sprite = item.GetComponent<ItemObj>().sprite;
                item_img.color = Color.white;

                Items[i] = new ShowItem(item.GetComponent<ItemObj>().Type, ItemParent.GetChild(i).gameObject);

                return;
            }
        }
    }

    private void Start()
    {
        Items = new ShowItem[Count];
        index = int.MaxValue;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            UseItem();
        }
    }
}
