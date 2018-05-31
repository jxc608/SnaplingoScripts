using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeConst
{
    #region general
    public const int FirstLevelID = 1001;
    public const int LevelBase = 1000;
    #endregion

    #region coreplay
    public const int HitPerfect = 0;
    public const int HitRight = 1;
    public const int HitWrong = 2;

    public const int LifeCutUnit = -1;
    public const int SecondToMS = 1000;
    public const float MSToSecond = 0.001f;


    public const int CheckFinished = -1;
    public const int ClickTooEarly = -2;
    public const int InitLife = 5;

    public const int BossWarLose = -1;
    public const int BossWarWin = 0;
    public const int BossWarDraw = 1;

    public const float ScoreParam = 2.4f;
    public const float ScoreAddOnParam = 1.1f;
    #endregion

    #region EditorMode
    public const float TimeRulerLength = 12f;
    public const float TimeRulerHalfLength = 6f;
    public const int InitTimeLineNodeNumber = 30;
    #endregion

    #region Ranking
    public const int DefaultPerPageNumber = 10;
    public const int DefaultEmptyRank = -1;
    #endregion
}
