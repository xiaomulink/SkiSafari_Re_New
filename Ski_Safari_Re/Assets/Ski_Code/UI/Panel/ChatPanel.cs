using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ChatPanel : BasePanel
{
    public static ChatPanel _chat;
    public bool isshow;

    Transform MainPanel;

    Transform Chat_;

    Transform MinimizPanel;

    Transform content_MainPanel;
    Transform content_MinimizPanel;

    GameObject ChatGird;

    public List<Chat> ChatList;
    public List<Chat> ChatLastList;

    public class Chat
    {
        public string name;
        public string chat;
    }
    //UI
    public Text ChatText;

    Button Takeback_Btn;
    Button MinimizPanel_Btn;
    Button ChatSendButton;

    InputField chatField;
    //初始化UI
    public override void OnInit()
	{
		this.skinPath = "ChatPanel";
		this.layer = PanelManager.Layer.Panel;
	}
	
	//UI绑定
	public override void OnShow(params object[] args)
	{
        _chat = this;

        ChatText = skin.transform.Find("ChatText").GetComponent<Text>();
        ChatGird = ChatText.gameObject;
        Takeback_Btn = skin.transform.Find("MainPanel/Takeback_Btn").GetComponent<Button>();
        MinimizPanel_Btn = skin.transform.Find("MinimizPanel").GetComponent<Button>();
        ChatSendButton = skin.transform.Find("MainPanel/ChatSendButton").GetComponent<Button>();
        chatField = skin.transform.Find("MainPanel/InputField").GetComponent<InputField>();

        Chat_ = skin.transform.Find("MainPanel");

        content_MainPanel = skin.transform.Find("MainPanel/Scroll View/Viewport/Content");
        content_MinimizPanel = skin.transform.Find("MinimizPanel/Scroll View/Viewport/Content");

        ChatGird.gameObject.SetActive(false);

        NetManager.AddMsgListener("MsgGameChat", OnMsgGameChat);
        NetWebServerManager.AddMsgListener("MsgGameChat", OnMsgGameChat);

        ChatList = new List<Chat>();
        ChatLastList = new List<Chat>();

        Hide();

        ChatSendButton.onClick.AddListener(delegate () {
            SendGameChat();
        });
        Takeback_Btn.onClick.AddListener(delegate () {
                Hide();
        });
        MinimizPanel_Btn.onClick.AddListener(delegate () {
                Show();
        });
    }

    public void SendGameChat()
    {
        if (_chat.chatField.text != "")
        {
            MsgGameChat msg = new MsgGameChat();
            msg.ChatType = 3;
            msg.Id = MyGameInfo.id;
            msg.Name = MyGameInfo.N_name;
            msg.Chat = _chat.chatField.text;
            Debug.Log(MyGameInfo.id);
            Debug.Log(MyGameInfo.N_name);
            NetManager.Send(msg);
            NetWebServerManager.Send(msg);
            _chat.chatField.text = "";
        }
    }

    public void Hide()
    {
        chatField.DeactivateInputField();
        Chat_.gameObject.SetActive(false);
        isshow = false;
    }
    public void Show()
    {
        Chat_.gameObject.SetActive(true);
        chatField.ActivateInputField();
        isshow = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SendGameChat();
        }
    }

    public void OnMsgGameChat(MsgBase msgBase)
    {
        MsgGameChat msgChat = (MsgGameChat)msgBase;
        Chat chat = new Chat();
        Debug.LogError("Chat:" + msgChat.Name + msgChat.Chat);
        if (msgChat.ChatType == 0)
        {
            chat.name = "系统提示：";
        }
        else
        {
            chat.name = msgChat.Name;
        }
        chat.chat = msgChat.Chat;
        ChatList.Add(chat);
    }

  
    public GameObject GenerateItem(Chat chat,Transform content)
    {

        GameObject Object = Instantiate(ChatGird);
        Object.name = "name"+ ChatGird.name;
        Object.tag = "itemData";
        Object.transform.SetParent(content);
        Object.SetActive(true);
        Object.transform.localScale = Vector3.one;
        Transform trans = Object.transform;
      
        //聊天内容
        Text ChatText = trans.GetComponent<Text>();

        ChatText.text =chat.name+":"+ chat.chat;
        return Object;
    }
    private void FixedUpdate()
    {
        try
        {
            List<Chat> Chats = ChatList.Except(ChatLastList).ToList();
            foreach (Chat chat in Chats)
            {
                GenerateItem(chat, content_MainPanel);
                GenerateItem(chat, content_MinimizPanel);
                ChatLastList.Add(chat);
            }
        }
        catch
        {

        }
    }
}
