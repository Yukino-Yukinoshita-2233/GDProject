using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridBuildingSystem : MonoBehaviour
{
    [SerializeField] private List<BuildType> buildTypeList;
    private BuildType buildType;
    private GridXZ<GridObject> grid;
    private BuildType.Dir dir = BuildType.Dir.Down;
    private void Awake()
    {
        int gridWidth = 80;
        int gridHeight = 45;
        float cellSize = 1.0f;
        Vector3 Origin = new Vector3(-0.5f, 0, -0.5f);
        grid = new GridXZ<GridObject>(gridWidth, gridHeight, cellSize, Origin, (GridXZ<GridObject> grid, int x, int z) => new GridObject(grid, x, z));

        buildType = buildTypeList[0];
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 WorldPosition = Mouse3D.GetMouseWorldPosition();
            grid.GetXZ(WorldPosition, out int x, out int z);
            if (x == 0 && z == 0)
                return;
            
            GridObject gridObject = grid.GetGridObject(x, z);

            List<Vector2Int> gridPositionList = buildType.GetGridPositionList(new Vector2Int(x, z), dir);

            bool canBuild = true;
            foreach (var item in gridPositionList)
            {
                if (item.x <= grid.GetWidth() - 1 && item.y <= grid.GetHeight() - 1)
                {
                    if (!grid.GetGridObject(item.x, item.y).CanBuild())
                    {
                        canBuild = false;
                    }
                }
                else
                {
                    canBuild = false;
                }
            }
            if (canBuild)
            {
                Vector2Int rotationOffset = buildType.GetRotationOffset(dir);
                Vector3 buildObjWorldPosition = grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, WorldPosition.y+0.5f, rotationOffset.y) * grid.GetCellSize();

                Build build = Build.Create(buildObjWorldPosition, new Vector2Int(x, z), dir, buildType);

                foreach (var gridPosition in gridPositionList)
                {
                    grid.GetGridObject(gridPosition.x, gridPosition.y).SetBuild(build);
                }
            }
            else
            {
                Debug.Log("不能建造 "+ Mouse3D.GetMouseWorldPosition());
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            GridObject gridObject = grid.GetGridObject(Mouse3D.GetMouseWorldPosition());
            Build build = gridObject.GetBuild();
            if (build != null)
            {
                build.DestroySelf();

                List<Vector2Int> gridPositionList = build.GetGridPositionList();

                foreach (var gridPosition in gridPositionList)
                {
                    grid.GetGridObject(gridPosition.x, gridPosition.y).ClearBuild();
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            dir = BuildType.GetNextDir(dir);
            Debug.Log(dir+" "+ Mouse3D.GetMouseWorldPosition());
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) { buildType = buildTypeList[0]; }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { buildType = buildTypeList[1]; }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { buildType = buildTypeList[2]; }

    }

    public class GridObject
    {
        private GridXZ<GridObject> grid;
        private int x;
        private int z;
        private Build build;
        public GridObject(GridXZ<GridObject> grid, int x, int z)
        {
            this.grid = grid;
            this.x = x;
            this.z = z;
        }
        public void SetBuild(Build build)
        {
            this.build = build;
            grid.TriggerGridObjectChanged(x, z);
        }
        public Build GetBuild()
        {
            return build;
        }
        public bool CanBuild()
        {
            return build == null;
        }
        public override string ToString()
        {
            return x + "," + z + build;
        }

        internal void ClearBuild()
        {
            build = null;
            grid.TriggerGridObjectChanged(x, z);
        }
    }
}
