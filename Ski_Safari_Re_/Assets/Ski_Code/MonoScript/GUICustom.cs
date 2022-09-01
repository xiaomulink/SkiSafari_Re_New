using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//==========================
// - ScriptName:      GUICustom.cs         	
// - CreateTime:    #CreateTime#	
// - Homepage:         #AuthorHomepage#		
// - Email:         #AuthorEmail#		
// - Description:   
//==========================

public class GUICustom : MonoBehaviour
{
    public static GUIShop Instance;
    private enum State
    {
        Hidden = 0,
        Opening = 1,
        Browsing = 2,
        Buying = 3,
        Downloading = 4,
        AddingBoosterCount = 5,
        RefreshingBoosterItem = 6,
        ChallengeStarHidingShop = 7,
        ChallengeStarShowingChallenges = 8,
        ChallengeStarAnimatingStar = 9,
        ChallengeStarCompleting = 10,
        WaitingOnTransaction = 11,
        WaitingOnRestore = 12,
        GivingCoins = 13,
        ShowingMessage = 14,
        ShowingPopup = 15
    }

    private State m_state;

    public Action OnHide;
    public Action OnShow;

    private float m_stateTimer;

    public void Show()
    {
        if (m_state == State.Hidden)
        {
            m_state = State.Opening;
            m_stateTimer = 0f;
            AnalyticsManager.Instance.SendEvent("custom_show");
           // buttons.SetActive(true);
            if (OnShow != null)
            {
                OnShow();
            }
        }
    }

    public void Hide()
    {
        if (m_state != 0)
        {
            m_state = State.Hidden;
            AnalyticsManager.Instance.SendEvent("custom_hide");
          
           
            if (OnHide != null)
            {
                OnHide();
            }
        }
    }

}
