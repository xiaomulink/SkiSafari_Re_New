using JEngine.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIMultiplayerButton : GUIButton
{
    public GUITransitionAnimator transitionAnimator;

    public Vector3 normalOffset = new Vector3(-1.5f, -1.5f, 0f);

    public Vector3 pulseOffset = new Vector3(-2f, -2f, 0f);

    public GameObject newObject;

    private bool m_canBuyAnyItem;

    private float m_scaleTimer;

    public bool CanBuyAnyItem
    {
        get
        {
            return m_canBuyAnyItem;
        }
    }

    public override void Click(Vector3 position)
    {
        //Deactivate();
        //SkiGameManager.Instance.ShowCustom = true;
        base.Click(position);
        AssetMgr.LoadSceneAsync("Assets/HotUpdateResources/Scene/skiMultiplayer.unity");

    }

    protected override void OnActivate()
    {
        base.OnActivate();
        m_canBuyAnyItem = ItemManager.Instance.CanBuyAnyItem;
        try
        {
            scaleNode.localPosition = ((!m_canBuyAnyItem) ? normalOffset : pulseOffset);
        }
        catch
        {
        }
    }
    private void Update()
    {
    }
}
