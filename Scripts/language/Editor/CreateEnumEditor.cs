using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using UnityEditor;

public class CreateEnumEditor
{
    /// <summary>
    /// 存放配表数据的第一列数据
    /// </summary>
    private static List<string> itemList = null;

    /// <summary>
    /// 创建以配表数据第一列为枚举元素的枚举和一个持有该枚举对象的类
    /// </summary>
    private static void CreateClassText()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("public enum SelfType\n");
        stringBuilder.Append("{\n");
		stringBuilder.Append("\tDefault = -1,\n");
        for (int i = 0; i < itemList.Count; i++)
        {
            stringBuilder.Append("\t" + itemList[i] + ",\n");
        }
		stringBuilder.Append("\tCount,\n");
        stringBuilder.Append("}\n");
     
        string path = Application.dataPath + "/" + "Scripts/language/flag/";
        if(!File.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
		File.WriteAllText(path+"SelfType.cs", stringBuilder.ToString());
		LogManager.Log("<------创建配表枚举标志类成功------>");
    }
    /// <summary>
    /// 编辑器方法：工程文件夹streamingAssetsPath/language/language.txt文件存在则可生成此枚举和类
    /// </summary>
    [MenuItem("Tools/CreateEnum")]
    private static void ReadLocalTable()
    {
        itemList = new List<string>();
        string path = Application.streamingAssetsPath + "/language/" + "language.txt";
        if(File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);
            for (int i = 0; i < lines.Length; i++)
            {
                itemList.Add(lines[i].Split('$')[0]);
            }
            CreateClassText();
        }
        else
        {
			LogManager.Log("<---创建配表枚举标志类失败，请确保streamingAssetsPath/language/该路径下是否存在language.txt文件--->");
        }
    }
}
