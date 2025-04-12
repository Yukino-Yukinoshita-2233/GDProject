using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;
using Unity.Burst.CompilerServices;
using System;
using System.IO;
using TMPro; // ����TextMeshPro�����ռ�

/// <summary>
/// ѡ�������������ѡ���Ѿ���λ��ʿ�������������ǵ��ж���
/// </summary>
public class SelectionManager : MonoBehaviour
{
    // ��ѡ�еĵ�λ�б�
    public List<Soldier> selectedSoldiers = new List<Soldier>();
    public List<Soldier> selectedObjectIcon = new List<Soldier>();

    // ��ѡ��ʼ��
    private Vector2 selectionStartPos;
    private Vector2 selectionEndPos;
    private bool isSelecting = false;

    // ��ѡ��UI
    public GameObject selectionObjectParent;
    public List<GameObject> selectionObjectIcon = new List<GameObject>();
    public List<GameObject> SoldierType = new List<GameObject>();
    public Transform soldierParent;

    // ѡ��ť
    public List<Button> SoldierStateButton = new List<Button>();
    public List<Button> SoldierTypeButton = new List<Button>();

    public static Transform castle; // �Ǳ�
    public TextMeshProUGUI castleHP;
    public BuildStyle buildStyle;

    private void Start()
    {
        castle = GameObject.Find("Building").transform.Find("Castle");

        StartCoroutine(ClearSelectionObjectChildrenNextFrame());

        // �����б���ÿ����ť��Ӽ����¼�
        for (int i = 0; i < SoldierStateButton.Count; i++)
        {
            int index = i; // ��� Lambda �հ�����
            SoldierStateButton[i].onClick.AddListener(() => OnSoldierStateButtonClick(index));
        }
        for (int i = 0; i < SoldierTypeButton.Count; i++)
        {
            int index = i; // ��� Lambda �հ�����
            SoldierTypeButton[i].onClick.AddListener(() => OnSoldierTypeButtonClick(index));
        }
    }
    private void Update()
    {
        HandleSelectionInput(); // �������ѡ���߼�
        HandleCommandInput();   // �����Ҽ������߼�
        //isSelectedOnGUI();
        castleHP.text = castle.GetComponent<Health>().currentHealth.ToString() + "/" + castle.GetComponent<Health>().maxHealth.ToString();
    }

    void ShowAllButtons(bool isShow)
    {
        foreach (Button btn in SoldierStateButton)
        {
            btn.gameObject.SetActive(isShow);
        }
    }

    /// <summary>
    /// ��ť����¼�����
    /// </summary>
    void OnSoldierStateButtonClick(int index)
    {
        Debug.Log($"��ť {index} �����");

        // ������ִ�а�ť�������߼�
        if(index == 2)
        {
            for (int i = selectedSoldiers.Count - 1; i >= 0; i--)
            {
                var soldier = selectedSoldiers[i];
                if (soldier == null)
                {
                    selectedSoldiers.RemoveAt(i); // ��ȫɾ���ն���
                }
                else
                {
                    // ����Ŀ��㲢���ø�ʿ��
                    soldier.SetTarget(castle.transform.position);
                }
            }
        }
        else
        {
            for (int i = selectedSoldiers.Count - 1; i >= 0; i--)
            {
                var soldier = selectedSoldiers[i];
                if (soldier == null)
                {
                    selectedSoldiers.RemoveAt(i); // ��ȫɾ���ն���
                }
                else
                {
                    soldier.currentBaseState = SoldierBaseState.Idle;
                    soldier.currentState = (SoldierState)index;
                }
            }
        }


    }
    
    /// <summary>
    /// ��ť����¼�����
    /// </summary>
    void OnSoldierTypeButtonClick(int index)
    {
        Debug.Log($"��ť {index} �����");

        // ������ִ�а�ť�������߼�
        if (index == 0)
        {
            buildStyle = BuildStyle.JianShi;
        }
        else if (index == 1)
        {
            buildStyle = BuildStyle.FaShi;
        }

        Vector3Int Xioahao = BuildManger.Instance.GetXiaoHao(buildStyle);
        Debug.Log("����Ϊ " + Xioahao);
        if (!CaiLiaoManager.Instance.GetUseCaiLiao(Xioahao))
        {
            return;
        }

        var obj = SoldierType[index];
        GameObject TarGetObj = Instantiate(obj, castle.position + new Vector3(0,1,0), Quaternion.Euler(0, 180, 0), soldierParent);
        StartCoroutine(MoveBackward(TarGetObj.transform, -2f, 1f)); // �ƶ� 2 ����λ���� 1 �����

    }

    /// <summary>
    /// ʿ��ѵ����λ���ƶ�
    /// </summary>
    IEnumerator MoveBackward(Transform soldier, float distance, float duration)
    {
        Vector3 startPos = soldier.position;
        Vector3 endPos = startPos - soldier.forward * distance; // -Z ����
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            soldier.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        soldier.position = endPos; // ȷ������λ����ȷ
    }

