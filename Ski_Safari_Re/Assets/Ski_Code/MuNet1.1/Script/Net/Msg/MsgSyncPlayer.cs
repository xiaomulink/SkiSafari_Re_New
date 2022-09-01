using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MsgSyncPlayer : MsgBase
{
	//设置协议名
	public MsgSyncPlayer()
	{
		this.protoName = "MsgSyncPlayer";
	}

    // 接下来填写参数
    public float mx;

    public float my;

    //位置x
    public double x;

    //位置y
    public double y;

    //位置z
    public double z;

    //旋转x
    public double ex;

    //旋转y
    public double ey;

    //旋转z
    public double ez;

    //旋转x
    public double lookex;

    //旋转y
    public double lookey;

    //旋转z
    public double lookez;

    public string State;

    public string playerType;
    public string playerMovementType;
    public string playerMovementHandType;

    public int itembag;
    public int itemprotective;
    public int itemshoes;
    public int itemweapon;
    public int itemcloth;

    //游戏id
    public string id = "";
}
