using System;

public class MsgSyncPlayer : MsgBase
{
    // 接下来填写参数
    public float mx;

    public float my;

    public double x;

    public double y;

    public double z;

    public double ex;

    public double ey;

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

    public string id = "";

    public int itembag;
    public int itemprotective;
    public int itemshoes;
    public int itemweapon;
    public int itemcloth;

    public MsgSyncPlayer()
    {
        this.protoName = "MsgSyncPlayer";
    }
}
