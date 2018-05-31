using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class EditorModeFileMisc  
{
    public static List<string> GetAudioFileName()
    {
        List<string> audioFileName = new List<string>();
        AudioCategory ac = AudioController.Instance._GetCategory("Music");
        foreach (AudioItem item in ac.AudioItems)
        {
            audioFileName.Add(item.Name);
        }
        return audioFileName;
    }

    public static List<string> GetFileName(string partFileName, string filePath)
    {
        List<string> audioFileName = new List<string>();
        DirectoryInfo dirInfo = new DirectoryInfo(filePath);
        if (dirInfo.Exists)
        {
            FileInfo[] fis = dirInfo.GetFiles(partFileName);
            if (fis.Length > 0)
            {
                foreach (FileInfo file in fis)
                {
                    audioFileName.Add(Path.GetFileNameWithoutExtension(file.Name));
                }
            }
            else
                return null;
        }
        return audioFileName;
    }
}
