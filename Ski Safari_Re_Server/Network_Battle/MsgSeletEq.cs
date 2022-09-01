using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MsgSeletEq : MsgBase
{
    //设置协议名
    public MsgSeletEq()
    {
        this.protoName = "MsgSeletEq";
    }

    // 接下来填写参数
    public int id;
    public string type;//0=char,1=bullet
}

