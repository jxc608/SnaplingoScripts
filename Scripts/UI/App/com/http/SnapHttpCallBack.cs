using System;
using UnityEngine;

class SnapHttpCallBack
{
    public Action<SnapRpcDataVO> actionCallBack;
    public SnapRpcDataVO dataVO;

    public void SendAction()
    {
        if (actionCallBack != null)
        {
            actionCallBack.Invoke(dataVO);
            actionCallBack = null;
        }
    }

}
