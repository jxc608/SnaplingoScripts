using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

public class WriteSongEider : Manager
{
	public static WriteSongEider Instance { get { return GetManager<WriteSongEider>(); } }
	private string filePath =  "/Resources/Songs";

	private string KeyTimePath = "/Resources/SongKey";
	
	private string m_filePath;
	private string m_fileName;

	public void WriteSongtoTxt(SongTxtObject songInfo)
	{
		m_filePath = Application.dataPath + filePath;
		m_fileName = m_filePath	+ "/" + songInfo.SongInfos.SongName + ".txt";
		LogManager.Log(m_fileName);
		if (!Directory.Exists(m_filePath))
		{
			Directory.CreateDirectory(m_filePath);
		}
		FileStream fs;
		StreamWriter sw;
		if (File.Exists(m_fileName))
		{
			fs = new FileStream(m_fileName, FileMode.Create, FileAccess.Write);
		}
		else
		{
			fs = new FileStream(m_fileName, FileMode.Create, FileAccess.Write);
		}
		sw = new StreamWriter(fs);
		sw.WriteLine("[General]");
		sw.WriteLine("AudioFileName:" + songInfo.SongInfos.AudioFileName + ".mp3");
		sw.WriteLine("BPM:" + songInfo.SongInfos.BPM);
		sw.WriteLine("[HitObjects]");
		if (songInfo.HitInfos.Count > 0 && songInfo.BossInfos.IsBoss == false)
		{
			for (int i = 0; i < songInfo.HitInfos.Count; i++)
			{
				float x = (songInfo.HitInfos[i].Position[0] + 700) * 512f / 1400f;
				float y = (525 - songInfo.HitInfos[i].Position[1]) * 512f / 1400f;
				if (songInfo.HitInfos[i].Type == SentenceInfo.SentenceType.SoundNode)
				{
					sw.WriteLine((int)x + "," + (int)y + "," + (songInfo.HitInfos[i].StartHitTime + songInfo.HitInfos[i].EndHitTime)/2 + ",1,0,0:0:0:0:");
				}
				else
				{
					sw.WriteLine((int)x + "," + (int)y + "," + songInfo.HitInfos[i].HitTime + ",1,0,0:0:0:0:");
				}
			}
			sw.WriteLine("[SentenceInfo]");
			int startTime = songInfo.HitInfos[0].StartHitTime;
			int endTime = songInfo.HitInfos[0].EndHitTime;
			string soundText = "";
			string clickText = songInfo.HitInfos[0].ClickText;
			if (songInfo.HitInfos[0].Type == SentenceInfo.SentenceType.SoundNode)
				soundText = "#" + songInfo.HitInfos[0].SoundText;
			else
				soundText = songInfo.HitInfos[0].SoundText;
			for (int i = 1; i < songInfo.HitInfos.Count; i++)
			{
				if (songInfo.HitInfos[i].StartHitTime == startTime)
				{
					if (!string.IsNullOrEmpty(songInfo.HitInfos[i].SoundText) && !string.IsNullOrEmpty(songInfo.HitInfos[i].ClickText))
					{
						soundText = soundText + "$" + songInfo.HitInfos[i].SoundText;
						clickText = clickText + "$" + songInfo.HitInfos[i].ClickText;
					}
				}
				else
				{
					if (string.IsNullOrEmpty(soundText) == false && string.IsNullOrEmpty(clickText) == false)
					{
						sw.WriteLine(startTime + "," + endTime + "," + soundText + "," + clickText);
					}
					startTime = songInfo.HitInfos[i].StartHitTime;
					endTime = songInfo.HitInfos[i].EndHitTime;
					clickText = songInfo.HitInfos[i].ClickText;
					if (songInfo.HitInfos[i].Type == SentenceInfo.SentenceType.SoundNode)
						soundText = "#" + songInfo.HitInfos[i].SoundText;
					else
						soundText = songInfo.HitInfos[i].SoundText;
				}
			}
			if ( string.IsNullOrEmpty(soundText) == false && string.IsNullOrEmpty(clickText) == false)
			{
				sw.WriteLine(startTime + "," + endTime + "," + soundText + "," + clickText);
			}
		}
		else
		{
			sw.WriteLine("[SentenceInfo]");
		}

		BossInfo tempBoss = songInfo.BossInfos;
		if (tempBoss.BossLineObject.Count > 0)
		{
			if (tempBoss.IsBoss)
			{
				sw.WriteLine("@@" + tempBoss.StartTime + "," + tempBoss.DelayTime);
				for (int i = 0; i < tempBoss.BossLineObject.Count; i++)
				{
					BossLineInfo tempLine = tempBoss.BossLineObject[i];
					string isEnd = "";
					if (tempLine.IsEnd == true)
						isEnd = "BB";
					if (tempLine.Type == SentenceInfo.SentenceType.SoundNode)
						sw.WriteLine(tempLine.KeepTimeLength + ",#" + tempLine.SoundText + "," + tempLine.ClickText + "," + tempLine.MusicFileName + "," + isEnd);
					else
						sw.WriteLine(tempLine.KeepTimeLength + "," + tempLine.SoundText + "," + tempLine.ClickText + "," + tempLine.MusicFileName + "," + isEnd);
				}
			}
			else
			{
				sw.WriteLine("@@VoiceStart," + tempBoss.DelayTime);
				for (int i = 0; i < tempBoss.BossLineObject.Count; i++)
				{
					BossLineInfo tempLine = tempBoss.BossLineObject[i];
					if (tempLine.Type == SentenceInfo.SentenceType.SoundNode)
						sw.WriteLine(tempLine.KeepTimeLength + ",#" + tempLine.SoundText + "," + tempLine.ClickText + "," + tempLine.MusicFileName);
					else
						sw.WriteLine(tempLine.KeepTimeLength + "," + tempLine.SoundText + "," + tempLine.ClickText + "," + tempLine.MusicFileName);
				}
			}
		}

		sw.Close();
		fs.Close();
	}

	public void WriteSongKeyTime(SongTxtObject songInfo)
	{
		m_filePath = Application.dataPath + KeyTimePath;
		m_fileName = m_filePath + "/" + songInfo.SongInfos.SongName + "KeyTime.txt";
		LogManager.Log(m_fileName);
		if (!Directory.Exists(m_filePath))
		{
			Directory.CreateDirectory(m_filePath);
		}
		FileStream fs;
		StreamWriter sw;
		if (File.Exists(m_fileName))
		{
			fs = new FileStream(m_fileName, FileMode.Create, FileAccess.Write);
		}
		else
		{
			fs = new FileStream(m_fileName, FileMode.Create, FileAccess.Write);
		}
		sw = new StreamWriter(fs);
		for (int i = 0; i < songInfo.HitInfos.Count; i++)
		{
			int ms = songInfo.HitInfos[i].HitTime % 1000;
			int s = (songInfo.HitInfos[i].HitTime / 1000)%60;
			int m = (songInfo.HitInfos[i].HitTime / 1000) / 60;
			string timeHit = m + ":" + s.ToString("00") + "." + ms.ToString("000");
			if( songInfo.HitInfos[i].Type == SentenceInfo.SentenceType.SoundNode )
				sw.WriteLine("#num: " + (i + 1) + ", time: " + timeHit);
			else
				sw.WriteLine("num: " + (i+1)+", time: " + timeHit);
		}
		sw.Close();
		fs.Close();
	}
}