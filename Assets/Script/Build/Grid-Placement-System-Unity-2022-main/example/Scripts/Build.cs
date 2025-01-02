using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Build : MonoBehaviour
{
    [SerializeField] private BuildType buildType;
    [SerializeField] private Vector2Int origin;
    [SerializeField] private BuildType.Dir dir;

    public static Build Create(Vector3 pos, Vector2Int origin, BuildType.Dir dir, BuildType buildType)
    {
        Transform buildTrans = Instantiate(buildType.prefab, pos, Quaternion.Euler(0, buildType.GetRotationAngle(dir), 0));

        Build build = buildTrans.GetComponent<Build>();

        build.buildType = buildType;
        build.origin = origin;
        build.dir = dir;

        return build;
    }

    public List<Vector2Int> GetGridPositionList()
    {
        return buildType.GetGridPositionList(origin, dir);
    }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
