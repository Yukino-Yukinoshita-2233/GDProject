using PathFinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    private Vector3 destination = new Vector3(15, 15, 15);
    public bool isMove = false;
    private float speed = 3;

    // Start is called before the first frame update
    void Start()
    {
        //SetPostion(destination);
    }

    // Update is called once per frame
    void Update()
    {
        //if (CreateCheckGameObject())
        //{
        //    return;
        //}

        Move();
    }

    public void SetPostion(Vector3 vector3)
    {
        isMove = true;
        destination = vector3;
        StartSearchPath();
    }

    private Stack<Position> _stackPos = new Stack<Position>();
    private void StartSearchPath()
    {
        DestroyGO();

        // 获取开始位置、终点位置
        Position from = new Position(this.transform.position.z, this.transform.position.x);
        Position to = new Position(destination.z, destination.x);

        // 搜索路径，如果返回结果为 null，则说明没有找到路径，否则说明已找到路径，且 pathNode 为终点节点
        // 顺着 pathNode 一直向上查找 parentNode，最终将到达开始点
        Node pathNode = JPSMangaer.Instance._jps.SearchPath(from, to);

        //需要通过 pathNode 逆序向上查找 parentNode
        //所以使用 栈：FILO 先进后出,存放路径点
        _stackPos.Clear();
        while (null != pathNode)
        {
            Position pos = JPSMangaer.Instance.imap.NodeToPosition(pathNode);
            // 数据入栈
            _stackPos.Push(pos);
            pathNode = pathNode.Parent;
        }
        // 顺次执行 _stackPos.Peek(); 将 路点从 栈中取出即是从 开始点到结束点的路径
    }
    Vector3 velocity;
    private void Move()
    {
        if (_stackPos.Count <= 0)
        {
            isMove = false;
            return;
        }

        Position position = _stackPos.Peek();
        Vector3 destinationPos = new Vector3(position.ColPos, 0.5f, position.RowPos);

        //Vector3 dir = destinationPos - this.transform.position;
        //this.transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);
        transform.position = Vector3.SmoothDamp(transform.position, destinationPos, ref velocity, 0.25f, speed);
        transform.LookAt(destinationPos);
        if (Vector3.Distance(this.transform.position, destinationPos) > 0.05f)
        {
            return;
        }
        _stackPos.Pop();
        CreatePathPos();

    }
    private List<GameObject> pathGoList = new List<GameObject>();
    public static List<KeyValuePair<int, Node>> checkNodeList = new List<KeyValuePair<int, Node>>();
    private bool CreateCheckGameObject()
    {
        if (checkNodeList.Count <= 0)
        {
            return false;
        }

        KeyValuePair<int, Node> kv = checkNodeList[0];
        Node node = kv.Value;
        checkNodeList.RemoveAt(0);

        Position pos = JPSMangaer.Instance.imap.NodeToPosition(node);
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
    /// 创建走过的节点
    /// </summary>
    private void CreatePathPos()
    {
        //GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        GameObject go = new GameObject();
        go.transform.localScale = Vector3.one * 0.2f;
        go.transform.position = new Vector3(this.transform.position.x, 0.5f, this.transform.position.z);
        //go.GetComponent<Renderer>().material.color = Color.red;
        pathGoList.Add(go);
    }
}
