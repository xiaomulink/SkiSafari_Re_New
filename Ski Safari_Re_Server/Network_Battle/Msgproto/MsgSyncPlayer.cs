using System;

public class MsgSyncPlayer : MsgBase
{
    // ��������д����
    public float mx;

    public float my;

    public double x;

    public double y;

    public double z;

    public double ex;

    public double ey;

    public double ez;

    //��תx
    public double lookex;

    //��תy
    public double lookey;

    //��תz
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
