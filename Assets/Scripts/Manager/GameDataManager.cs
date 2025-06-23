using System;
using System.Collections.Generic;
using DataModels;
using UnityEngine;

public class GameDataManager
{
    public static GameDataManager Instance { get; } = new GameDataManager();

    private Dictionary<string, object> _tableDict = new();

    public void LoadAllData()
    {
        //table 추가시에 여기다가 작업
        _tableDict["card"] = CsvLoader.Load<CardData>("DataExcels/card_data");
        Dictionary<int, CardData> dataDict = (Dictionary<int, CardData>)_tableDict["card"];
        foreach (var cardData in dataDict.Values )
        {
            Debug.Log($"data 불러옴, {cardData.id} | {cardData.title}");
        }
        Debug.Log("<color=green>[GameDataManager] 모든 데이터 로드 완료!</color>");
    }

    public Dictionary<int, T> GetTable<T>(string tableName)
    {
        if (!_tableDict.TryGetValue(tableName, out var tableObj))
        {
            Debug.LogError($"[GameDataManager] 테이블 {tableName} 없음");
            return null;
        }

        var table = tableObj as Dictionary<int, T>;

        if (table == null)
        {
            Debug.LogError($"[GameDataManager] 테이블 {tableName} 타입 오류");
            return null;
        }

        return table;
    }

    public T GetData<T>(string tableName, int id) where T : class
    {
        var table = GetTable<T>(tableName);

        if (table == null)
            return null;

        if (table.TryGetValue(id, out var data))
            return data;

        Debug.LogWarning($"[GameDataManager] 테이블 {tableName} → ID {id} 없음");
        return null;
    }
}
