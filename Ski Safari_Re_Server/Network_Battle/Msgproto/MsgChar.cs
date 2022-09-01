using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class MsgChar : MsgBase
{
    //设置协议名
    public MsgChar()
    {
        this.protoName = "MsgChar";
    }

    // 接下来填写参数
    public int type;//0=set,1=get
    public string char_name;
}
