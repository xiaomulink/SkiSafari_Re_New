using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : BasePanel
{
    Transform UserAgreement;
    //UI
    Toggle IsUserAgreement;

    Button UserAgreementButton;
    Button CloseButton;
    Button LoginButton;

    InputField accountInputField;
    InputField nameInputField;
    //初始化UI
    public override void OnInit()
	{
		this.skinPath = "LoginPanel";
		this.layer = PanelManager.Layer.Panel;
	}
	
	//UI绑定
	public override void OnShow(params object[] args)
	{

        UserAgreementButton = skin.transform.Find("UserAgreementButton").GetComponent<Button>();
        CloseButton = skin.transform.Find("UserAgreement/CloseButton").GetComponent<Button>();
        LoginButton = skin.transform.Find("LoginButton").GetComponent<Button>();
        IsUserAgreement = skin.transform.Find("IsUserAgreement").GetComponent<Toggle>();
        accountInputField = skin.transform.Find("IdInputField").GetComponent<InputField>();
        nameInputField = skin.transform.Find("NameInputField").GetComponent<InputField>();

        UserAgreement = skin.transform.Find("UserAgreement");
        UserAgreement.gameObject.SetActive(false);
        UserAgreementButton.onClick.AddListener(delegate () {
            UserAgreement.gameObject.SetActive(true);
        });
        CloseButton.onClick.AddListener(delegate () {
            UserAgreement.gameObject.SetActive(false);
        });
        LoginButton.onClick.AddListener(delegate () {
            if (IsUserAgreement.isOn)
            {

                MsgTestPlayer msgTestPlayer = new MsgTestPlayer();
                msgTestPlayer.id = accountInputField.text;
                msgTestPlayer.name = nameInputField.text;
                MyGameInfo.id = msgTestPlayer.id;

                NetManager.Send(msgTestPlayer);

                MyGameInfo.id = accountInputField.text;
                MyGameInfo.N_name = nameInputField.text;
            }
            else
            {
                PanelManager.Open<Tip>(PanelManager.UIstyle.Default, "请阅读用户协议并勾选 “我已阅读并同意《用户协议》”框");
            }
        });
        try
        {
            if (PlayerPrefs.GetInt("IsUserAgreement")==1)
                IsUserAgreement.isOn =true;
            else
                IsUserAgreement.isOn = false;

            accountInputField.text = PlayerPrefs.GetString("Game_account");
            nameInputField.text = PlayerPrefs.GetString("Game_name");
        }
        catch { }
        NetManager.AddMsgListener("MsgTestPlayer", OnMsgTestPlayer);
    }

    public void OnMsgTestPlayer(MsgBase msgBase)
    {
        MsgTestPlayer msg = (MsgTestPlayer)msgBase;
        if (msg.result == 0)
        {
            MyGameInfo.id = msg.id;
            MyGameInfo.N_name = msg.name;
            PlayerPrefs.SetInt("IsUserAgreement", 1);
            PlayerPrefs.SetString("Game_account", MyGameInfo.id);
            PlayerPrefs.SetString("Game_name", MyGameInfo.N_name);
            Close();
        }
        else
        {
            PanelManager.Open<Tip>(PanelManager.UIstyle.Default, "已有此玩家,请重新填写！");
        }
    
    }
}
