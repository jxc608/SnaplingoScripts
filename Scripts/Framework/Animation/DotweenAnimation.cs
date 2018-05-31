using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public static class DotweenAnimation 
{
    public static void FromA2B(this Transform trans, Vector3 startPos, Vector3 endPos, float duration)
    {
        Vector3 middleLerp = Vector3.Lerp(startPos, endPos, 0.5f);
        if(Mathf.Approximately(startPos.y, 0))
        {
            middleLerp = new Vector3(middleLerp.x, endPos.y + Mathf.Abs(endPos.y) / 2, middleLerp.z);
        }
        else
        {
            middleLerp = new Vector3(middleLerp.x, endPos.y + Mathf.Abs(endPos.y) / 2, middleLerp.z);
        }
     
        StaticMonoBehaviour.Instance.StartCoroutine(DoProcess(trans, startPos, endPos, middleLerp, duration, 0, 0.05f));
    }

    public static void FromA2BLocal(this Transform trans, Vector3 startPos, Vector3 endPos, float duration)
    {
        trans.localPosition = startPos;
        Tweener tweener = trans.DOLocalMove(endPos, duration);
        tweener.SetEase(Ease.OutBack);
    }

    static IEnumerator DoProcess(Transform trans, Vector3 startPos , Vector3 endPos, Vector3 middlePos, float duration, float timer, float delta)
    {
        yield return new WaitForSeconds(delta);

        Matrix matrix = new Matrix(3);
        List<float> row1 = new List<float>(new float[] { startPos.x * startPos.x, startPos.x, 1, startPos.y});
        List<float> row2 = new List<float>(new float[] { middlePos.x * middlePos.x, middlePos.x, 1, middlePos.y });
        List<float> row3 = new List<float>(new float[] { endPos.x * endPos.x, endPos.x, 1, endPos.y });
        matrix.m_Matrix[0] = row1;
        matrix.m_Matrix[1] = row2;
        matrix.m_Matrix[2] = row3;

        List<float> result = Matrix.Gauss(matrix);
        //a b c分别为二次函数的系数
        float c = result[2];
        float b = result[1];
        float a = result[0];

        float t = timer / duration;
        Vector3 lerpVec = Vector3.Lerp(startPos, endPos, t);
        float lerpY = a * lerpVec.x * lerpVec.x + b * lerpVec.x + c;

        trans.position = new Vector3(lerpVec.x, lerpY, lerpVec.z);

        if(timer <= duration)
        {
            StaticMonoBehaviour.Instance.StartCoroutine(DoProcess(trans, startPos, endPos, middlePos, duration, timer + delta, delta));
        }
    }
}

public class Matrix
{
    public List<List<float>> m_Matrix = new List<List<float>>();
    public Matrix (int rowNum)
    {
        for (int i = 0; i < rowNum; i++)
        {
            List<float> temp = new List<float>();
            m_Matrix.Add(temp);
        }
    }

    public static List<float> Gauss(Matrix matrix)
    {
        List<float> result = new List<float>();
        if (matrix.m_Matrix.Count > 0)
        {
            for (int row = 0; row < matrix.m_Matrix.Count - 1; row++)
            {
                if (matrix.m_Matrix[row].Count != matrix.m_Matrix[row + 1].Count)
                {
					LogManager.LogError("matrix input error!!");
                    return null;
                }
            }
        }
        else
        {
			LogManager.LogError("matrix input error!!");
            return null;
        }

        int rowNum = matrix.m_Matrix.Count;
        int columnNum = matrix.m_Matrix[0].Count;
        int i = 0, j = 0;
        while (i < rowNum && j < columnNum)
        {
            int maxI = i;
            for (int k = i + 1; k < rowNum; k++)
            {
                if (Mathf.Abs(matrix.m_Matrix[k][j]) > Mathf.Abs(matrix.m_Matrix[maxI][j]))
                {
                    maxI = k;
                }
            }

            if (!Mathf.Approximately(matrix.m_Matrix[maxI][j],0))
            {
                List<float> temp = null;
                temp = matrix.m_Matrix[maxI];
                matrix.m_Matrix[maxI] = matrix.m_Matrix[i];
                matrix.m_Matrix[i] = temp;
                float maxValue = matrix.m_Matrix[i][j];
                for (int k = 0; k < matrix.m_Matrix[i].Count; k++)
                {
                    matrix.m_Matrix[i][k] /= maxValue;
                }

                for (int u = i + 1; u < rowNum; u++)
                {
                    float firstValue = matrix.m_Matrix[u][j];
                    for (int k = j; k < columnNum; k++)
                    {
                        matrix.m_Matrix[u][k] -= firstValue * matrix.m_Matrix[i][k];
                    }
                }
                i++;
            }
            j++;
        }


        //求行列式的值
        //当有3个参数的时候，手动求解过程如下：
        //float c = result.m_Matrix[2][3];
        //float b = result.m_Matrix[1][3] - result.m_Matrix[1][2] * c;
        //float a = result.m_Matrix[0][3] - result.m_Matrix[0][2] * c - result.m_Matrix[0][1] * b;
        //改成用循环的方式：
        for (i = rowNum - 1; i >= 0; i--)
        {//
            float tempResult = matrix.m_Matrix[i][columnNum - 1];
            int calcTimes = rowNum - 1 - i;
            for (j = calcTimes; j > 0 && result.Count > 0; j--)
            {
                tempResult -= matrix.m_Matrix[i][columnNum - 1 - j] * result[ result.Count - j];
            }
            result.Insert(0,tempResult);
        }

        return result;
    }
}
