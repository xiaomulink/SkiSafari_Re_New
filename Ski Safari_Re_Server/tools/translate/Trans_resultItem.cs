using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//如果好用，请收藏地址，帮忙分享。
public class Trans_resultItem
{
    /// <summary>
    /// 你好
    /// </summary>
    public string src { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string dst { get; set; }
}

public class Root
{
    /// <summary>
    /// 
    /// </summary>
    public string @from { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string to { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public List<Trans_resultItem> trans_result { get; set; }
}

