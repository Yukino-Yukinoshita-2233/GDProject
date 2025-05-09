using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathFinding;

public class MineJpsTest : MonoBehaviour
{
    public GameObject personGo;
    public GameObject destination;

    private IMap imap;
    private JPS _jps;
    private float speed = 3;
    private void Start()
    {
        // ��ȡ��ͼ����
        //imap = QuadMapCreate.CreateQuad("Terrain7", 0, 0, 10, 20);
        imap = new MapQuad(0, 0, 45,80 , 1.0f, 1.0f);
        // ��ʼ�� �㷨��������ͼ���ݴ��ݽ�ȥ
        _jps = new JPS(imap);

        new MapToolsDrawNode(imap);
        //CreatePerson();
    }

    private Stack<Position> _stackPos = new Stack<Position>();
    /// <summary>
    /// ����·��
    /// </summary>
    private void StartSearchPath()
    {
        DestroyGO();

        // ��ȡ��ʼλ�á��յ�λ��
        Position from = new Position(personGo.transform.position.z, personGo.transform.position.x);
        Position to = new Position(destination.transform.position.z, destination.transform.position.x);

        // ����·����������ؽ��Ϊ null����˵��û���ҵ�·��������˵�����ҵ�·������ pathNode Ϊ�յ�ڵ�
        // ˳�� pathNode һֱ���ϲ��� parentNode�����ս����￪ʼ��
        Node pathNode = _jps.SearchPath(from, to);

        //��Ҫͨ�� pathNode �������ϲ��� parentNode
        //����ʹ�� ջ��FILO �Ƚ����,���·����
        _stackPos.Clear();
        while (null != pathNode)
        {
            Position pos = imap.NodeToPosition(pathNode);
            // ������ջ
            _stackPos.Push(pos);
            pathNode = pathNode.Parent;
        }
        // ˳��ִ�� _stackPos.Peek(); �� ·��� ջ��ȡ�����Ǵ� ��ʼ�㵽�������·��
    }

    private void Move()
    {
        if (_stackPos.Count <= 0)
        {
            return;
        }

        Position position = _stackPos.Peek();
        Vector3 destinationPos = new Vector3(position.ColPos, 0.3f, position.RowPos);
        Vector3 dir = destinationPos - personGo.transform.position;
        personGo.transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);
        if (Vector3.Distance(personGo.transform.position, destinationPos) > 0.05f)
        {
            return;
        }
        _stackPos.Pop();
        CreatePathPos();
    }

    #region DebugUse
    private List<GameObject> pathGoList = new List<GameObject>();
    public static List<KeyValuePair<int, Node>> checkNodeList = new List<KeyValuePair<int, Node>>();
    private void Update()
    {
        if (CreateCheckGameObject())
        {
            return;
        }

        Move();
    }

    private void OnGUI()
    {
        GUI.skin.button.fontSize = 40;
        if (GUI.Button(new Rect(10, 10, 200, 80), "Start"))
        {
            StartSearchPath();
        }

        // JPS+ ��û����ɣ�����ʹ��
        if (!_jps.IsPreprocess)
        {
            if (GUI.Button(new Rect(300, 10, 400, 80), "JPS+ Ԥ�����ͼ"))
            {
                Debug.LogError("��δ��ɣ����ܻ�����ʹ��");
                _jps.Preprocess();
            }
        }
    }

    /// <summary>
    /// �������뵽 OpenList ��Ľڵ�
    /// �ʹ� OpenList ��ȡ�����Ľڵ�
    /// </summary>
    /// <returns></returns>
    private bool CreateCheckGameObject()
    {
        if (checkNodeList.Count <= 0)
        {
            return false;
        }

        KeyValuePair<int, Node> kv = checkNodeList[0];
        Node node = kv.Value;
        checkNodeList.RemoveAt(0);

        Position pos = imap.NodeToPosition(node);
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        go.transform.position = (kv.Key == 1) ? new Vector3(pos.ColPos + 0.1f, 0f, pos.RowPos + 0.1f) : new Vector3(pos.ColPos - 0.1f, 0f, pos.RowPos - 0.1f);
        go.transform.localScale = Vector3.one * 0.3f;
        go.name = (kv.Key == 1) ? string.Format("open:{0}_{1}", node.Row, node.Col) : string.Format("insertOpen:{0}_{1}", node.Row, node.Col);
        go.GetComponent<Renderer>().material.color = (kv.Key == 1) ? Color.green : Color.blue;
        pathGoList.Add(go);

        return true;
    }

    private void DestroyGO()
    {
        for (int i = pathGoList.Count - 1; i >= 0; --i)
        {
            GameObject.Destroy(pathGoList[i]);
        }
        pathGoList.Clear();
    }

    /// <summary>
    /// �����߹��Ľڵ�
    /// </summary>
    private void CreatePathPos()
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.transform.localScale = Vector3.one * 0.2f;
        go.transform.position = new Vector3(personGo.transform.position.x, 0.6f, personGo.transform.position.z);
        go.GetComponent<Renderer>().material.color = Color.red;
        pathGoList.Add(go);
    }
    //private GameObject personGo;
    //private GameObject destination;
    //private void CreatePerson()
    //{
    //    // ��ɫ������
    //    personGo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    //    personGo.name = "Person";
    //    personGo.transform.position = new Vector3(2.5f, 0.3f, 3.85f);
    //    personGo.GetComponent<Renderer>().material.color = Color.green;

    //    // Ŀ���յ�
    //    destination = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    //    destination.name = "Destination";
    //    destination.transform.position = new Vector3(10.5f, 0.3f, 4f);
    //    destination.GetComponent<Renderer>().material.color = Color.black;
    //}

    #endregion
}
