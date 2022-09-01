using System;

public class Player
{
	public string name = "";

	public string id = "";

	public ClientState state;

	public string CharName = "";

    public string CharSkinName = "";

    public string Weapon = "";

    // 接下来填写参数
    public float mx;

    public float my;

    public double x;

	public double y;

	public double z;

    public string State;

    public string playerType;
    public string playerMovementType;
    public string playerMovementHandType;

    public int itembag;
    public int itemprotective;
    public int itemshoes;
    public int itemweapon;
    public int itemcloth;


    public int roomId = -1;

	public int camp = 1;

	public string Usecharname;

	public int UseSkillnub;

	public int hp = 100;

	public PlayerData data;

    public int Score;

    public int Diescore;

    public Player(ClientState state)
	{
		this.state = state;
	}

	public void Send(MsgBase msgBase)
	{
		NetManager.Send(this.state, msgBase);
	}

	public void Broadcast(MsgBase msg)
	{
		Room room = new Room();
		foreach (string current in room.playerIds.Keys)
		{
			Player player = PlayerManager.GetPlayer(current);
			player.Send(msg);
		}
	}
}
