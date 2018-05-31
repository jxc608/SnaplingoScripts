using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using Snaplingo.UI;
using System;
using LitJson;
using UnityEngine.SceneManagement;

public class PlayerInfoPanel : MonoBehaviour
{
	#region [ --- Property --- ]
	const float _enterLoaclPosY = -110;
	const float _panelSpaceY = 256;
	int _score;
	Vector3 _defaultPos;
	bool isSelf;

	public RankObject rankObjectDebug;
	Animator anim_btnVoice;
	#endregion


	#region [ --- Object References --- ]
	[Header (" --- Object References ---")]
	public Text text_name, text_rank, text_score;
	public Image image_base, image_avatar, image_accuracy;
	public Sprite[] sp_ranks;
	public Sprite sp_default, sp_player;
	public ChangeSelfInfoNode node_ChangeSelfInfoNode;
	public Button but_Video, but_Voice;
    public Image userCountry;
	#endregion

	#region [ --- Mono --- ]
	void Awake ()
	{
		_defaultPos = transform.localPosition;
		if (but_Voice != null)
			anim_btnVoice = but_Voice.GetComponent<Animator> ();

		MicManager.Instance.stopVoiceEvent.AddListener (OnStopVoice);
	}
	private void Start ()
	{
		if (but_Voice != null)
		{
			but_Voice.onClick.AddListener (() => {
				MicManager.Instance.PlayAllVoice (rankObjectDebug.uid,
												  StaticData.LevelID,
												  () => {
													  anim_btnVoice.SetBool ("isPlaying", true);
												  },
												  () => {
					LogManager.Log ("下载录音失败");
													  anim_btnVoice.SetBool ("isPlaying", false);
												  });
			});
		}


		if (but_Video != null)
		{
			but_Video.onClick.AddListener (() => {
				JsonData data = new JsonData ();
				data["historyID"] = rankObjectDebug.historyID;
				DancingWordAPI.Instance.RequestLevelDancDataFromServer (data, (string resultData) => {
					LogManager.Log (resultData);
					JsonData result = JsonMapper.ToObject (resultData);
					if (result.TryGetString ("code").Equals ("1"))
					{
                        string json = result.TryGetData("data").TryGetData("danceData").TryGetString("danceResult");
                        if(SceneManager.GetActiveScene().name == "BookScene")
                        {
                            StaticData.ChoreographerData = json;
                            LoadSceneManager.Instance.LoadNormalScene("StageScene");
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(json))
                            {
                                PageManager.Instance.CurrentPage.GetNode<ScoreListNode>().Close(true);
                                PageManager.Instance.CurrentPage.GetNode<StageOverNode>().Close(true);
                                ChoreographerData cData = ChoreographerData.GetChoreographerDataFromJson(json);
                                CorePlayManager.Instance.WatchOtherDance(cData);
                            }
                            else
                            {
                                PromptManager.Instance.MessageBox(PromptManager.Type.FloatingTip, "无法获取舞蹈数据");
                            }
                        }
					}
				}, () => {
					LogManager.Log ("获取过程数据失败！");
				});
			});
		}

	}
	void OnDestroy ()
	{
		MicManager.Instance.stopVoiceEvent.RemoveListener (OnStopVoice);
		SelfPlayerLevelData.Instance.changeNameEvent.RemoveListener (ChangeMyNameOrAvatar);
	}
	#endregion



	#region [ --- Public --- ]
	public void Reset ()
	{
		isSelf = false;
		image_base.sprite = sp_default;
		text_score.text = "0";
		transform.localPosition = _defaultPos;
		transform.localScale = Vector3.one;
		gameObject.SetActive (false);
		DOTween.Kill (transform);
		StopAllCoroutines ();
	}
	public PlayerInfoPanel Setup (RankObject rankObject, bool setScoreZero = false)
	{
		if (string.IsNullOrEmpty (rankObject.user.avatar))
			rankObject.user.avatar = "NoAvatar";


		string avatarUrl = "";

		rankObjectDebug = rankObject;
		isSelf = rankObject.uid == SelfPlayerData.Uuid;
		var sprite = Resources.Load<Sprite> ("UI/_Avatars/" + rankObject.user.avatar);

		if (isSelf && string.IsNullOrEmpty (SelfPlayerData.Uuid) == false)
		{
			SelfPlayerLevelData.Instance.changeNameEvent.AddListener (ChangeMyNameOrAvatar);
			ChangeMyNameOrAvatar ();
			//var sprite = Resources.Load<Sprite>("UI/_Avatars/" + SelfPlayerData.AvatarUrl);
			//if (sprite == null)
			//{
			//	LogManager.LogWarning("没有图片 Resources/UI/_Avatars/" , SelfPlayerData.AvatarUrl);
			//	SelfPlayerData.AvatarUrl = "NoAvatar";
			//	sprite = Resources.Load<Sprite>("UI/_Avatars/" + SelfPlayerData.AvatarUrl);
			//}
			transform.Find ("Image_gear").GetComponent<Image> ().color = new Color32 (255, 221, 142, 255);
			//image_avatar.sprite = sprite;
			avatarUrl = SelfPlayerData.AvatarUrl;
			//AsyncImageDownload.GetInstance().SetAsyncImage(SelfPlayerData.AvatarUrl, image_avatar, sprite);
			image_base.sprite = sp_player;
		}
		else
		{
			SelfPlayerLevelData.Instance.changeNameEvent.RemoveListener (ChangeMyNameOrAvatar);
			text_name.text = rankObject.user.nickname;

			//if (sprite == null)
			//{
			//	LogManager.LogWarning("没有图片 Resources/UI/_Avatars/" , rankObject.user.avatar);
			//	sprite = Resources.Load<Sprite>("UI/_Avatars/NoAvatar");
			//}
			transform.Find ("Image_gear").GetComponent<Image> ().color = new Color32 (255, 255, 255, 255);
			//image_avatar.sprite = sprite;
			avatarUrl = rankObject.user.avatar;
			//AsyncImageDownload.GetInstance().SetAsyncImage(rankObject.user.avatar, image_avatar, sprite);
			image_base.sprite = sp_default;
		}

		//if (avatarUrl.IndexOf("http:") != -1)
		//{
		//	AsyncImageDownload.GetInstance().SetAsyncImage(avatarUrl, image_avatar);
		//}
		//else if(avatarUrl.IndexOf("Avatar") != -1)
		//{
		SetAvatar (image_avatar, rankObject.uid);
        userCountry.sprite = GetCountryByIndex(rankObject.user.country);
		//}

		text_rank.text = rankObject.rank.ToString ();
		int grade = SetLevel.setLevel (rankObject.accuracy);
		LogManager.Log("玩家获得的级别序号：",grade);
		_score = rankObject.score;
		image_accuracy.sprite = sp_ranks[grade];


		text_score.text = setScoreZero ? "0" : _score.ToString ();

		gameObject.SetActive (true);
		if (but_Video != null)
		{
			if (rankObjectDebug.rank != 1 || StaticData.LevelID % 4 == 0)
			{
				but_Video.gameObject.SetActive (false);
				but_Voice.transform.localPosition = new Vector2 (but_Voice.transform.localPosition.x, 0);
			}
			else
			{
				but_Video.gameObject.SetActive (true);
				but_Voice.transform.localPosition = new Vector2 (but_Voice.transform.localPosition.x, -48);
			}
		}
		return this;
	}
	public static void SetAvatar (Image target, string uid)
	{
		//LogManager.Log(uid);
		Sprite sprite = null;
		JsonData data = new JsonData ();
		data["uid"] = uid;
		DancingWordAPI.Instance.RequestRoleDataFromServer (data, (string result) => {
			//LogManager.Log(result);

			JsonData resultData = JsonMapper.ToObject (result);
			//LogManager.Log(resultData["code"]);
			if (resultData.TryGetString ("code") == "1")
			{
				//LogManager.Log("OKOKOKOKOK");
				//LogManager.Log(resultData.TryGetData("data").TryGetString("modelId"));
				sprite = Resources.Load<Sprite> ("Avatar/Avatar_" + resultData.TryGetData ("data").TryGetString ("modelId"));
				//LogManager.Log(sprite.name);

			}
			if (sprite == null)
			{
				sprite = Resources.Load<Sprite> ("Avatar/NoAvatar");
			}
			target.sprite = sprite;
		}, null);
	}
	public void PlayMoveIn (float enterDelay, float addScoreTime)
	{
		transform.localPosition = new Vector3 (Screen.width, 0, 0) + _defaultPos;
		var _seq = DOTween.Sequence ();
		_seq.Append (transform.DOLocalMoveX (0, enterDelay).SetEase (Ease.OutBack))
				 .AppendCallback (() => StartCoroutine (_CorAddScore (addScoreTime)));
	}
	public void PlayMoveUp (float enterDelay, float addScoreTime)
	{
		transform.localPosition = new Vector3 (Screen.width, _enterLoaclPosY, 0);
		var _seq = DOTween.Sequence ();
		_seq.Append (transform.DOLocalMoveX (0, enterDelay).SetEase (Ease.OutBack))
			.AppendCallback (() => StartCoroutine (_CorAddScore (addScoreTime)))
			.AppendInterval (addScoreTime)
			.Append (transform.DOLocalMove (_defaultPos, .8f).SetEase (Ease.InOutBack))
			.Insert (enterDelay * .6f, transform.DOScale (1.2f, enterDelay * .4f)
					.SetEase (Ease.InCirc)
					.SetLoops (2, LoopType.Yoyo));
	}
	public void PlayMoveDown (float enterDelay, float addScoreTime)
	{
		transform.localPosition = new Vector3 (0, _defaultPos.y + _panelSpaceY, 0);
		var _seq = DOTween.Sequence ();
		_seq.AppendInterval (enterDelay + addScoreTime)
			.Append (transform.DOLocalMoveY (_defaultPos.y, .8f).SetEase (Ease.OutBack));
	}
	public void SetName (string name, Sprite avatar = null)
	{
		text_name.text = name;
		if (avatar != null)
		{
			image_avatar.sprite = avatar;
		}
	}


	public void OnClickNamePanel ()
	{
		PageManager.Instance.CurrentPage.AddNode<UserInfoNode> (true, rankObjectDebug.uid);
		LogManager.Log ("点击排行榜某一用户时HIstoryID：" , rankObjectDebug.historyID);
		LogManager.Log ("点击排行榜某一用户时UID：" , rankObjectDebug.uid);
		LogManager.Log ("点击排行榜某一用户时Rank：" , rankObjectDebug.rank);
	}
	#endregion

	IEnumerator _CorAddScore (float addScoreTime)
	{
		float timer = 0;
		addScoreTime = addScoreTime < 1f ? addScoreTime : 1f;
		while (timer < addScoreTime)
		{
			timer += Time.deltaTime;
			float percent = timer / addScoreTime;
			percent = percent > 1 ? 1 : percent;
			text_score.text = ((int)(_score * percent)).ToString ();
			yield return null;
		}
		yield return null;
	}

	void ChangeMyNameOrAvatar ()
	{
		text_name.text = SelfPlayerData.Nickname;
		//if (string.IsNullOrEmpty(SelfPlayerData.AvatarUrl) == false)
		//	image_avatar.sprite = Instantiate(ResourceLoadUtils.Load<Sprite>("UI/_Avatars/" + SelfPlayerData.AvatarUrl));
	}



	void OnStopVoice ()
	{
		if (anim_btnVoice != null && anim_btnVoice.isActiveAndEnabled)
			anim_btnVoice.SetBool ("isPlaying", false);
	}

    private Sprite GetCountryByIndex(int index)
    {
        string flag = null;
        switch(index)
        {
            case 0:
                flag = "UI_CHN";
                break;
            case 1:
                flag = "UI_USA";
                break;
            case 2:
                flag = "UI_CAN";
                break;
            case 3:
                flag = "UI_UK";
                break;
            case 4:
                flag = "UI_USA";
                break;
        }
        return Resources.Load<Sprite>("Flag/" + flag);
    }
}

