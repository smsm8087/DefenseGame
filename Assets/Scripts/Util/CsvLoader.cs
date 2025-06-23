using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class CsvLoader
{
    public static Dictionary<int, T> Load<T>(string resourcePath) where T : new()
    {
        var dict = new Dictionary<int, T>();

        TextAsset textAsset = Resources.Load<TextAsset>(resourcePath);
        if (textAsset == null)
        {
            Debug.LogError($"[CsvLoader] CSV 파일 없음: {resourcePath}");
            return dict;
        }

        var lines = textAsset.text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length < 2)
        {
            Debug.LogError($"[CsvLoader] CSV 파일 빈 내용: {resourcePath}");
            return dict;
        }

        var headers = lines[0].Split(',');

        var type = typeof(T);
        var fields = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        for (int i = 1; i < lines.Length; i++)
        {
            var cols = lines[i].Split(',');

            if (cols.Length < headers.Length)
                continue;

            T obj = new T();

            for (int j = 0; j < headers.Length; j++)
            {
                var header = headers[j].Trim();
                var field = Array.Find(fields, f => f.Name.Equals(header, StringComparison.OrdinalIgnoreCase));

                if (field == null)
                    continue;

                try
                {
                    object value = Convert.ChangeType(cols[j], field.PropertyType);
                    field.SetValue(obj, value);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[CsvLoader] 변환 오류: {header} / 값: {cols[j]} / {ex.Message}");
                }
            }

            var keyField = Array.Find(fields, f => f.Name.Equals("CardId") || f.Name.Equals("Id"));
            if (keyField == null)
            {
                Debug.LogError($"[CsvLoader] 기본키(Id, CardId) 없음 - {typeof(T).Name}");
                continue;
            }

            int key = (int)Convert.ChangeType(keyField.GetValue(obj), typeof(int));
            dict[key] = obj;
        }

        Debug.Log($"[CsvLoader] {typeof(T).Name} → {dict.Count}개 로드 완료!");
        return dict;
    }
}
