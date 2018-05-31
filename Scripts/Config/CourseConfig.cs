using System.Collections;
using System.Collections.Generic;
using LitJson;
using Snaplingo.Config;
using UnityEngine;

public class CourseConfig : IConfig {
    private static CourseConfig _instance;
    public Dictionary<int, List<int>> courseLevelTable = new Dictionary<int, List<int>>();
    public static CourseConfig Instance
    {
        get { return _instance; }
    }
    public CourseConfig()
    {
        _instance = this;
    }
    public void Fill(string jsonstr)
    {
        JsonData data = JsonMapper.ToObject(jsonstr);
        List<string> keys = new List<string>(data.Keys);
        foreach(string key in keys)
        {
            courseLevelTable.Add(int.Parse(data[key].TryGetString("courseid")),GetLevelIdList(data[key].TryGetString("levelids")));
        }
    }
    private List<int> GetLevelIdList(string str)
    {
        List<int> list = new List<int>();
        string[] strs = str.Split('|');
        for (int i = 0; i < strs.Length;i++)
        {
            list.Add(int.Parse(strs[i]));
        }
        return list;
    }
    public List<int> GetLevels(int courseId)
    {
        return courseLevelTable[courseId];
    }
    public int GetCourseIdFromLevelId(int levelId)
    {
        foreach(KeyValuePair<int,List<int>> key in courseLevelTable)
        {
            if(key.Value.Contains(levelId))
            {
                return key.Key;
            }
        }
        return -1;
    }
}
