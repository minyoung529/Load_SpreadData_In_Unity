using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ReadSpreadSheet : MonoBehaviour
{
    public readonly string ADDRESS = "https://docs.google.com/spreadsheets/d/1AG7XW-6r4R6yC33K1mVNCb4HWHe-bgtjbCOTtAn97DU";
    public readonly string RANGE = "A2:D";
    public readonly long SHEET_ID = 1191946581;

    public List<Animal> animals;

    // key      > 스프   레드 시트 주제
    // value    > 스프레드시트 데이터 (처음엔 링크)
    private Dictionary<Type, string> sheetDatas = new Dictionary<Type, string>();
    public bool IsLoading { get; private set; } = true;

    public void Awake()
    {
        // --------------------- TEST CODE -----------------------------
        sheetDatas.Add(typeof(Animal), GetTSVAddress(ADDRESS, RANGE, SHEET_ID));
        // --------------------- TEST CODE -----------------------------
    }

    private void Start()
    {
        StartCoroutine(LoadData());
    }

    public IEnumerator LoadData()
    {
        List<Type> sheetTypes = new List<Type>(sheetDatas.Keys);

        foreach (Type type in sheetTypes)
        {
            UnityWebRequest www = UnityWebRequest.Get(sheetDatas[type]);
            yield return www.SendWebRequest();

            sheetDatas[type] = www.downloadHandler.text;

            // --------------------- TEST CODE -----------------------------
            if (type == typeof(Animal))
            {
                animals = GetDatasAsChildren<Animal>(sheetDatas[type]);

                foreach (Animal a in animals)
                {
                    a.Hello();
                }
            }
            // --------------------- TEST CODE -----------------------------
        }
    }

    public static string GetTSVAddress(string address, string range, long sheetID)
    {
        return $"{address}/export?format=tsv&range={range}&gid={sheetID}";
    }

    List<T> GetDatas<T>(string data)
    {
        List<T> returnList = new List<T>();
        string[] splitedData = data.Split('\n');

        foreach (string element in splitedData)
        {
            string[] datas = element.Split('\t');
            returnList.Add(GetData<T>(datas));
        }

        return returnList;
    }

    List<T> GetDatasAsChildren<T>(string data)
    {
        List<T> returnList = new List<T>();
        string[] splitedData = data.Split('\n');

        foreach (string element in splitedData)
        {
            string[] datas = element.Split('\t');
            returnList.Add(GetData<T>(datas, datas[0]));
        }

        return returnList;
    }

    T GetData<T>(string[] datas, string childType = "")
    {
        object data;

        // childType이 비어있거나 그런 Type이 없을 때
        if (string.IsNullOrEmpty(childType) || Type.GetType(childType) == null)
        {
            data = Activator.CreateInstance(typeof(T));
        }
        else
        {
            data = Activator.CreateInstance(Type.GetType(childType));
        }

        // 클래스에 있는 변수들을 순서대로 저장한 배열
        FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        for (int i = 0; i < datas.Length; i++)
        {
            try
            {
                // string > parse
                Type type = fields[i].FieldType;

                if (string.IsNullOrEmpty(datas[i])) continue;

                if (type == typeof(int))
                    fields[i].SetValue(data, int.Parse(datas[i]));

                else if (type == typeof(float))
                    fields[i].SetValue(data, float.Parse(datas[i]));

                else if (type == typeof(bool))
                    fields[i].SetValue(data, bool.Parse(datas[i]));

                else if (type == typeof(string))
                    fields[i].SetValue(data, datas[i]);

                // enum
                else
                    fields[i].SetValue(data, Enum.Parse(type, datas[i]));
            }

            catch (Exception e)
            {
                Debug.LogError($"SpreadSheet Error : {e.Message}");
            }
        }

        return (T)data;
    }
}
