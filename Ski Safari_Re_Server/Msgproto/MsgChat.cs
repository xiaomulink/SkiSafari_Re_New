using System.Collections;
using System.Collections.Generic;

public class MsgChat : MsgBase
{
    //设置协议名
    public MsgChat()
    {
        this.protoName = "MsgChat";
    }

    // 接下来填写参数
    public int ChatType;//0 System,1 Player,2 Lobby System,3 Lobby System

    public string Id;
    public string Name;
    public string Chat;
}
