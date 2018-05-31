using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;

public class ReadSongEider
{
    private enum CollectType { General, HitObject, SentenceInfo, Boss }
    private static CollectType m_CollectType;

    public static SongTxtObject GetSongInfo(string fileName)
    {
        SongTxtObject songInfo = new SongTxtObject();

        string path = "/Resources/Songs/" + fileName+ ".txt";

		if (DebugConfigController.Instance.RunTimeEditor)
		{
			path = Application.dataPath + path;
		}
		else
		{
			path = "Assets" + path;
		}
		songInfo.SongInfos.SongName = fileName;
		if (!File.Exists(path))
		{
			return null;
		}
		string[] contents = File.ReadAllLines(path);
		m_CollectType = CollectType.General;

		int hitno = 0;
		for (int i = 0; i < contents.Length; i++)
        {
            string parseContent = contents[i].Trim();

            if (string.IsNullOrEmpty(parseContent))
            {
                continue;
            }

            if (parseContent.Contains("[HitObjects]"))
            {
                m_CollectType = CollectType.HitObject;
                continue;
            }

			if (parseContent.Contains("[SentenceInfo]"))
			{
				m_CollectType = CollectType.SentenceInfo;
				continue;
			}

			if (parseContent.Contains("@@"))
			{
				m_CollectType = CollectType.Boss;
				string[] timeTemp = parseContent.Split(',');
				if (timeTemp.Length < 2)
				{
					LogManager.LogError("data parse error: the data file is  --" , path);
				}
				if (parseContent.Contains("@@VoiceStart"))
				{
					songInfo.BossInfos.IsBoss = false;
					songInfo.BossInfos.StartTime = -1;
					songInfo.BossInfos.DelayTime = int.Parse(timeTemp[1]);
				}
				else
				{
					songInfo.BossInfos.IsBoss = true;
					songInfo.BossInfos.StartTime = int.Parse(timeTemp[0].Replace("@@", ""));
					songInfo.BossInfos.DelayTime = int.Parse(timeTemp[1]);
				}
				continue;
			}

            switch (m_CollectType)
            {
                case CollectType.General:
                    if (parseContent.Contains("AudioFileName:"))
                    {
                        songInfo.SongInfos.AudioFileName = parseContent.Replace("AudioFileName:", "");
                    }
                    else if (parseContent.Contains("BPM:"))
                    {
                        songInfo.SongInfos.BPM = int.Parse(parseContent.Replace("BPM:", ""));
                    }
                    break;
                case CollectType.HitObject:
                    string[] param = parseContent.Split(',');
                    if (param.Length < 3)
                    {
						LogManager.LogError("data parse error: the data file is  --" , path);
                    }
                    else
                    {
                        Vector2 pos = new Vector2(int.Parse(param[0]), int.Parse(param[1]));

						Vector2 pos2 = new Vector2(pos.x * (1400f / 512f) - 700f, 525 - pos.y * (1400f / 512f));
						int startTime = int.Parse(param[2]);

                        HitInfo ho = new HitInfo();
                        ho.HitTime = startTime;
                        ho.Position = pos2;
                        if(songInfo.HitInfos.Count > 0)
                            songInfo.HitInfos[songInfo.HitInfos.Count-1].EndHitTime = ho.HitTime - 100;
                        ho.StartHitTime = ho.HitTime;
                        ho.Type = SentenceInfo.SentenceType.ClickNode;
                        ho.EndHitTime = ho.HitTime + 2000;
                        ho.SoundText = "SE";
                        ho.ClickText = "SE";
                        songInfo.HitInfos.Add(ho);

                    }
                    break;
				case CollectType.SentenceInfo:
					string[] temp = parseContent.Split(',');
					//加了配音字段，先注掉
					if (temp.Length != 4 && false)
					{
						LogManager.LogError("data parse error: the data file is  --" , path);
						LogManager.LogError ("parseContent: " , parseContent);
					}
					else
					{
						SentenceInfo sentence = new SentenceInfo();
						sentence.StartTime = int.Parse(temp[0]);
						sentence.EndTime = int.Parse(temp[1]);
						if (temp[2].Contains("#"))
						{
							temp[2] = temp[2].Replace("#", "");
							sentence.Type = SentenceInfo.SentenceType.SoundNode;
						}
						else
							sentence.Type = SentenceInfo.SentenceType.ClickNode;
						string[] SoundText = temp[2].Split('$');
						string[] ClickText = temp[3].Split('$');
						for (int textNum = 0; hitno < songInfo.HitInfos.Count; hitno++, textNum++ )
						{
							if (songInfo.HitInfos[hitno].HitTime >= sentence.StartTime && songInfo.HitInfos[hitno].HitTime <= sentence.EndTime)
							{
								songInfo.HitInfos[hitno].StartHitTime = sentence.StartTime;
								songInfo.HitInfos[hitno].EndHitTime = sentence.EndTime;
								songInfo.HitInfos[hitno].Type = sentence.Type;
								if( sentence.Type == SentenceInfo.SentenceType.SoundNode )
									songInfo.HitInfos[hitno].HitTime = sentence.StartTime;
								if (textNum < SoundText.Length && textNum < ClickText.Length)
								{
									songInfo.HitInfos[hitno].SoundText = SoundText[textNum];
									songInfo.HitInfos[hitno].ClickText = ClickText[textNum];
								}
							}
							else
							{
								break;
							}
						}
					}
					break;
				case CollectType.Boss:
					string[] bossTemp = parseContent.Split(',');
					if ((songInfo.BossInfos.IsBoss && bossTemp.Length != 5) || (!songInfo.BossInfos.IsBoss && bossTemp.Length != 4 && bossTemp.Length != 3))
					{
						LogManager.LogError("data parse error: the data file is  --" ,path);
					}
					BossLineInfo lineTemp = new BossLineInfo();
					lineTemp.KeepTimeLength = int.Parse(bossTemp[0]);
					if (bossTemp[1].Contains("#"))
						lineTemp.Type = SentenceInfo.SentenceType.SoundNode;
					else
						lineTemp.Type = SentenceInfo.SentenceType.ClickNode;
					lineTemp.SoundText = bossTemp[1].Replace("#", "");
					lineTemp.ClickText = bossTemp[2].Replace("#", "");
					if (bossTemp.Length > 3)
					{
						lineTemp.MusicFileName = bossTemp[3];
					}
					if (songInfo.BossInfos.IsBoss && bossTemp[4] == "BB")
					{
						lineTemp.IsEnd = true;
					}
					else
					{
						lineTemp.IsEnd = false;
					}
					songInfo.BossInfos.BossLineObject.Add(lineTemp);
					break;
            }
        }
        return songInfo;
    }
}