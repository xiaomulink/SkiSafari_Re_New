using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MsgShop : MsgBase
{
    //设置协议名
    public MsgShop()
    {
        this.protoName = "MsgShop";
    }

    // 接下来填写参数
    public int coin;
    public int itemid;
    public int itemnub;
    public int itemtype;
    public int result;
}

