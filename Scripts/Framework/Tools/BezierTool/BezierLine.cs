using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierLine : MonoBehaviour 
{
    public Transform m_NextPosition;

    public List<Vector3> Points
    {
        get { return m_BezierPoints; }
    }

    public List<Vector3> m_BezierPoints = new List<Vector3>();

    public const int AnchorNumber = 3;
    public const int FirstAnchor = 0;
    public const int SecondAnchor = 1;
    public const int ThirdAnchor = 2;
    public Vector3[] m_AnchorPos = new Vector3[AnchorNumber];//每段贝塞尔曲线有4个定位点

    public int m_PointNumber = 10;
  
	// Use this for initialization
	void Start () 
    {
		
	}
	
	// Update is called once per frame
	void Update () 
    {
		
	}

    public static List<Vector3> GetEqualSpaceBezier(Vector3 startPos, Vector3 anchor1, Vector3 anchor2, Vector3 endPos, int pointNumber)
    {
        List<Vector3> result = new List<Vector3>();

        List<float> lengthList = new List<float>();
        List<Vector3> tempPointList = new List<Vector3>();
        float lineLength = 0;
        for (int i = 0; i < 200; i++)
        {
            float t = i / 200f;

            Vector3 p1 = (anchor1 - startPos) * t + startPos;
            Vector3 p2 = ((anchor2 - anchor1) * t + anchor1 - p1) * t + p1;
            Vector3 p3 = ((endPos - anchor2) * t + anchor2 - p2) * t + p2;
            Vector3 final = (endPos - p3) * t + p3;

            tempPointList.Add(final);
            if (i > 0)
            {
                float distance = (final - tempPointList[i - 1]).magnitude;
                lineLength += distance;
                lengthList.Add(lineLength);
            }
            else
            {
                lengthList.Add(0f);
            }
        }



        #region interpolate the bezier line points to make them equaly spaced
        float delta = lineLength / pointNumber;
        result.Add(tempPointList[0]);
        for (int i = 0; i < pointNumber; i++)
        {
            float curLength = delta * i;

            for (int j = 0; j < 200; j++)
            {
                if (lengthList[j] >= curLength)
                {
                    if (j > 0)
                    {
                        result.Add(tempPointList[j - 1]);
                    }

                    break;
                }
            }
        }
        #endregion

        return result;
    }
}
