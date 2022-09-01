using System.Collections;
using System.Collections.Generic;

public class MsgExpliciItem : MsgBase
{
    //设置协议名
    public MsgExpliciItem()
    {
        this.protoName = "MsgExpliciItem";
    }

    // 接下来填写参数
    public int[] Itemid;
    public int[] RandomID;
    public int[] result_RandomID;
}
