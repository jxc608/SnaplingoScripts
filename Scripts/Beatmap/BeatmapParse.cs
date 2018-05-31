using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;

public struct SongMaxScoreInfo
{
	public int MaxScore;
	public int MaxCombo;
	public int ClickFrequency;
	public float ClickScorePercent;
	public float VoiceScorePercent;
	public float ClickTimePercent;
	public float VoiceTimePercent;

}

public class BeatmapParse
{
	private static string[] m_Fields = { "AudioFileName:", "BPM:" };

	public static void CalcAllTxtScore ()
	{
#if UNITY_EDITOR
		List<string> options = null;

		options = EditorModeFileMisc.GetFileName ("*.txt", "Assets/Resources/Songs");

		for (int i = 0; i < options.Count; i++)
		{
			SongObject so = Parse (options[i]);
			SongMaxScoreInfo info = GetSongObjectMaxScore (so);
			//LogManager.Log("关卡 :" , options[i] , "的最高得分：" , info.MaxScore , 
			//          ". 其中点击次数：" , info.ClickFrequency , ", 语音语句数量：" , (info.MaxCombo - info.ClickFrequency));
			LogManager.Log ("关卡 :" , options[i] , "的最高得分：" , info.MaxScore,
					  ". 其中点击分数占比：" , info.ClickScorePercent , ", 语音分数占比：" , info.VoiceScorePercent ,
					  ".点击时间占比：" , info.ClickTimePercent , ", 语音时间占比：" ,info.VoiceTimePercent);
		}
#endif
	}

	public static SongMaxScoreInfo GetSongObjectMaxScore (SongObject so)
	{
		SongMaxScoreInfo info = new SongMaxScoreInfo ();
		info.MaxScore = 0;
		info.MaxCombo = 0;
		int voiceSorce = 0;
		int clickTime = 0;
		int voiceTime = 0;
		info.ClickFrequency = 0;
		//点击和语音
		for (int i = 0; i < so.SentenceObjs.Count; i++)
		{
			if (so.SentenceObjs[i].Type == SentenceType.Common)
			{
				for (int j = 0; j < so.SentenceObjs[i].ClickAndHOList.HitObjects.Count; j++)
				{
					clickTime += so.SentenceObjs[i].m_InOutTime.EndTime - so.SentenceObjs[i].m_InOutTime.StartTime;
					info.MaxScore += (int)(CorePlaySettings.Instance.m_ComboPoint + info.MaxCombo * RuntimeConst.ScoreParam);
					info.MaxCombo++;
					info.ClickFrequency++;
				}
			}
			else if (so.SentenceObjs[i].Type == SentenceType.Voice)
			{
				voiceTime += so.SentenceObjs[i].m_InOutTime.EndTime - so.SentenceObjs[i].m_InOutTime.StartTime;
				voiceSorce += CorePlaySettings.Instance.m_VoiceRightPoint;
				info.MaxScore += CorePlaySettings.Instance.m_VoiceRightPoint;
				info.MaxCombo++;
			}
		}

		//boss战
		for (int i = 0; i < so.StreamSentences.Count; i++)
		{
			if (so.StreamSentences[i].Type == SentenceType.Common)
			{
				for (int j = 0; j < so.StreamSentences[i].Group[0].Sentence.Count; j++)
				{
					clickTime += so.StreamSentences[i].TimeLength;
					info.MaxScore += (int)(CorePlaySettings.Instance.m_ComboPoint + info.MaxCombo * RuntimeConst.ScoreParam);
					info.MaxCombo++;
					info.ClickFrequency++;
				}
			}
			else if (so.StreamSentences[i].Type == SentenceType.Voice)
			{
				voiceTime += so.StreamSentences[i].TimeLength;
				voiceSorce += CorePlaySettings.Instance.m_VoiceRightPoint;
				info.MaxScore += CorePlaySettings.Instance.m_VoiceRightPoint;
				info.MaxCombo++;
			}
		}

		//普通关语音回放部分
		for (int i = 0; i < so.NonRhythmSenteces.Count; i++)
		{
			if (so.NonRhythmSenteces[i].Type == SentenceType.NonRhythmCommon)
			{
				for (int j = 0; j < so.NonRhythmSenteces[i].Sentence.Count; j++)
				{
					clickTime += (int)(so.NonRhythmSenteces[i].Duration * 1000);
					info.MaxScore += (int)(CorePlaySettings.Instance.m_ComboPoint + info.MaxCombo * RuntimeConst.ScoreParam);
					info.MaxCombo++;
					info.ClickFrequency++;
				}
			}
			else if (so.NonRhythmSenteces[i].Type == SentenceType.NonRhythmVoice)
			{
				voiceTime += (int)(so.NonRhythmSenteces[i].Duration * 1000);
				voiceSorce += CorePlaySettings.Instance.m_VoiceRightPoint;
				info.MaxScore += CorePlaySettings.Instance.m_VoiceRightPoint;
				info.MaxCombo++;
			}
		}

		info.VoiceScorePercent = (float)voiceSorce / info.MaxScore;
		info.ClickScorePercent = 1 - info.VoiceScorePercent;

		info.ClickTimePercent = (float)(clickTime) / (clickTime + voiceTime);
		info.VoiceTimePercent = 1 - info.ClickTimePercent;
		return info;
	}

