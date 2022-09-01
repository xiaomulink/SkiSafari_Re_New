using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CustomPanel : BasePanel
{
    Texture2D tempImage;
    public UnityEngine.Sprite tempSprie;

    //UI
    Text GameScoreMultiplierText;
    Button DeleteDataButton;
    Button HomeButton;
    Button GoLinkButton;
    Toggle InfiniteCoinToggle;
    Toggle FractionX10;
    Toggle FractionX100;
    Toggle FractionX1000;
    Toggle OriginalMap;

    GameObject CustomSettingPanel;
    GameObject CustomPlayerPanel;

    Button CustomSettingButton;
    Button CustomPlayerButton;

    Image PlayerTexture;

    AudioSource FullAudio;
    AudioSource GetupAudio;
    AudioSource BacksomersaultAudio;

    Button FileTextureButton;
    Button FileAudio1Button;
    Button FileAudio2Button;
    Button FileAudio3Button;

    Button PlayAudio_1;
    Button PlayAudio_2;
    Button PlayAudio_3;

    Dropdown PlayAudio_1_drop;
    Dropdown PlayAudio_2_drop;
    Dropdown PlayAudio_3_drop;

    InputField PlayerTexture_Field;
    InputField PlayAudio_1_Field;
    InputField PlayAudio_2_Field;
    InputField PlayAudio_3_Field;

    [Serializable]
    public class Custom
    {
        public bool InfiniteCoinToggle;
        public bool FractionX10;
        public bool FractionX100;
        public bool FractionX1000;
        public bool OriginalMap;

        public bool Texture_1;
        public bool Audio_1;
        public bool Audio_2;
        public bool Audio_3;
    }
    //初始化UI
    public override void OnInit()
	{
		this.skinPath = "CustomPanel";
		this.layer = PanelManager.Layer.Panel;
	}

    //UI绑定
    public override void OnShow(params object[] args)
    {
        PlayerTexture = skin.transform.Find("CustomPlayerPanel/TexturePanel/Image").GetComponent<Image>();

        CustomSettingPanel = skin.transform.Find("CustomSettingPanel").gameObject;
        CustomPlayerPanel = skin.transform.Find("CustomPlayerPanel").gameObject;

        GameScoreMultiplierText = CustomSettingPanel.transform.Find("GameScoreMultiplierText").GetComponent<Text>();

        FileTextureButton = skin.transform.Find("CustomPlayerPanel/TexturePanel/Button").GetComponent<Button>();
        DeleteDataButton = skin.transform.Find("DeleteDataButton").GetComponent<Button>();
        HomeButton = skin.transform.Find("HomeButton").GetComponent<Button>();
        GoLinkButton = skin.transform.Find("GoLinkButton").GetComponent<Button>();
        CustomSettingButton = skin.transform.Find("CustomSettingButton").GetComponent<Button>();
        CustomPlayerButton = skin.transform.Find("CustomPlayerButton").GetComponent<Button>();

        FileAudio1Button = skin.transform.Find("CustomPlayerPanel/AudioPanel/Audio_1/Button").GetComponent<Button>();
        FileAudio2Button = skin.transform.Find("CustomPlayerPanel/AudioPanel/Audio_2/Button").GetComponent<Button>();
        FileAudio3Button = skin.transform.Find("CustomPlayerPanel/AudioPanel/Audio_3/Button").GetComponent<Button>();

        PlayAudio_1_drop = skin.transform.Find("CustomPlayerPanel/AudioPanel/Audio_1/Dropdown").GetComponent<Dropdown>();
        PlayAudio_2_drop = skin.transform.Find("CustomPlayerPanel/AudioPanel/Audio_2/Dropdown").GetComponent<Dropdown>();
        PlayAudio_3_drop = skin.transform.Find("CustomPlayerPanel/AudioPanel/Audio_3/Dropdown").GetComponent<Dropdown>();

        PlayerTexture_Field = skin.transform.Find("CustomPlayerPanel/TexturePanel/InputField").GetComponent<InputField>();

        PlayAudio_1_Field = skin.transform.Find("CustomPlayerPanel/AudioPanel/Audio_1/InputField").GetComponent<InputField>();
        PlayAudio_2_Field = skin.transform.Find("CustomPlayerPanel/AudioPanel/Audio_2/InputField").GetComponent<InputField>();
        PlayAudio_3_Field = skin.transform.Find("CustomPlayerPanel/AudioPanel/Audio_3/InputField").GetComponent<InputField>();

        PlayAudio_1 = skin.transform.Find("CustomPlayerPanel/AudioPanel/Audio_1/AudioPlay").GetComponent<Button>();
        PlayAudio_2 = skin.transform.Find("CustomPlayerPanel/AudioPanel/Audio_2/AudioPlay").GetComponent<Button>();
        PlayAudio_3 = skin.transform.Find("CustomPlayerPanel/AudioPanel/Audio_3/AudioPlay").GetComponent<Button>();

        InfiniteCoinToggle = CustomSettingPanel.transform.Find("InfiniteCoinToggle").GetComponent<Toggle>();
        FractionX10 = CustomSettingPanel.transform.Find("FractionX10").GetComponent<Toggle>();
        FractionX100 = CustomSettingPanel.transform.Find("FractionX100").GetComponent<Toggle>();
        FractionX1000 = CustomSettingPanel.transform.Find("FractionX1000").GetComponent<Toggle>();
        OriginalMap = CustomSettingPanel.transform.Find("OriginalMap").GetComponent<Toggle>();

        FullAudio = skin.transform.Find("CustomPlayerPanel/AudioPanel/Audio_1/AudioPlay").GetComponent<AudioSource>();
        GetupAudio = skin.transform.Find("CustomPlayerPanel/AudioPanel/Audio_2/AudioPlay").GetComponent<AudioSource>();
        BacksomersaultAudio = skin.transform.Find("CustomPlayerPanel/AudioPanel/Audio_3/AudioPlay").GetComponent<AudioSource>();

        InfiniteCoinToggle.isOn = SkiGameManager.Instance.custom.InfiniteCoinToggle;
        FractionX10.isOn = SkiGameManager.Instance.custom.FractionX10;
        FractionX100.isOn = SkiGameManager.Instance.custom.FractionX100;
        FractionX1000.isOn = SkiGameManager.Instance.custom.FractionX1000;

        PlayerTexture.sprite = ResManager.LoadRes<UnityEngine.Sprite>("common/sd/" + GameResCustomManager.Instance.Player_Default_Skin);

        FullAudio.clip = SkiGameManager.Instance.PlayerHit1.clip;
        GetupAudio.clip = SkiGameManager.Instance.FXFlameTrailGlow_1.clip;
        BacksomersaultAudio.clip = SkiGameManager.Instance.FxFlameTrail_1.clip;

        if (PlayerPrefs.GetInt("OriginalMap") == 0)
            OriginalMap.isOn = false;
        else
            OriginalMap.isOn = true;

        PlayAudio_1.onClick.AddListener(delegate () {
            //GetupAudio.enabled = false;
            GetupAudio.Play();
            // GetupAudio.enabled = true;
        });
        PlayAudio_2.onClick.AddListener(delegate () {
            //FullAudio.enabled = false;
            FullAudio.Play();
            //FullAudio.enabled = true;
        });
        PlayAudio_3.onClick.AddListener(delegate () {
            // BacksomersaultAudio.enabled = false;
            BacksomersaultAudio.Play();
            // BacksomersaultAudio.enabled = true;
        });

        GoLinkButton.onClick.AddListener(delegate ()
        {
            string url = "https://space.bilibili.com/297969683";
            Application.OpenURL(url);
        });

        DeleteDataButton.onClick.AddListener(delegate ()
        {
            GameState.DeleteAll();
        });

        HomeButton.onClick.AddListener(delegate () {
            SkiGameManager.Instance.ShowCustom = false;
            Close();
        });

        FileTextureButton.onClick.AddListener(delegate ()
        {

#if UNITY_EDITOR
            // 加载PC端上的图片
            GetTexture(Application.streamingAssetsPath+"/Custom/"+PlayerTexture_Field.text,
           
                SetTexttureToRawImage);
#else
        // 加载Android端的图片
        GetTexture(@"file:///"+Application.persistentDataPath+"/Custom/"+PlayerTexture_Field.text,
            SetTexttureToRawImage);
#endif
        }
        );

        FileAudio1Button.onClick.AddListener(delegate ()
        {
            if(PlayAudio_1_drop.value==0)
                StartCoroutine(LoadExternalAudioWebRequest(GetupAudio, "Audios", PlayAudio_1_Field.text));
            else
                StartCoroutine(LoadExternalAudioWebRequest(GetupAudio, "Audios", PlayAudio_1_Field.text, AudioType.MPEG));
            SkiGameManager.Instance.custom.Audio_1 = true;
        });

        FileAudio2Button.onClick.AddListener(delegate ()
        {
            if (PlayAudio_2_drop.value == 0)
                StartCoroutine(LoadExternalAudioWebRequest(FullAudio, "Audios", PlayAudio_2_Field.text));
            else
                StartCoroutine(LoadExternalAudioWebRequest(FullAudio, "Audios", PlayAudio_2_Field.text, AudioType.MPEG));
            SkiGameManager.Instance.custom.Audio_2 = true;
        });

        FileAudio3Button.onClick.AddListener(delegate ()
        {
            if (PlayAudio_3_drop.value == 0)
                StartCoroutine(LoadExternalAudioWebRequest(BacksomersaultAudio, "Audios", PlayAudio_3_Field.text));
            else
                StartCoroutine(LoadExternalAudioWebRequest(BacksomersaultAudio, "Audios", PlayAudio_3_Field.text, AudioType.MPEG));
            SkiGameManager.Instance.custom.Audio_3 = true;
        });
        CustomSettingButton.onClick.AddListener(delegate () {
            CustomPlayerPanel.SetActive(false);
        });
        CustomPlayerButton.onClick.AddListener(delegate () {
            CustomPlayerPanel.SetActive(true);
        });
        CustomPlayerPanel.SetActive(false);

    }

    public void BrowserClosed(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            // no selection
            return;
        }
        // successful selection
        Debug.Log("You selected: " + path);
    }

    private void Update()
    {
        SkiGameManager.Instance.custom.InfiniteCoinToggle = InfiniteCoinToggle.isOn;
        SkiGameManager.Instance.custom.FractionX10 = FractionX10.isOn;
        SkiGameManager.Instance.custom.FractionX100 = FractionX100.isOn;
        SkiGameManager.Instance.custom.FractionX1000 = FractionX1000.isOn;
        if(FractionX10.isOn)
        {
            SkiGameManager.Instance.customIncreaseMultiple = 10;
        }else if(SkiGameManager.Instance.customIncreaseMultiple == 10)
        {
            SkiGameManager.Instance.customIncreaseMultiple = 1;
        }
        if (FractionX100.isOn)
        {
            SkiGameManager.Instance.customIncreaseMultiple = 100;
        }
        else if (SkiGameManager.Instance.customIncreaseMultiple == 100)
        {
            SkiGameManager.Instance.customIncreaseMultiple = 1;
        }
        if (FractionX1000.isOn)
        {
            SkiGameManager.Instance.customIncreaseMultiple = 1000;
        }
        else if (SkiGameManager.Instance.customIncreaseMultiple == 1000)
        {
            SkiGameManager.Instance.customIncreaseMultiple = 1;
        }
        if (OriginalMap.isOn)
        {
            PlayerPrefs.SetInt("OriginalMap", 1);
        }
        else 
        {
            PlayerPrefs.SetInt("OriginalMap", 0);
        }
        GameScoreMultiplierText.text = "当前分数倍率:" + SkiGameManager.Instance.customIncreaseMultiple.ToString("N0");
    }

    void SetTexttureToRawImage(Texture2D texture)
    {
        tempSprie= ToSprite(texture);
        GameResCustomManager.Instance.m_PlayerSprie = tempSprie;
        GameResCustomManager.Instance._PlayerTexture = texture;
        PlayerTexture.sprite = tempSprie;
        SkiGameManager.Instance.custom.Texture_1 = true;
        SkiGameManager.Instance.ResCustomManager.enabled = true;
    }


    /// <summary>
    /// 请求图片
    /// </summary>
    /// <param name="url">图片地址,like 'http://www.my-server.com/image.png '</param>
    /// <param name="action">请求发起后处理回调结果的委托,处理请求结果的图片</param>
    /// <returns></returns>
    public void GetTexture(string url, Action<Texture2D> actionResult)
    {
        Debug.Log(url);
        StartCoroutine(_GetTexture(url, actionResult));
    }

  
    IEnumerator _GetTexture(string url, Action<Texture2D> actionResult)
    {
        UnityWebRequest uwr = new UnityWebRequest(url);
        DownloadHandlerTexture downloadTexture = new DownloadHandlerTexture(true);
        uwr.downloadHandler = downloadTexture;
        yield return uwr.SendWebRequest();
        Texture2D t = null;
        if (!(uwr.isNetworkError || uwr.isHttpError))
        {
            t = downloadTexture.texture;
        }
        else
        {
            Debug.Log("下载失败，请检查网络，或者下载地址是否正确 ");
        }

        if (actionResult != null)
        {
            actionResult(t);
        }
    }

    /// <summary>
    /// 加载外部音频文件，并添加到挂载AudioSource组件的物体上
    /// </summary>
    /// <param name="audioSource">AudioSource组件</param>
    /// <param name="floderName">音频所在的文件夹名称</param>
    /// <param name="audioName">音频名称</param>
    /// <param name="_audioType">音频格式</param>
    /// <returns></returns>
    public IEnumerator LoadExternalAudioWebRequest(AudioSource audioSource, string floderName, string audioName, AudioType _audioType = AudioType.WAV)
    {
        string readPath="";//读取文件的路径
        if (_audioType == AudioType.WAV)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                readPath = "file:///"+ Application.persistentDataPath + "/Custom/" + floderName + "/" + audioName + ".wav";
                Debug.LogError($"安卓端音频读取::{readPath},");
            }
            else
            {
                readPath = Application.streamingAssetsPath + "/Custom/" + floderName + "/" + audioName + ".wav";
                Debug.LogError($"PC端音频读取::{readPath},");
            }
        }else if(_audioType ==AudioType.MPEG)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                readPath = "file:///" + Application.persistentDataPath + "/Custom/" + floderName + "/" + audioName + ".mp3";
                Debug.LogError($"安卓端音频读取::{readPath},");
            }
            else
            {
                readPath = Application.streamingAssetsPath + "/Custom/" + floderName + "/" + audioName + ".mp3";
                Debug.LogError($"PC端音频读取::{readPath},");
            }
        }
        yield return null;
        UnityWebRequest _unityWebRequest = UnityWebRequestMultimedia.GetAudioClip(readPath, _audioType);
        yield return _unityWebRequest.SendWebRequest();

        if (_unityWebRequest.isHttpError || _unityWebRequest.isNetworkError)
        {
            Debug.Log(_unityWebRequest.error.ToString());
        }
        else
        {
            AudioClip _audioClip = DownloadHandlerAudioClip.GetContent(_unityWebRequest);
            audioSource.clip = _audioClip;
            if (FullAudio.clip != null)
            {
                GameResCustomManager.Instance.FullAudio = FullAudio.clip;
                SkiGameManager.Instance.ResCustomManager.enabled = true;
            }
            if (GetupAudio.clip != null)
            {
                GameResCustomManager.Instance.GetupAudio = GetupAudio.clip;
                SkiGameManager.Instance.ResCustomManager.enabled = true;
            }
            if (BacksomersaultAudio.clip != null)
            {
                GameResCustomManager.Instance.BacksomersaultAudio = BacksomersaultAudio.clip;
                SkiGameManager.Instance.ResCustomManager.enabled = true;
            }
            //audioSource.loop = true;
            //audioSource.Play();
        }
    }

    public static UnityEngine.Sprite ToSprite(Texture2D self)
    {
        var rect = new Rect(0, 0, self.width, self.height);
        var pivot = Vector2.one * 0.5f;
        UnityEngine.Sprite newSprite = UnityEngine.Sprite.Create(self, rect, pivot);

        return newSprite;
    }
}
