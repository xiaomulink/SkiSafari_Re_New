using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Debug.Log("开始界面");

        SplashClose();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    private void SplashClose()
    {
        Debug.Log("关闭页面");
        //能访问到，但是在munityplayer报错
        //AndroidJavaObject jo = new AndroidJavaObject("com.MMPark.splash.MainActivity");
        //jo.Call("HideSplash");
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        jo.Call("HideSplash");
    }
}
