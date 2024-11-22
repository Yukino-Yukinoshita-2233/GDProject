using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ѡ�������������ѡ���Ѿ���λ��ʿ�������������ǵ��ж���
/// </summary>
public class SelectionManager : MonoBehaviour
{
    // ��ѡ�еĵ�λ�б�
    private List<Soldier> selectedSoldiers = new List<Soldier>();

    // ��ѡ��ʼ��
    private Vector2 selectionStartPos;
    private bool isSelecting = false;

    private void Update()
    {
        HandleSelectionInput(); // ��������ѡ�߼�
        HandleCommandInput();   // �����Ҽ������߼�
    }

    /// <summary>
    /// �����ѡ����������߼���
    /// </summary>
    private void HandleSelectionInput()
    {
        // ��ʼ��ѡ
        if (Input.GetMouseButtonDown(0))
        {
            isSelecting = true;
            selectionStartPos = Input.mousePosition;
        }

        // ������ѡ
        if (Input.GetMouseButtonUp(0))
        {
            isSelecting = false;
            SelectUnitsInRectangle();
        }
    }

    /// <summary>
    /// ִ���Ҽ������߼���
    /// </summary>
    private void HandleCommandInput()
    {
        // �Ҽ����
        if (Input.GetMouseButtonDown(1) && selectedSoldiers.Count > 0)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Grass"))
                {
                    // ��������ѡ��ʿ����Ŀ��Ϊ���λ��
                    foreach (var soldier in selectedSoldiers)
                    {
                        soldier.SetTarget(new GameObject("TargetPoint") { transform = { position = hit.point } }.transform);
                    }
                }
                else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Monster"))
                {
                    // ��������ѡ��ʿ����Ŀ��Ϊ����
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
    /// ��ѡ��λ����������������ʿ������ѡ���б�
    /// </summary>
    private void SelectUnitsInRectangle()
    {
        // ���֮ǰ��ѡ��
        selectedSoldiers.Clear();

        // ��ȡ��Ļѡ���ķ�Χ
        Vector2 selectionEndPos = Input.mousePosition;
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
            }
        }

        Debug.Log($"Selected {selectedSoldiers.Count} soldiers");
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
    }
}
