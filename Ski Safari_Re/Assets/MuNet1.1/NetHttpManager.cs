using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;


//==========================
// - ScriptName:      NetHttpManager.cs         	
// - CreateTime:    2022/06/05 23:02:40	
// - Homepage:         http://touhouboredmutation.online/		
// - Email:         2252391595@qq.com		
// - Description:   
//==========================

public class NetHttpManager : MonoBehaviour
{
    /// <summary>
    /// HTTP Get请求
    /// </summary>
    /// <param name="url">API地址</param>
    /// <param name="encode">编码</param>

    public static String GetData(String url, Encoding encode)
    {

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "GET";
        request.ContentType = "text/html, application/xhtml+xml, */*";

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        Stream rs = response.GetResponseStream();
        StreamReader sr = new StreamReader(rs, encode);
        var result = sr.ReadToEnd();
        sr.Close();
        rs.Close();
        return result;

    }
}