    /// <summary>
    /// �����ѡ�͵�������������߼���
    /// </summary>
    private void HandleSelectionInput()
    {

        // ��ʼ��ѡ
        if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject() == false)
        {
            for (int i = selectedSoldiers.Count - 1; i >= 0; i--)
            {
                var soldier = selectedSoldiers[i];
                if (soldier == null)
                {
                    selectedSoldiers.RemoveAt(i); // ��ȫɾ���ն���
                }
                else
                {
                    soldier.transform.Find("AttackRangeImage").gameObject.SetActive(false); // �رչ�����Χ��ʾ

                }
            }

            // ���֮ǰ��ѡ��
            selectedSoldiers.Clear();

            selectionStartPos = Input.mousePosition;
            isSelecting = true; // �����ѡģʽ
        }
        // ��ѡ����
        if (Input.GetMouseButtonUp(0) && isSelecting)
        {
            // ��ȡ��Ļѡ���ķ�Χ
            selectionEndPos = Input.mousePosition;
            isSelecting = false;
            if(selectionStartPos == selectionEndPos)
            {
                SelectUnitsSingle();
                //syncSelectionObject();

            }
            else
            {
                SelectUnitsInRectangle();
                //syncSelectionObject();

            }
            StartCoroutine(ClearSelectionObjectChildrenNextFrame());

        }
    }

    /// <summary>
    /// ������λ��ѡ��һ��ʿ����
    /// </summary>
    private void SelectUnitsSingle()
    {
        // ���֮ǰ��ѡ�������Ҫ�Ļ�������ȡ��ע���������д���
        // selectedSoldiers.Clear();

        // �����������ȡ�����λ�ö�Ӧ������
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // ʹ�� Physics.Raycast �������߼�⣬�ж��Ƿ����Ŀ��
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            // �����������ӡ���߼�⵽�Ķ������ƣ����ڵ���ȷ�������������
            Debug.Log("���߼�⵽����: " + hitInfo.collider.gameObject.name);

            // ���Ի�ȡ���߻��ж����ϵ� Soldier ���
            Soldier soldier = hitInfo.collider.GetComponent<Soldier>();
            if (soldier != null)
            {
                // �����⵽�Ķ����� Soldier ������򽫸�ʿ������ѡ���б�
                selectedSoldiers.Add(soldier);
                Debug.Log($"Selected soldier: {soldier.name}");
            }
            else
            {
                //if (hitInfo.collider.tag == "JGC")
                //{
                //    UPUIManager.Instance.Show(hitInfo.collider.gameObject);

                //}
                //else
                //{
                //    UPUIManager.Instance.Hide();

                //}
                // �����⵽�Ķ���û�� Soldier ������������ʾ��Ϣ
                Debug.Log("No soldier selected");
            }
        }
    }


    /// <summary>
    /// ��ѡ��λ����������������ʿ������ѡ���б�
    /// </summary>
    private void SelectUnitsInRectangle()
    {
        // ���֮ǰ��ѡ��
        //selectedSoldiers.Clear();

        Rect selectionRect = new Rect(
            Mathf.Min(selectionStartPos.x, selectionEndPos.x),
            Mathf.Min(selectionStartPos.y, selectionEndPos.y),
            Mathf.Abs(selectionStartPos.x - selectionEndPos.x),
            Mathf.Abs(selectionStartPos.y - selectionEndPos.y)
        );

        // ��������ʿ�����ж��Ƿ���ѡ�����
        foreach (var soldier in SoldierManager.Instance.GetAllSoldiers())
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(soldier.transform.position);
            if (selectionRect.Contains(screenPos, true))
            {
                selectedSoldiers.Add(soldier);
                soldier.transform.Find("AttackRangeImage").gameObject.SetActive(true); // ����

            }
        }

        Debug.Log($"Selected {selectedSoldiers.Count} soldiers");
        //syncSelectionObject();

    }

    IEnumerator ClearSelectionObjectChildrenNextFrame()
    {
        // ѭ��ֱ�������Ӷ��󶼱�����
        while (selectionObjectParent.transform.childCount > 0)
        {
            //Debug.Log("����icon");
            Destroy(selectionObjectParent.transform.GetChild(0).gameObject);
            // �ȴ�һ֡���� Destroy() ��Ч
            yield return null;
        }
        syncSelectionObject();

    }

    /// <summary>
    /// ͬ����ѡ���������±�ѡ��������ʾͼ�ꡣ
    /// </summary>
    void syncSelectionObject()
    {
        //StartCoroutine(ClearSelectionObjectChildrenNextFrame());
        if (selectedSoldiers.Count != 0)
        {
            for (int i = 0; i < selectedSoldiers.Count; i++)
            {
                //Debug.Log("����ʿ��icon: " + selectedSoldiers[i].name);

                // �жϵ�ǰʿ���Ƿ��ǽ�ʿ���ͣ�������¡���ԭ����
                if (selectedSoldiers[i].name == "Swordsman(Clone)" || selectedSoldiers[i].name == "Swordsman")
                {
                    // Instantiate ������������Ԥ���壬������Ϊ selectionObjectParent ���Ӷ���
                    Instantiate(selectionObjectIcon[0], Vector3.zero, Quaternion.identity, selectionObjectParent.transform);
                }
                else if (selectedSoldiers[i].name == "Mage(Clone)" || selectedSoldiers[i].name == "Mage")
                {
                    // Instantiate ������������Ԥ���壬������Ϊ selectionObjectParent ���Ӷ���
                    Instantiate(selectionObjectIcon[1], Vector3.zero, Quaternion.identity, selectionObjectParent.transform);
                }

            }

            ShowAllButtons(true);
        }
        else
        {
            ShowAllButtons(false);
        }
    }

    /// <summary>
    /// ִ���Ҽ������߼���
    /// </summary>
    private void HandleCommandInput()
    {
        // ������Ҫ���Ĳ㼶��Grass �� Monster��
        int layerMask = LayerMask.GetMask("Grass", "Monster");
        // �Ҽ����
        if (Input.GetMouseButtonUp(1) && selectedSoldiers.Count > 0)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, layerMask))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Grass"))
                {
                    for (int i = selectedSoldiers.Count - 1; i >= 0; i--)
                    {
                        var soldier = selectedSoldiers[i];
                        if (soldier == null)
                        {
                            selectedSoldiers.RemoveAt(i); // ��ȫɾ���ն���
                        }
                        else
                        {
                            // ����Ŀ��㲢���ø�ʿ��
                            var targetPoint = new GameObject("TargetPoint");
                            targetPoint.transform.position = hit.point;
                            soldier.SetTarget(targetPoint.transform.position);
                            Destroy(targetPoint, 60f);
                            soldier.currentState = SoldierState.Jingjie;

                        }
                    }
                }
                //else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Monster"))
                //{
                //    Transform enemy = hit.collider.transform;
                //    foreach (var soldier in selectedSoldiers)
                //    {
                //        soldier.SetTarget(enemy);
                //    }
                //}
            }
        }
    }


    /// <summary>
    /// ����ѡ��򣨽����Ӿ���������
    /// </summary>
    private void OnGUI()
    {
        if (isSelecting)
        {
            // ����ѡ���
            Rect rect = new Rect(
                selectionStartPos.x,
                Screen.height - selectionStartPos.y,
                Input.mousePosition.x - selectionStartPos.x,
                -(Input.mousePosition.y - selectionStartPos.y)
            );
            GUI.color = new Color(0, 0.5f, 1, 0.25f);
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = Color.white;
        }
        isSelectedOnGUI();
    }

    /// <summary>
    /// ʹ�� OnGUI ����Ϊ selectedSoldiers �б��е�ÿ��ʿ������һ��ѡ�б�ʶ��
    /// �˷������� GUI ��Ⱦ�׶�ִ�У���ʿ������������ת��Ϊ��Ļ���꣬���ڸ�λ�û���ѡ����ͼ��
    /// </summary>
    void isSelectedOnGUI()
    {
        // ȷ��ѡ���б�Ϊ��
        if (selectedSoldiers == null || selectedSoldiers.Count == 0)
            return;

        // ���� GUIStyle �������ı���ʽ
        GUIStyle triangleStyle = new GUIStyle();
        triangleStyle.fontSize = 30; // �趨�����С
        triangleStyle.normal.textColor = Color.black; // �趨��ɫΪ��ɫ

        // �������б�ѡ�е�ʿ��
        foreach (Soldier soldier in selectedSoldiers)
        {
            if (soldier == null)
                continue;

            // ��ȡʿ�����������꣬��ת��Ϊ��Ļ����
            Vector3 screenPos = Camera.main.WorldToScreenPoint(soldier.transform.position);
            screenPos.y = Screen.height - screenPos.y; // GUI ����ԭ�������Ͻǣ���Ҫת�� y ����

            // ���㵹���ǵĻ���λ�ã�ʹ��λ��ʿ���Ϸ�
            float triangleOffsetY = 50f; // ���Ƶ����ǵĸ߶�ƫ��
            Vector2 labelPos = new Vector2(screenPos.x - 10, screenPos.y - triangleOffsetY);

            // ���Ƶ����ǣ�Unicode �ַ� "��"��
            GUI.Label(new Rect(labelPos.x, labelPos.y, 20, 20), "��", triangleStyle);
        }
    }
}
