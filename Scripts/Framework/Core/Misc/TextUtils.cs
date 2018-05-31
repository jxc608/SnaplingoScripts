using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TextUtils  
{
    public static bool IsChinese(string text, int index)
    {
        int code = 0;
        int chfrom = Convert.ToInt32("4e00", 16);    //范围（0x4e00～0x9fff）转换成int（chfrom～chend）
        int chend = Convert.ToInt32("9fff", 16);
        if (!string.IsNullOrEmpty(text))
        {
            code = Char.ConvertToUtf32(text, index);    //获得字符串input中指定索引index处字符unicode编码

            if (code >= chfrom && code <= chend)
            {
                return true;     //当code在中文范围内返回true

            }
            else
            {
                return false;    //当code不在中文范围内返回false
            }
        }
        return false;
    }
}
