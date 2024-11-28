using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 选择管理器，负责选择友军单位（士兵）并控制它们的行动。
/// </summary>
public class SelectionManager : MonoBehaviour
{
    // 被选中的单位列表
    private List<Soldier> selectedSoldiers = new List<Soldier>();

    // 框选起始点
    private Vector2 selectionStartPos;
    private bool isSelecting = false;

    private void Update()
    {
        HandleSelectionInput(); // 处理鼠标框选逻辑
        HandleCommandInput();   // 处理右键命令逻辑
    }

    /// <summary>
    /// 处理框选的鼠标输入逻辑。
    /// </summary>
    private void HandleSelectionInput()
    {
        // 开始框选
        if (Input.GetMouseButtonDown(0))
        {
            isSelecting = true;
            selectionStartPos = Input.mousePosition;
        }

        // 结束框选
        if (Input.GetMouseButtonUp(0))
        {
            isSelecting = false;
            SelectUnitsInRectangle();
        }
    }

    /// <summary>
    /// 执行右键命令逻辑。
    /// </summary>
    private void HandleCommandInput()
    {
        // 右键点击
        if (Input.GetMouseButtonDown(1) && selectedSoldiers.Count > 0)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Grass"))
                {
                    // 设置所有选中士兵的目标为点击位置
                    foreach (var soldier in selectedSoldiers)
                    {
                        soldier.SetTarget(new GameObject("TargetPoint") { transform = { position = hit.point } }.transform);
                    }
                }
                else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Monster"))
                {
                    // 设置所有选中士兵的目标为敌人
                    Transform enemy = hit.collider.transform;
                    foreach (var soldier in selectedSoldiers)
                    {
                        soldier.SetTarget(enemy);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 框选单位，并将符合条件的士兵加入选中列表。
    /// </summary>
    private void SelectUnitsInRectangle()
    {
        // 清空之前的选择
        selectedSoldiers.Clear();

        // 获取屏幕选择框的范围
        Vector2 selectionEndPos = Input.mousePosition;
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
            }
        }

        Debug.Log($"Selected {selectedSoldiers.Count} soldiers");
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
    }
}
