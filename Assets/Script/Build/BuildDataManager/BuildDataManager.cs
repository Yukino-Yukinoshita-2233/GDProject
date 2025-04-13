using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Excel;
using UnityEngine;


public class BuildDataManager : SingletonMono<BuildDataManager>
{
    public string filePath = Application.dataPath + "/Script/Build/BuildDataManager/BuildData.xlsx"; // �滻Ϊ���Excel�ļ�·��

    public Dictionary<int, BuildUpLvData> BuildUpLvDatas=new Dictionary<int, BuildUpLvData>();

    void Start()
    {
        DataRowCollection _dataRowCollection = ReadExcel(filePath);
        int i = 0;
        // ����ÿһ������
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

    //ͨ���������������һ��DataRowCollection�����ݶ���
    private DataRowCollection ReadExcel(string _path, int _sheetIndex = 0)
    {
        FileStream stream = File.Open(_path, FileMode.Open, FileAccess.Read, FileShare.Read);
        IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
        DataSet result = excelReader.AsDataSet();
        return result.Tables[_sheetIndex].Rows;
    }

    //ͨ��������֣�����һ��DataRowCollection�����ݶ���
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
    public Vector3Int upgradeResources;//ľ��ʯ�Ľ���
    public int storageLimit;//����֮ǰ�洢
    public int upgradedStorageLimit;//����֮��洢
}
