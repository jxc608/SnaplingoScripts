using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.Text;

public class ChoreographerData
{
    public enum DanceType { Fever, Medium, Slow }

    public DancerInfo m_MiddleDancer;
    public DancerInfo m_RightDancer;
    public DancerInfo m_LeftDancer;

    public bool HasPlayerSelf;
    public bool ChampionStage;

    public int StartSpellIndex;

    public string ToJson()
    {
        JsonData data = new JsonData();

      
        JsonData middleDancer = m_MiddleDancer.GetJsonData();
        JsonData rightDancer = m_RightDancer.GetJsonData();
        JsonData leftDancer = m_LeftDancer.GetJsonData();

        data["startSpellIndex"] = StartSpellIndex;
        data["middle"] = middleDancer;
        data["right"] = rightDancer;
        data["left"] = leftDancer;

        return data.ToJson();
    }

    public static ChoreographerData GetChoreographerDataFromJson(string json)
    {
        ChoreographerData cd = new ChoreographerData();

        JsonData data = JsonMapper.ToObject(json);
        cd.StartSpellIndex = int.Parse(data.TryGetString("startSpellIndex","-1"));

        JsonData middleDancer = data["middle"];
        JsonData leftDancer = data["left"];
        JsonData rightDancer = data["right"];

        DancerInfo middle = new DancerInfo();
        DancerInfo left = new DancerInfo();
        DancerInfo right = new DancerInfo();

        middle.Name = middleDancer.TryGetString("name"); 
        middle.ModelID = middleDancer.TryGetString("modelID"); 
        middle.FaceID = middleDancer.TryGetString("faceID"); 
        middle.m_Country = (DancerInfo.Country)int.Parse(middleDancer.TryGetString("country")) ;
        middle.PlayerID = middleDancer.TryGetString("uid");
        middle.m_RankingType = (DancerInfo.RankingType)int.Parse(middleDancer.TryGetString("ranking"));

        left.Name = leftDancer.TryGetString("name"); 
        left.ModelID = leftDancer.TryGetString("modelID"); 
        left.FaceID = leftDancer.TryGetString("faceID"); 
        left.m_Country = (DancerInfo.Country)int.Parse(leftDancer.TryGetString("country")) ;
        left.PlayerID = leftDancer.TryGetString("uid");
        left.m_RankingType = (DancerInfo.RankingType)int.Parse(leftDancer.TryGetString("ranking"));

        right.Name = rightDancer.TryGetString("name"); 
        right.ModelID = rightDancer.TryGetString("modelID"); 
        right.FaceID = rightDancer.TryGetString("faceID"); 
        right.m_Country = (DancerInfo.Country)int.Parse(rightDancer.TryGetString("country")) ;
        right.PlayerID = rightDancer.TryGetString("uid");
        right.m_RankingType = (DancerInfo.RankingType)int.Parse(rightDancer.TryGetString("ranking"));

        string[] middleActionList = middleDancer.TryGetString("actions").Split(',');
        string[] rightActionList = rightDancer.TryGetString("actions").Split(',');
        string[] leftActionList = leftDancer.TryGetString("actions").Split(',');

        for (int i = 0; i < middleActionList.Length; i++)
        {
            string[] array = middleActionList[i].Split('|');
            int beatNumber = int.Parse(array[0]);
            switch(array[1])
            {
                case "a":
                    DanceActionData attackData = new DanceActionData("",0,beatNumber);
                    attackData.m_AnimationType = DanceActionData.AnimationType.Attack;
                    switch(array[2])
                    {
                        case "lr":
                            attackData.m_AttackDir = DanceActionData.AttackDir.FromLeftToRight;
                            break;
                        case "rl":
                            attackData.m_AttackDir = DanceActionData.AttackDir.FromRightToLeft;
                            break;
                        case "rm":
                            attackData.m_AttackDir = DanceActionData.AttackDir.FromRightToMiddle;
                            break;
                        case "mr":
                            attackData.m_AttackDir = DanceActionData.AttackDir.FromMiddleToRight;
                            break;
                        case "lm":
                            attackData.m_AttackDir = DanceActionData.AttackDir.FromLeftToMiddle;
                            break;
                        case "ml":
                            attackData.m_AttackDir = DanceActionData.AttackDir.FromMiddleToLeft;
                            break;
                    }
                    middle.m_DanceActionList.Add(attackData);
                    break;
                case "b": 
                    DanceActionData beatenkData = new DanceActionData("", 0, beatNumber);
                    beatenkData.m_AnimationType = DanceActionData.AnimationType.Beaten;
                    switch (array[2])
                    {
                        case "lr":
                            beatenkData.m_AttackDir = DanceActionData.AttackDir.FromLeftToRight;
                            break;
                        case "rl":
                            beatenkData.m_AttackDir = DanceActionData.AttackDir.FromRightToLeft;
                            break;
                        case "rm":
                            beatenkData.m_AttackDir = DanceActionData.AttackDir.FromRightToMiddle;
                            break;
                        case "mr":
                            beatenkData.m_AttackDir = DanceActionData.AttackDir.FromMiddleToRight;
                            break;
                        case "lm":
                            beatenkData.m_AttackDir = DanceActionData.AttackDir.FromLeftToMiddle;
                            break;
                        case "ml":
                            beatenkData.m_AttackDir = DanceActionData.AttackDir.FromMiddleToLeft;
                            break;
                    }
                    middle.m_DanceActionList.Add(beatenkData);
                    break;
                case "d": 
                    int animationID = int.Parse(array[2]);
                    DanceActionData danceData = new DanceActionData(StageManager.AnimationGroup[animationID], animationID, beatNumber);
                    danceData.m_AnimationType = DanceActionData.AnimationType.Dance;
                    middle.m_DanceActionList.Add(danceData);
                    break;
            }
        }

        for (int i = 0; i < rightActionList.Length; i++)
        {
            string[] array = rightActionList[i].Split('|');
            int beatNumber = int.Parse(array[0]);
            switch (array[1])
            {
                case "a":
                    DanceActionData attackData = new DanceActionData("", 0, beatNumber);
                    attackData.m_AnimationType = DanceActionData.AnimationType.Attack;
                    switch (array[2])
                    {
                        case "lr":
                            attackData.m_AttackDir = DanceActionData.AttackDir.FromLeftToRight;
                            break;
                        case "rl":
                            attackData.m_AttackDir = DanceActionData.AttackDir.FromRightToLeft;
                            break;
                        case "rm":
                            attackData.m_AttackDir = DanceActionData.AttackDir.FromRightToMiddle;
                            break;
                        case "mr":
                            attackData.m_AttackDir = DanceActionData.AttackDir.FromMiddleToRight;
                            break;
                        case "lm":
                            attackData.m_AttackDir = DanceActionData.AttackDir.FromLeftToMiddle;
                            break;
                        case "ml":
                            attackData.m_AttackDir = DanceActionData.AttackDir.FromMiddleToLeft;
                            break;
                    }
                    right.m_DanceActionList.Add(attackData);
                    break;
                case "b":
                    DanceActionData beatenkData = new DanceActionData("", 0, beatNumber);
                    beatenkData.m_AnimationType = DanceActionData.AnimationType.Beaten;
                    switch (array[2])
                    {
                        case "lr":
                            beatenkData.m_AttackDir = DanceActionData.AttackDir.FromLeftToRight;
                            break;
                        case "rl":
                            beatenkData.m_AttackDir = DanceActionData.AttackDir.FromRightToLeft;
                            break;
                        case "rm":
                            beatenkData.m_AttackDir = DanceActionData.AttackDir.FromRightToMiddle;
                            break;
                        case "mr":
                            beatenkData.m_AttackDir = DanceActionData.AttackDir.FromMiddleToRight;
                            break;
                        case "lm":
                            beatenkData.m_AttackDir = DanceActionData.AttackDir.FromLeftToMiddle;
                            break;
                        case "ml":
                            beatenkData.m_AttackDir = DanceActionData.AttackDir.FromMiddleToLeft;
                            break;
                    }
                    right.m_DanceActionList.Add(beatenkData);
                    break;
                case "d":
                    int animationID = int.Parse(array[2]);
                    DanceActionData danceData = new DanceActionData(StageManager.AnimationGroup[animationID], animationID, beatNumber);
                    danceData.m_AnimationType = DanceActionData.AnimationType.Dance;
                    right.m_DanceActionList.Add(danceData);
                    break;
            }
        }

        for (int i = 0; i < leftActionList.Length; i++)
        {
            string[] array = leftActionList[i].Split('|');
            int beatNumber = int.Parse(array[0]);
            switch (array[1])
            {
                case "a":
                    DanceActionData attackData = new DanceActionData("", 0, beatNumber);
                    attackData.m_AnimationType = DanceActionData.AnimationType.Attack;
                    switch (array[2])
                    {
                        case "lr":
                            attackData.m_AttackDir = DanceActionData.AttackDir.FromLeftToRight;
                            break;
                        case "rl":
                            attackData.m_AttackDir = DanceActionData.AttackDir.FromRightToLeft;
                            break;
                        case "rm":
                            attackData.m_AttackDir = DanceActionData.AttackDir.FromRightToMiddle;
                            break;
                        case "mr":
                            attackData.m_AttackDir = DanceActionData.AttackDir.FromMiddleToRight;
                            break;
                        case "lm":
                            attackData.m_AttackDir = DanceActionData.AttackDir.FromLeftToMiddle;
                            break;
                        case "ml":
                            attackData.m_AttackDir = DanceActionData.AttackDir.FromMiddleToLeft;
                            break;
                    }
                    left.m_DanceActionList.Add(attackData);
                    break;
                case "b":
                    DanceActionData beatenkData = new DanceActionData("", 0, beatNumber);
                    beatenkData.m_AnimationType = DanceActionData.AnimationType.Beaten;
                    switch (array[2])
                    {
                        case "lr":
                            beatenkData.m_AttackDir = DanceActionData.AttackDir.FromLeftToRight;
                            break;
                        case "rl":
                            beatenkData.m_AttackDir = DanceActionData.AttackDir.FromRightToLeft;
                            break;
                        case "rm":
                            beatenkData.m_AttackDir = DanceActionData.AttackDir.FromRightToMiddle;
                            break;
                        case "mr":
                            beatenkData.m_AttackDir = DanceActionData.AttackDir.FromMiddleToRight;
                            break;
                        case "lm":
                            beatenkData.m_AttackDir = DanceActionData.AttackDir.FromLeftToMiddle;
                            break;
                        case "ml":
                            beatenkData.m_AttackDir = DanceActionData.AttackDir.FromMiddleToLeft;
                            break;
                    }
                    left.m_DanceActionList.Add(beatenkData);
                    break;
                case "d":
                    int animationID = int.Parse(array[2]);
                    int beatCounts = 1;
                    if(array.Length == 4)
                    {
                        beatCounts = int.Parse(array[3]);
                    }
                    DanceActionData danceData = new DanceActionData(StageManager.AnimationGroup[animationID], animationID, beatNumber);
                    danceData.AnimationBeatCount = beatCounts;
                    danceData.m_AnimationType = DanceActionData.AnimationType.Dance;
                    left.m_DanceActionList.Add(danceData);
                    break;
            }
        }

        cd.m_MiddleDancer = middle;
        cd.m_LeftDancer = left;
        cd.m_RightDancer = right;

        return cd;
    }
}


