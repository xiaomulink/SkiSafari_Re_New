using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PausePanel : BasePanel
{
    //UI
    Button HomeButton;
    Button ResumeButton;

    //初始化UI
    public override void OnInit()
    {
        this.skinPath = "PausePanel";
        this.layer = PanelManager.Layer.Panel;
    }

    //UI绑定
    public override void OnShow(params object[] args)
    {
        HomeButton = skin.transform.Find("HomeButton").GetComponent<Button>();
        ResumeButton = skin.transform.Find("ResumeButton").GetComponent<Button>();

        HomeButton.onClick.AddListener(delegate ()
        {
            SceneManager.LoadSceneAsync(2);
        });
        ResumeButton.onClick.AddListener(delegate () {
            Close();
        });
    }
}
