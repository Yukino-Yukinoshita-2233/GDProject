using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LumberjackAI : MonoBehaviour
{
    private NavMeshAgent agent;        // ����AI�ƶ��ĵ����������
    public Transform baseLocation;     // ���ص�λ�ã���ľ���᷵�ز�������Դ
    public float detectionRange = 20f; // �����Դ�ķ�Χ
    public float collectionTime = 2f;  // �ɼ���Դ�����ʱ��

    private GameObject targetResource; // ��ǰĿ����Դ
    private bool hasWood = false;      // ��ʾ�Ƿ�ɼ���ľ����Դ

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        StartCoroutine(BehaviorTree()); // ��ʼִ����Ϊ��
    }

    // ��Ϊ����ѭ��
    IEnumerator BehaviorTree()
    {
        while (true)
        {
            yield return IdleOrSearchForWood(); // Ѱ��ľ����Դ
            yield return MoveToResource();      // ǰ����Դλ��
            yield return CollectWood();         // �ɼ���Դ
            yield return MoveToBase();          // ���ػ���
            yield return DeliverWood();         // ������Դ
        }
    }

    // ���л�Ѱ��ľ����Դ
    IEnumerator IdleOrSearchForWood()
    {
        // ����Ѿ���ľ����Դ��������Ѱ����Դ����Ϊ
        if (hasWood)
            yield break;

        // ���������ľ����Դ
        targetResource = FindNearestWood();
        if (targetResource == null)
        {
            // δ�ҵ�ľ�ģ�����ȴ�״̬
            Debug.Log("û���ҵ�ľ�ģ��ȴ���...");
            yield return new WaitForSeconds(1f);
        }
        yield break;
    }

    // �ƶ�����Դλ��
    IEnumerator MoveToResource()
    {
        // ���û����Դ���Ѿ���ľ�ģ��򲻽����ƶ�
        if (targetResource == null || hasWood)
            yield break;

        // ���õ��������Ŀ�ĵ�Ϊ��Դλ��
        agent.SetDestination(targetResource.transform.position);
        while (Vector3.Distance(transform.position, targetResource.transform.position) > agent.stoppingDistance)
        {
            yield return null; // �ȴ��ƶ�����Ŀ��
        }
    }

    // �ɼ�ľ��
    IEnumerator CollectWood()
    {
        // ���û����Դ���Ѿ���ľ�ģ��������ɼ�
        if (targetResource == null || hasWood)
            yield break;

        Debug.Log("�ɼ���...");
        yield return new WaitForSeconds(collectionTime); // �ȴ��ɼ����

        Destroy(targetResource); // ������Դ����
        hasWood = true;          // �����ӵ��ľ��
        Debug.Log("�ɼ����");
    }

    // ���ػ���
    IEnumerator MoveToBase()
    {
        // ���û��ľ�ģ��򲻷��ػ���
        if (!hasWood)
            yield break;

        // ���õ��������Ŀ�ĵ�Ϊ����λ��
        agent.SetDestination(baseLocation.position);
        while (Vector3.Distance(transform.position, baseLocation.position) > agent.stoppingDistance)
        {
            yield return null; // �ȴ��ƶ��������
        }
    }

    // ����ľ��
    IEnumerator DeliverWood()
    {
        // ���û��ľ�ģ�����������
        if (!hasWood)
            yield break;

        Debug.Log("����ľ��");
        hasWood = false; // ����ľ��״̬��׼���´βɼ�
        yield break;
    }

    // ���������ľ����Դ
    GameObject FindNearestWood()
    {
        GameObject[] woodResources = GameObject.FindGameObjectsWithTag("Wood"); // �������б��Ϊ "Wood" ����Դ����
        GameObject nearestWood = null;      // �����ľ�Ķ���
        float closestDistance = detectionRange; // ��ʼ���������Ϊ��ⷶΧ

        foreach (var wood in woodResources)
        {
            float distance = Vector3.Distance(transform.position, wood.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestWood = wood; // �������ľ�Ķ���
            }
        }
        return nearestWood; // �����ҵ������ľ�Ķ���
    }
}