public class DancerInfo
{
    public enum Country {China, America}
    public enum Sex {Male, Female}
    public enum RankingType { Winner, Loser }

    public Country m_Country;
    public Sex m_Sex;
    public RankingType m_RankingType;

    public float ClickScorePercent;//0~1
    public float VoiceScorePercent;//0~1
    public float WholeRankingPercent;//0~1
    public int ClickScore;
    public int VoiceScore;
    public int WholeScore;
    public string ModelID;
    public string FaceID;
    public string Name;
    public string PlayerID;

    public List<DanceActionData> m_DanceActionList = new List<DanceActionData>();

    public JsonData GetJsonData()
    {
        JsonData data = new JsonData();

        data["modelID"] = ModelID;
        data["faceID"] = FaceID;
        data["name"] = Name;
        data["country"] = (int)m_Country;
        data["uid"] = PlayerID;
        data["ranking"] = (int)m_RankingType;

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < m_DanceActionList.Count; i++)
        {
            sb.Append(m_DanceActionList[i].StartBeat.ToString() + "|");
            switch(m_DanceActionList[i].m_AnimationType)
            {
                case DanceActionData.AnimationType.Attack:
                    sb.Append("a|");
                    switch(m_DanceActionList[i].m_AttackDir)
                    {
                        case DanceActionData.AttackDir.FromLeftToMiddle: sb.Append("fm"); break;
                        case DanceActionData.AttackDir.FromRightToLeft: sb.Append("rl"); break;
                        case DanceActionData.AttackDir.FromLeftToRight: sb.Append("lr"); break;
                        case DanceActionData.AttackDir.FromMiddleToLeft: sb.Append("ml");  break;
                        case DanceActionData.AttackDir.FromMiddleToRight: sb.Append("mr"); break;
                        case DanceActionData.AttackDir.FromRightToMiddle: sb.Append("rm"); break;
                    }
                    break;
                case DanceActionData.AnimationType.Beaten:
                    sb.Append("b|");
                    switch (m_DanceActionList[i].m_AttackDir)
                    {
                        case DanceActionData.AttackDir.FromLeftToMiddle: sb.Append("fm"); break;
                        case DanceActionData.AttackDir.FromRightToLeft: sb.Append("rl"); break;
                        case DanceActionData.AttackDir.FromLeftToRight: sb.Append("lr"); break;
                        case DanceActionData.AttackDir.FromMiddleToLeft: sb.Append("ml"); break;
                        case DanceActionData.AttackDir.FromMiddleToRight: sb.Append("mr"); break;
                        case DanceActionData.AttackDir.FromRightToMiddle: sb.Append("rm"); break;
                    }
                    break;
                case DanceActionData.AnimationType.Dance:
                    sb.Append("d|" + m_DanceActionList[i].AnimationID.ToString() + "|" + m_DanceActionList[i].AnimationBeatCount.ToString());
                    break;
            }

            if(i != m_DanceActionList.Count - 1)
            {
                sb.Append(",");
            }
        }

        data["actions"] = sb.ToString();

        return data;
    }
}

public class DanceActionData
{
    public string AnimationName;
    public int AnimationID;
    public int StartBeat;
    public bool MoveToTarget;
    public Vector3 TargetPos;
    public bool FlipXAxis;
    public bool Loop;
    public int NumberOfJumps;
    public float JumpPower;
    public int AnimationBeatCount;

    public enum AnimationType{Dance, Attack, Beaten};
    public enum AttackDir{FromRightToLeft, FromLeftToRight, FromMiddleToRight, FromMiddleToLeft, FromRightToMiddle, FromLeftToMiddle}

    public AnimationType m_AnimationType;
    public AttackDir m_AttackDir;

    public DanceActionData(string animationName, int animationID, int startBeat)
    {
        AnimationName = animationName;
        AnimationID = animationID;
        StartBeat = startBeat;

        NumberOfJumps = 1;
        JumpPower = 2f;
        MoveToTarget = false;
        FlipXAxis = false;
        Loop = true;
        AnimationBeatCount = 1;
        m_AnimationType = AnimationType.Dance;
    }

    public static string ListToString(List<DanceActionData> dataList)
    {
        string result = "";

        return result;
    }
}
