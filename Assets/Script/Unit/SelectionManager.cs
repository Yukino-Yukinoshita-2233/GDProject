using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;
using Unity.Burst.CompilerServices;
using System;
using System.IO;
using TMPro; // 引入TextMeshPro命名空间

/// <summary>
/// 选择管理器，负责选择友军单位（士兵）并控制它们的行动。
/// </summary>
public class SelectionManager : MonoBehaviour
{
    // 被选中的单位列表
    public List<Soldier> selectedSoldiers = new List<Soldier>();
    public List<Soldier> selectedObjectIcon = new List<Soldier>();

    // 框选起始点
    private Vector2 selectionStartPos;
    private Vector2 selectionEndPos;
    private bool isSelecting = false;

    // 被选中UI
    public GameObject selectionObjectParent;
    public List<GameObject> selectionObjectIcon = new List<GameObject>();
    public List<GameObject> SoldierType = new List<GameObject>();
    public Transform soldierParent;

    // 选择按钮
    public List<Button> SoldierStateButton = new List<Button>();
    public List<Button> SoldierTypeButton = new List<Button>();

    public static Transform castle; // 城堡
    public TextMeshProUGUI castleHP;
    public BuildStyle buildStyle;

    private void Start()
    {
        castle = GameObject.Find("Building").transform.Find("Castle");

        StartCoroutine(ClearSelectionObjectChildrenNextFrame());

        // 遍历列表，给每个按钮添加监听事件
        for (int i = 0; i < SoldierStateButton.Count; i++)
        {
            int index = i; // 解决 Lambda 闭包问题
            SoldierStateButton[i].onClick.AddListener(() => OnSoldierStateButtonClick(index));
        }
        for (int i = 0; i < SoldierTypeButton.Count; i++)
        {
            int index = i; // 解决 Lambda 闭包问题
            SoldierTypeButton[i].onClick.AddListener(() => OnSoldierTypeButtonClick(index));
        }
    }
    private void Update()
    {
        HandleSelectionInput(); // 处理鼠标选择逻辑
        HandleCommandInput();   // 处理右键命令逻辑
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
    /// 按钮点击事件处理
    /// </summary>
    void OnSoldierStateButtonClick(int index)
    {
        Debug.Log($"按钮 {index} 被点击");

        // 在这里执行按钮点击后的逻辑
        if(index == 2)
        {
            for (int i = selectedSoldiers.Count - 1; i >= 0; i--)
            {
                var soldier = selectedSoldiers[i];
                if (soldier == null)
                {
                    selectedSoldiers.RemoveAt(i); // 安全删除空对象
                }
                else
                {
                    // 创建目标点并设置给士兵
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
                    selectedSoldiers.RemoveAt(i); // 安全删除空对象
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
    /// 按钮点击事件处理
    /// </summary>
    void OnSoldierTypeButtonClick(int index)
    {
        Debug.Log($"按钮 {index} 被点击");

        // 在这里执行按钮点击后的逻辑
        if (index == 0)
        {
            buildStyle = BuildStyle.JianShi;
        }
        else if (index == 1)
        {
            buildStyle = BuildStyle.FaShi;
        }

        Vector3Int Xioahao = BuildManger.Instance.GetXiaoHao(buildStyle);
        Debug.Log("消耗为 " + Xioahao);
        if (!CaiLiaoManager.Instance.GetUseCaiLiao(Xioahao))
        {
            return;
        }

        var obj = SoldierType[index];
        GameObject TarGetObj = Instantiate(obj, castle.position + new Vector3(0,1,0), Quaternion.Euler(0, 180, 0), soldierParent);
        StartCoroutine(MoveBackward(TarGetObj.transform, -2f, 1f)); // 移动 2 个单位，用 1 秒完成

    }

    /// <summary>
    /// 士兵训练后位置移动
    /// </summary>
    IEnumerator MoveBackward(Transform soldier, float distance, float duration)
    {
        Vector3 startPos = soldier.position;
        Vector3 endPos = startPos - soldier.forward * distance; // -Z 方向
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            soldier.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        soldier.position = endPos; // 确保最终位置正确
    }

    /// <summary>
    /// 处理框选和单击的鼠标输入逻辑。
    /// </summary>
    private void HandleSelectionInput()
    {

        // 开始框选
        if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject() == false)
        {
            for (int i = selectedSoldiers.Count - 1; i >= 0; i--)
            {
                var soldier = selectedSoldiers[i];
                if (soldier == null)
                {
                    selectedSoldiers.RemoveAt(i); // 安全删除空对象
                }
                else
                {
                    soldier.transform.Find("AttackRangeImage").gameObject.SetActive(false); // 关闭攻击范围显示

                }
            }

            // 清空之前的选择
            selectedSoldiers.Clear();

            selectionStartPos = Input.mousePosition;
            isSelecting = true; // 进入框选模式
        }
        // 框选结束
        if (Input.GetMouseButtonUp(0) && isSelecting)
        {
            // 获取屏幕选择框的范围
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
    /// 单击单位，选中一个士兵。
    /// </summary>
    private void SelectUnitsSingle()
    {
        // 清空之前的选择，如果需要的话，可以取消注释下面这行代码
        // selectedSoldiers.Clear();

        // 从主摄像机获取鼠标点击位置对应的射线
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // 使用 Physics.Raycast 进行射线检测，判断是否击中目标
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            // 调试输出：打印射线检测到的对象名称，便于调试确认射线命中情况
            Debug.Log("射线检测到对象: " + hitInfo.collider.gameObject.name);

            // 尝试获取射线击中对象上的 Soldier 组件
            Soldier soldier = hitInfo.collider.GetComponent<Soldier>();
            if (soldier != null)
            {
                // 如果检测到的对象有 Soldier 组件，则将该士兵加入选中列表
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
                // 如果检测到的对象没有 Soldier 组件，则输出提示信息
                Debug.Log("No soldier selected");
            }
        }
    }


    /// <summary>
    /// 框选单位，并将符合条件的士兵加入选中列表。
    /// </summary>
    private void SelectUnitsInRectangle()
    {
        // 清空之前的选择
        //selectedSoldiers.Clear();

        Rect selectionRect = new Rect(
            Mathf.Min(selectionStartPos.x, selectionEndPos.x),
            Mathf.Min(selectionStartPos.y, selectionEndPos.y),
            Mathf.Abs(selectionStartPos.x - selectionEndPos.x),
            Mathf.Abs(selectionStartPos.y - selectionEndPos.y)
        );

        // 遍历所有士兵，判断是否在选择框内
        foreach (var soldier in SoldierManager.Instance.GetAllSoldiers())
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(soldier.transform.position);
            if (selectionRect.Contains(screenPos, true))
            {
                selectedSoldiers.Add(soldier);
                soldier.transform.Find("AttackRangeImage").gameObject.SetActive(true); // 启用

            }
        }

        Debug.Log($"Selected {selectedSoldiers.Count} soldiers");
        //syncSelectionObject();

    }

    IEnumerator ClearSelectionObjectChildrenNextFrame()
    {
        // 循环直到所有子对象都被销毁
        while (selectionObjectParent.transform.childCount > 0)
        {
            //Debug.Log("销毁icon");
            Destroy(selectionObjectParent.transform.GetChild(0).gameObject);
            // 等待一帧，让 Destroy() 生效
            yield return null;
        }
        syncSelectionObject();

    }

    /// <summary>
    /// 同步被选择栏，更新被选择对象的显示图标。
    /// </summary>
    void syncSelectionObject()
    {
        //StartCoroutine(ClearSelectionObjectChildrenNextFrame());
        if (selectedSoldiers.Count != 0)
        {
            for (int i = 0; i < selectedSoldiers.Count; i++)
            {
                //Debug.Log("生成士兵icon: " + selectedSoldiers[i].name);

                // 判断当前士兵是否是剑士类型（包括克隆体和原件）
                if (selectedSoldiers[i].name == "Swordsman(Clone)" || selectedSoldiers[i].name == "Swordsman")
                {
                    // Instantiate 方法用于生成预制体，将其作为 selectionObjectParent 的子对象
                    Instantiate(selectionObjectIcon[0], Vector3.zero, Quaternion.identity, selectionObjectParent.transform);
                }
                else if (selectedSoldiers[i].name == "Mage(Clone)" || selectedSoldiers[i].name == "Mage")
                {
                    // Instantiate 方法用于生成预制体，将其作为 selectionObjectParent 的子对象
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
    /// 执行右键命令逻辑。
    /// </summary>
    private void HandleCommandInput()
    {
        // 定义需要检测的层级（Grass 和 Monster）
        int layerMask = LayerMask.GetMask("Grass", "Monster");
        // 右键点击
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
                            selectedSoldiers.RemoveAt(i); // 安全删除空对象
                        }
                        else
                        {
                            // 创建目标点并设置给士兵
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
    /// 绘制选择框（仅供视觉辅助）。
    /// </summary>
    private void OnGUI()
    {
        if (isSelecting)
        {
            // 绘制选择框
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
    /// 使用 OnGUI 方法为 selectedSoldiers 列表中的每个士兵绘制一个选中标识。
    /// 此方法将在 GUI 渲染阶段执行，将士兵的世界坐标转换为屏幕坐标，并在该位置绘制选中贴图。
    /// </summary>
    void isSelectedOnGUI()
    {
        // 确保选中列表不为空
        if (selectedSoldiers == null || selectedSoldiers.Count == 0)
            return;

        // 创建 GUIStyle 来控制文本样式
        GUIStyle triangleStyle = new GUIStyle();
        triangleStyle.fontSize = 30; // 设定字体大小
        triangleStyle.normal.textColor = Color.black; // 设定颜色为黑色

        // 遍历所有被选中的士兵
        foreach (Soldier soldier in selectedSoldiers)
        {
            if (soldier == null)
                continue;

            // 获取士兵的世界坐标，并转换为屏幕坐标
            Vector3 screenPos = Camera.main.WorldToScreenPoint(soldier.transform.position);
            screenPos.y = Screen.height - screenPos.y; // GUI 坐标原点在左上角，需要转换 y 坐标

            // 计算倒三角的绘制位置，使其位于士兵上方
            float triangleOffsetY = 50f; // 控制倒三角的高度偏移
            Vector2 labelPos = new Vector2(screenPos.x - 10, screenPos.y - triangleOffsetY);

            // 绘制倒三角（Unicode 字符 "▼"）
            GUI.Label(new Rect(labelPos.x, labelPos.y, 20, 20), "▼", triangleStyle);
        }
    }
}
