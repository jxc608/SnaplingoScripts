using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

[System.Serializable]
public class PlayerReadingData  
{
    public string standardAnswer;
    public string playerAnswer;
    public float accuracy;
    public int score;

    public JsonData GetJsonData()
    {
        JsonData data = new JsonData();

        data["standardAnswer"] = standardAnswer;
        data["playerAnswer"] = playerAnswer;
        data["accuracy"] = accuracy;
        data["score"] = score;

        return data;
    }

    public static PlayerReadingData GetReadingDataFromJsonData(JsonData jsonData)
    {
        PlayerReadingData data = new PlayerReadingData();

        data.standardAnswer = jsonData.TryGetString("standardAnswer");
        data.playerAnswer = jsonData.TryGetString("playerAnswer");
        data.accuracy = float.Parse(jsonData.TryGetString("accuracy"));
        data.score = int.Parse(jsonData.TryGetString("score"));

        return data;
    }
}


public class PlayerOperationData
{
    public int wholeScore;
    public float clickAccuracy;
    public int clickScore;
    public int clickNumber;
    public int rightNumber;
    public int wrongNumber;

    public List<PlayerReadingData> m_ReadingData = new List<PlayerReadingData>();

    public string GetJson()
    {
        JsonData data = new JsonData();

        data["clickAccuracy"] = clickAccuracy;
        data["clickScore"] = clickScore;
        data["clickNumber"] = clickNumber;
        data["rightNumber"] = rightNumber;
        data["wrongNumber"] = wrongNumber;
        data["wholeScore"] = wholeScore;

        JsonData readingData = new JsonData();

        for (int i = 0; i < m_ReadingData.Count; i++)
        {
            readingData.Add(m_ReadingData[i].GetJsonData());
        }

        data["readingData"] = readingData;

        return data.ToJson();
    }

    public static PlayerOperationData GetDataFromJson(string json)
    {
        PlayerOperationData data = new PlayerOperationData();

        JsonData jsonData = JsonMapper.ToObject(json);

        data.clickAccuracy = float.Parse(jsonData.TryGetString("clickAccuracy"));
        data.clickScore = int.Parse(jsonData.TryGetString("clickScore"));
        data.clickNumber = int.Parse(jsonData.TryGetString("clickNumber"));
        data.clickAccuracy = int.Parse(jsonData.TryGetString("rightNumber"));
        data.wrongNumber = int.Parse(jsonData.TryGetString("wrongNumber"));
        data.wholeScore = int.Parse(jsonData.TryGetString("wholeScore"));

        JsonData readingData = jsonData["readingData"];

        for (int i = 0; i < readingData.Count; i++)
        {
            data.m_ReadingData.Add(PlayerReadingData.GetReadingDataFromJsonData(readingData[i]));
        }

        return data;
    }
}