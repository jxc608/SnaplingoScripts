using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Text.RegularExpressions;

class RegisterConfig
{
	//用户输入的名字
	public static string Temp_UserName;
	//用户输入的密码
	public static string Temp_UserPassword;
	//用户选择的国家
	public static int Temp_userCity = 0;

	public const string Hint_UserNameCN = "5-11位的字母或数字";
	public const string Hint_UserNameEN = "5 to 11 letters and/or numbers";

	public const string Hint_PasswordCN = "这也将作为你的初始密码";
	public const string Hint_PasswordEN = "It is also your password";

	//手机号正则
	public static Regex Reg_Phone = new Regex(@"^(13[0-9]|14[5|7]|15[0|1|2|3|5|6|7|8|9]|18[0|1|2|3|5|6|7|8|9])\d{8}$");
	//邮箱正则
	public static Regex Reg_Email = new Regex(@"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");

}
