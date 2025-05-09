using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Item_UI : MonoBehaviour,IPointerDownHandler
{
    public ItemStyle style= ItemStyle.Null;
    public GameObject bg;
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Down");
        ItemControl.Instance.SelectItem(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        bg = this.transform.Find("bg").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
