class RpcDataCode
{

	/** 请求成功 **/
	public const int RequestFinish = 1;

	/**--------------------- 注册  ---------------------**/
	/** 状态码 - 用户名已存在 **/
	public const int NameExist = 4;
	/** 状态码 - 带脏字的名字 **/
	public const int NameDirty = 5;
	/** 状态码 - 激活码不能做用户名 **/
	public const int NameActivation = 5;
		
	/**--------------------- 登陆  ---------------------**/
	/** 状态码 - 账号被封 **/
	public const int AccoutLock = -102;
	/** 状态码 - 密码错误 **/
	public const int PasswordError = -100;

}