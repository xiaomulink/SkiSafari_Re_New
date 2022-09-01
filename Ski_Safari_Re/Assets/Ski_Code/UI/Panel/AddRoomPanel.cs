using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddRoomPanel : BasePanel
{
    //UI
    public Dropdown PeopleNnb;

    public Button OkBtn;

    public Button CloseBtn;
    public InputField Describe;
    //初始化UI
    public override void OnInit()
	{
		this.skinPath = "AddRoomPanel";
		this.layer = PanelManager.Layer.Panel;
	}

    //UI绑定
    public override void OnShow(params object[] args)
    {
        this.CloseBtn = this.skin.transform.Find("BackGround/Close").GetComponent<Button>();
        this.PeopleNnb = this.skin.transform.Find("BackGround/PeopleNub").GetComponent<Dropdown>();
        this.Describe = this.skin.transform.Find("BackGround/Describe").GetComponent<InputField>();

        this.OkBtn = this.skin.transform.Find("BackGround/OkBtn").GetComponent<Button>();
        NetManager.AddMsgListener("MsgCreateRoom", new NetManager.MsgListener(this.OnMsgCreateRoom));
        this.CloseBtn.onClick.AddListener(this.OnClickClose);
        this.OkBtn.onClick.AddListener(this.OnClick);
    }
    public void OnClickClose()
    {
     
        base.Close();
    }
    public void OnClick()
    {
        MsgCreateRoom msgCreateRoom = new MsgCreateRoom();
        //CharInfo component = ResManager.LoadPrefab("CharData/reimu").GetComponent<CharInfo>();
        switch (this.PeopleNnb.value)
        {
            case 0:
                msgCreateRoom.maxplayer = 2;
                break;
            case 1:
                msgCreateRoom.maxplayer = 4;
                break;
            case 2:
                msgCreateRoom.maxplayer = 6;
                break;
        }
     
        msgCreateRoom.Gamemode = 2;
        msgCreateRoom.describe = Describe.text;
        msgCreateRoom.id = MyGameInfo.id;
        NetManager.Send(msgCreateRoom);
    }

    public void OnMsgCreateRoom(MsgBase msgBase)
    {
        MsgCreateRoom msgCreateRoom = (MsgCreateRoom)msgBase;
        if (msgCreateRoom.result == 0)
        {
            RoomListPanel.Roomid = msgCreateRoom.roomid;
            RoomListPanel.GameMode = msgCreateRoom.Gamemode;
            MyGameInfo.roomid = msgCreateRoom.roomid;
            //PanelManager.Open<RoomPanel>(PanelManager.UIstyle.Default, Array.Empty<object>());
            PanelManager.Open<Tip>(PanelManager.UIstyle.Default, new object[]
        {
            "创建成功！"
        });
            PanelManager.Open<RoomPanel>(PanelManager.UIstyle.Default, Array.Empty<object>());

            base.Close();
            return;
        }
        PanelManager.Open<Tip>(PanelManager.UIstyle.Default, new object[]
        {
            "创建失败,请重试"
        });
    }

    public override void OnClose()
    {
        NetManager.RemoveMsgListener("MsgCreateRoom", new NetManager.MsgListener(this.OnMsgCreateRoom));
    }

}
