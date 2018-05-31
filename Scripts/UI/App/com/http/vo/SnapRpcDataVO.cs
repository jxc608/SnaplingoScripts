using LitJson;

public class SnapRpcDataVO
{

	/** 请求的Action **/
	public string action = "";

	/** 服务器返回的数据 **/
	public JsonData data = null;

	/** 服务器返回的状态码 **/
	public int code;

    /** 服务器返回的状态 **/
    public bool status;

    /** 服务器返回的状态码文字说明 **/
    public string message = "";

	//转换成json字符串
	override public string ToString()
	{
		if (data == null)
		{
			return "";
		}
		return data.ToJson();
	}
}
