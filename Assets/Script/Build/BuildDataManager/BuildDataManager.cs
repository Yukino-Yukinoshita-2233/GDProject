using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Excel;
using UnityEngine;


public class BuildDataManager : SingletonMono<BuildDataManager>
{
    public string filePath = Application.dataPath + "/Script/Build/BuildDataManager/BuildData.xlsx"; // 替换为你的Excel文件路径

    public Dictionary<int, BuildUpLvData> BuildUpLvDatas=new Dictionary<int, BuildUpLvData>();

    void Start()
    {
        DataRowCollection _dataRowCollection = ReadExcel(filePath);
        int i = 0;
        // 遍历每一行数据
        foreach (DataRow row in _dataRowCollection)
        {
            if (i == 0)
            {
                i++;
                continue;
            }
            BuildUpLvData buildUpLvData = new BuildUpLvData();
            string[] parts = row[2].ToString().Split(';');
            buildUpLvData.upgradeResources = new Vector3Int( int.Parse(parts[0].ToString()),
            int.Parse(parts[1].ToString()),
            int.Parse(parts[2].ToString()));
            buildUpLvData.storageLimit = int.Parse(row[3].ToString());
            buildUpLvData.upgradedStorageLimit = int.Parse(row[4].ToString());

            BuildUpLvDatas.Add(int.Parse(row[0].ToString()), buildUpLvData);
        }
    }

    //通过表的索引，返回一个DataRowCollection表数据对象
    private DataRowCollection ReadExcel(string _path, int _sheetIndex = 0)
    {
        FileStream stream = File.Open(_path, FileMode.Open, FileAccess.Read, FileShare.Read);
        IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
        DataSet result = excelReader.AsDataSet();
        return result.Tables[_sheetIndex].Rows;
    }

    //通过表的名字，返回一个DataRowCollection表数据对象
    private DataRowCollection ReadExcel(string _path, string _sheetName)
    {
        FileStream stream = File.Open(_path, FileMode.Open, FileAccess.Read, FileShare.Read);
        IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
        DataSet result = excelReader.AsDataSet();
        return result.Tables[_sheetName].Rows;
    }
}

public class BuildUpLvData
{
    public Vector3Int upgradeResources;//木材石材金属
    public int storageLimit;//升级之前存储
    public int upgradedStorageLimit;//升级之后存储
}
