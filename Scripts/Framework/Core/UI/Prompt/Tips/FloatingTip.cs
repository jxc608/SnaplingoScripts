using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class FloatingTip : MonoBehaviour
{
	Tweener tweener;
	public Text Desc;
	public Image BG;
	private PromptManager.OnReceiveMessageBoxResult messageBoxCallback = null;

	public void Init(string str, PromptManager.OnReceiveMessageBoxResult callBack)
	{
		messageBoxCallback = callBack;
		transform.localPosition = Vector3.zero;
		transform.localScale = Vector3.one;
        Desc.text = SetDescLanguage(str);
		startTime = true;
	}
    private string SetDescLanguage(string str)
    {
        print(str);
        switch(str)
        {
            case "用户名不能为空":
                return LanguageManager.Instance.GetValueByKey(SelfType.ErrorLoginUserName.ToString(), LanguageManager.languageType);
            case "密码不能为空":
                return LanguageManager.Instance.GetValueByKey(SelfType.ErrorLoginPassWord.ToString(), LanguageManager.languageType);
            case "请输入用户名":
                return LanguageManager.Instance.GetValueByKey(SelfType.ErrorRegisterUserName.ToString(), LanguageManager.languageType);
            case "请输入密码":
                return LanguageManager.Instance.GetValueByKey(SelfType.ErrorRegisterPassWord.ToString(), LanguageManager.languageType);
            case "系统繁忙，请稍后再试":
                return LanguageManager.Instance.GetValueByKey(SelfType.ErrorRegisterservierBusy.ToString(), LanguageManager.languageType);
            case "手机号输入有误，请重新输入":
                return LanguageManager.Instance.GetValueByKey(SelfType.ErrorRegisterShareNumberException.ToString(), LanguageManager.languageType);
			case "帐号被锁定，请联系客服人员":
				return LanguageManager.Instance.GetValueByKey(SelfType.ErrorLoginTipLock.ToString(), LanguageManager.languageType);
			case "密码输入有误":
				return LanguageManager.Instance.GetValueByKey(SelfType.ErrorLoginTipPassWordMiss.ToString(),LanguageManager.languageType);
			case "用户名太短":
				return LanguageManager.Instance.GetValueByKey(SelfType.ErrorRegisterLength.ToString(),LanguageManager.languageType);
			case "用户名已注册":
				return LanguageManager.Instance.GetValueByKey(SelfType.ErrorRegisterExist.ToString(),LanguageManager.languageType);
			case "用户名包含非法内容，请更换":
				return LanguageManager.Instance.GetValueByKey(SelfType.ErrorRegisterChangeUserName.ToString(),LanguageManager.languageType);
			case "请填写正确的手机号":
				return LanguageManager.Instance.GetValueByKey(SelfType.ErrorRegisterRightNumber.ToString(),LanguageManager.languageType);
			case "邮件地址错误":
				return LanguageManager.Instance.GetValueByKey(SelfType.ErrorRegisterEmailError.ToString(),LanguageManager.languageType);
			case "无法获取舞蹈数据":
				return LanguageManager.Instance.GetValueByKey(SelfType.GetVideoError.ToString(), LanguageManager.languageType);
            default:
                return null;
        }
    }

	public void Close()
	{
		if (messageBoxCallback != null) {
			messageBoxCallback(PromptManager.Result.OK);
			messageBoxCallback = null;
		}
		Destroy(gameObject);
	}

	bool startTime;
	bool isAnim;
	float speed = 0.05f;
	float timer;

	void Update()
	{
		if (startTime) {
			timer += Time.unscaledDeltaTime;

			if (timer > 1.5f) {
				tweener = BG.DOColor(new Color(0f, 0f, 0f, 0f), 1f);
				tweener.OnComplete<Tweener>(delegate () {
					Close();
				});
				isAnim = true;
				timer = 0;
				startTime = false;
			}
		}
		if (isAnim) {
			Desc.color -= new Color(0, 0, 0, speed);
			if (Desc.color.a < 0)
				isAnim = false;
		}
	}
}