	public static SongObject Parse (string fileName)
	{
		string path = "Songs/" + fileName;
		TextAsset osu = ResourceLoadUtils.Load<TextAsset> (path);
		if (osu == null)
		{
			LogManager.LogError (path , ".txt 文件没有找到！！请检查SongConfig文件中 scripts 列是否正确");
			return null;
		}

		return ParseOSU (osu, fileName);
	}

	public static string GetAudioFileName (string fileName)
	{
		string path = "Songs/" + fileName;
		TextAsset osu = ResourceLoadUtils.Load<TextAsset> (path);
		if (osu == null)
		{
			LogManager.LogError (path , ".txt 文件没有找到！！请检查SongConfig文件中 scripts 列是否正确");
			return null;
		}

		return ParseAudioFileName (osu);
	}

	private const string AudioFileName = "AudioFileName:";
	private static string ParseAudioFileName (TextAsset textAsset)
	{
		string[] contents = textAsset.text.Split ('\n');
		for (int i = 0; i < contents.Length; i++)
		{
			string parseContent = contents[i].Trim ();

			if (parseContent.Contains (AudioFileName))
			{
				return parseContent.Replace (AudioFileName, "");
			}
		}

		return null;
	}


	enum CollectType { General, Editor, Metadata, Difficulty, Events, TimingPoints, HitObject, SentenceInfo, KeyAndVoice }
	static SongObject ParseOSU (TextAsset textAsset, string fileName)
	{
		SongObject songObj = new SongObject ();
		CollectType _collectType = CollectType.General;

		string[] contents = textAsset.text.Split ('\n');
		Dictionary<string, string> result = new Dictionary<string, string> ();
		result.Add ("Title:", fileName);

		List<HitObject> tmpHitObject = new List<HitObject> ();
		int k = 0;
		bool nextSentenceAheadIn = false;
		bool streamSentenceEdit = false;


		for (int i = 0; i < contents.Length; i++)
		{
			string oneLine = contents[i].Trim ();

			//空行和注释行 不解析
			if (string.IsNullOrEmpty (oneLine) || oneLine.StartsWith ("//", StringComparison.Ordinal))
			{
				continue;
			}

			if (oneLine.Contains ("[HitObjects]"))
			{
				_collectType = CollectType.HitObject;
				continue;
			}
			if (oneLine.Contains ("[SentenceInfo]"))
			{
				_collectType = CollectType.SentenceInfo;
				continue;
			}

			switch (_collectType)
			{
				case CollectType.General:
					for (int j = 0; j < m_Fields.Length; j++)
					{
						if (oneLine.Contains (m_Fields[j]))
						{
							result.Add (m_Fields[j], oneLine.Replace (m_Fields[j], ""));
						}
					}
					break;
				case CollectType.SentenceInfo:
					if (!streamSentenceEdit)
					{//
						#region 普通句子
						SentenceObj sentenceobjs = new SentenceObj ();

						if (oneLine.Contains ("@@VoiceStart"))
						{
							sentenceobjs.Type = SentenceType.KeyVoiceStart;
							string[] stringArray = oneLine.Split (',');
							int sentenceStart = int.Parse (stringArray[1]);
							InOutTime inOut = new InOutTime (sentenceStart, sentenceStart);
							sentenceobjs.m_InOutTime = inOut;
							songObj.SentenceObjs.Add (sentenceobjs);
							_collectType = CollectType.KeyAndVoice;
							// LogManager.Log(oneLine , "     ///  _collectType = CollectType.KeyAndVoice");
							continue;
						}

						if (oneLine.Contains ("@@"))
						{
							sentenceobjs.Type = SentenceType.BossStart;
							string[] stringArray = oneLine.Replace ("@@", "").Split (',');
							if (stringArray.Length != 2)
							{
								LogManager.LogError ("boss战起始时间和延迟播放音乐配置错误");
							}
							int sentenceStart = int.Parse (stringArray[0]);
							int musicDelay = int.Parse (stringArray[1]);
							InOutTime inOut = new InOutTime (sentenceStart, sentenceStart);
							sentenceobjs.m_InOutTime = inOut;
							songObj.BossWarMusicDelay = musicDelay;
							streamSentenceEdit = true;
							songObj.SentenceObjs.Add (sentenceobjs);
							continue;
						}


						string[] parse = oneLine.Split (',');
						int startTime = int.Parse (parse[0]);
						int endTime = int.Parse (parse[1]);


						if (parse[2].Contains ("#"))
						{
							sentenceobjs.Type = SentenceType.Voice;
							parse[2] = parse[2].Replace ("#", "");
						}
						else
						{
							sentenceobjs.Type = SentenceType.Common;
						}

						string[] param = parse[2].Split ('$');
						string[] wordShow = parse[3].Split ('$');
						string[] wordSoundFile = null;
						if (parse.Length >= 5)
						{
							wordSoundFile = parse[4].Split ('$');
						}
						for (int j = 0; j < param.Length; j++)
						{
							if (k >= tmpHitObject.Count)
							{
								LogManager.LogError ("OSU文件中hitObject数目 与 txt文件中的单词数量不匹配");
								return null;
							}

							try
							{
								ClickObj click = new ClickObj (param[j], tmpHitObject[k].Position, wordShow[j]);
								click.m_ClickTimes = 1;
								sentenceobjs.ClickAndHOList.ClickObjs.Add (click);
								tmpHitObject[k].Word = param[j];
								if (wordSoundFile != null && wordSoundFile.Length > 0)
								{
									if (wordSoundFile.Length == param.Length)
									{
										tmpHitObject[k].SonudFile = wordSoundFile[j];
									}
									else
									{
										LogManager.LogError (string.Format ("行 {0} 有错误：句子单词个数 与 单词语音文件数量不匹配", (i + 1)));
										return null;
									}
								}
								sentenceobjs.ClickAndHOList.HitObjects.Add (tmpHitObject[k]);
								k++;
							}
							catch (Exception e)
							{
								LogManager.LogError ("the wrong line is:" , (i + 1).ToString ());
								LogManager.LogError (e);
								LogManager.LogError (e.StackTrace);
							}
						}

						InOutTime inOutTime = new InOutTime (startTime, endTime);
						sentenceobjs.m_InOutTime = inOutTime;
						//判断这一句是否应该延迟删除，下一句是否应该提前出现

						if (endTime - sentenceobjs.ClickAndHOList.HitObjects[sentenceobjs.ClickAndHOList.HitObjects.Count - 1].StartMilliSecond < CorePlaySettings.Instance.m_MinIntervalBeforeSentenceEnd)
						{//这一句延迟删除，下一句提前出现
							if (nextSentenceAheadIn)
							{
								sentenceobjs.m_InOutTime.InOut = EffectInOutTiming.Both;
							}
							else
							{
								sentenceobjs.m_InOutTime.InOut = EffectInOutTiming.DelayOut;
								nextSentenceAheadIn = true;
							}
						}
						else
						{
							if (nextSentenceAheadIn)
							{
								sentenceobjs.m_InOutTime.InOut = EffectInOutTiming.AheadIn;
								nextSentenceAheadIn = false;
							}
						}

						songObj.SentenceObjs.Add (sentenceobjs);
						#endregion
					}
					else
					{
						#region boss句子
						string[] parse = oneLine.Split (',');
						StreamSentenceGroup group = new StreamSentenceGroup ();
						if (parse.Length == 5 && parse[4].Equals ("BB"))
						{
							group.CalckSentence = true;
						}
						else
						{
							group.CalckSentence = false;
						}
						group.SetTimeLength (int.Parse (parse[0]));
						if (parse[1].Contains ("#"))
						{
							group.SetSentenceType (SentenceType.Voice);
							parse[1] = parse[1].Replace ("#", "");
						}
						else
						{
							group.SetSentenceType (SentenceType.Common);
						}
						string[] matchSentences = parse[1].Replace ("@@", "").Split ('|');
						string[] showSentences = parse[2].Split ('|');
						string[] fileNames = parse[3].Split ('|');
						if (matchSentences.Length != showSentences.Length || showSentences.Length != fileNames.Length)
						{
							LogManager.LogError ("TXT文件 第 " , (i + 1).ToString () , "行报错：断句数量不匹配");
							return null;
						}
						else
						{
							int sentenceNumber = matchSentences.Length;
							for (int line = 0; line < sentenceNumber; line++)
							{
								StreamSentence bossSentence = new StreamSentence ();
								bossSentence.SetAudioFileName (fileNames[line].ToLower ());
								string[] words = matchSentences[line].Split ('$');
								string[] showWords = showSentences[line].Split ('$');

								if (words.Length != showWords.Length)
								{
									LogManager.LogError ("TXT文件 第 " , (i + 1).ToString () , "行报错：单词数量不匹配 " ,
												   words.Length.ToString () , "/" + showWords.Length.ToString ());
									return null;
								}
								else
								{
									for (int index = 0; index < words.Length; index++)
									{
										KeyValuePair<string, string> kv = new KeyValuePair<string, string> (words[index], showWords[index]);
										bossSentence.AddToSentence (kv);
									}
								}
								group.AddSentence (bossSentence);
							}
						}

						songObj.StreamSentences.Add (group);
						#endregion
					}
					break;
				case CollectType.HitObject:
					if (oneLine.Contains ("C") || oneLine.Contains ("P") || oneLine.Contains ("L") || oneLine.Contains ("B"))
					{//2017年10月28日版本先不做这些功能
						continue;
					}
					else
					{
						string[] param = oneLine.Split (',');
						if (param.Length != 6)
						{
							LogManager.LogError ("data parse error: the data file is  --" , fileName);
						}
						else
						{
							Vector2 pos = new Vector2 (int.Parse (param[0]), int.Parse (param[1]));
							int startTime = int.Parse (param[2]);
							int type = int.Parse (param[3]);
							int soundType = int.Parse (param[4]);

							HitObject ho = new HitObject (pos, startTime, type, soundType);

							tmpHitObject.Add (ho);
						}
					}
					break;
				case CollectType.KeyAndVoice:
					//LogManager.Log(oneLine , " |||||  case CollectType.KeyAndVoice:");

					string[] parses = oneLine.Split (',');
					if (parses.Length < 3)
					{
						LogManager.LogError ("TXT文件 第 " , (i + 1).ToString () , "行报错：分段不够");
						return null;
					}

					StreamSentence ss = new StreamSentence ();
					ss.SetDuration (int.Parse (parses[0]) * 0.001f);
					if (parses[1].StartsWith ("#", StringComparison.Ordinal))
					{

						//parses[1] = parses[1].Substring(1);
						parses[1] = parses[1].Replace ("#", "");
						//LogManager.Log ("   需要录音 = " , parses[1]);
						ss.needRecord = true;
						ss.SetType (SentenceType.NonRhythmVoice);
						//if (parses[1].StartsWith ("#", StringComparison.Ordinal))
						//{
						//	parses[1] = parses[1].Substring (1);
						//	ss.needRecord = true;
						//}
					}
					else
					{
						ss.SetType (SentenceType.NonRhythmCommon);
					}

					if (parses.Length == 4)
					{
						string[] tempAudios = parses[3].Split ('|');
						if (string.IsNullOrEmpty (tempAudios[0]) == false)
							ss.SetAudioFileName (tempAudios[0].ToLower ());
						if (tempAudios.Length == 2 && string.IsNullOrEmpty (tempAudios[1]) == false)
							ss.Speaker = tempAudios[1];
						//ss.SetAudioFileName(parses[3]);
					}

					string[] firstWords = parses[1].Split ('$');
					string[] clickWords = parses[2].Split ('$');

					if (firstWords.Length != clickWords.Length)
					{
						LogManager.LogError ("TXT文件 第 " , (i + 1).ToString () , "行报错：单词数量不匹配");
						return null;
					}
					else
					{
						for (int index = 0; index < firstWords.Length; index++)
						{
							KeyValuePair<string, string> kv = new KeyValuePair<string, string> (firstWords[index], clickWords[index]);
							ss.AddToSentence (kv);
							//ClickObj click = new ClickObj(kv.Key, tmpHitObject[k].Position, kv.Value);
							//ClickObj click = new ClickObj(kv.Key, Vector2.zero, kv.Value);
							//click.m_ClickTimes = 1;
							//ss.ClickAndHOList.ClickObjs.Add(click);
							//tmpHitObject[k].Word = kv.Key;
							//ss.ClickAndHOList.HitObjects.Add(tmpHitObject[k]);
							//k++;
						}
					}
					songObj.NonRhythmSenteces.Add (ss);
					break;
			}
		}

		songObj.SetContents (result);

		return songObj;
	}
}
