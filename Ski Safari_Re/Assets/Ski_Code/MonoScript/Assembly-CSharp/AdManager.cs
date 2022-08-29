using System;
using System.Diagnostics;
using UnityEngine;

public class AdManager : MonoBehaviour
{
	[Serializable]
	public class AppId
	{
		public string id;

		public string signature;
	}

	private enum InterstitialState
	{
		None = 0,
		TryingToCache = 1,
		Cached = 2,
		Showing = 3,
		Finished = 4
	}

	public static AdManager Instance;

	public float interstitialDelay = 2f;

	public AppId iOSAppId;

	public AppId googlePlayAppId;

	private InterstitialState m_interstitialState;

	public bool ShowInterstitial()
	{
		switch (m_interstitialState)
		{
		case InterstitialState.None:
			m_interstitialState = InterstitialState.TryingToCache;
			return false;
		case InterstitialState.TryingToCache:
			return false;
		case InterstitialState.Cached:
			m_interstitialState = InterstitialState.Showing;
			Invoke("DelayedShowInterstitial", interstitialDelay);
			return true;
		case InterstitialState.Showing:
			return true;
		case InterstitialState.Finished:
			return false;
		default:
			return false;
		}
	}

	private void DelayedShowInterstitial()
	{
	}

	private void DidCacheInterstitial(string location)
	{
		switch (m_interstitialState)
		{
		case InterstitialState.None:
			m_interstitialState = InterstitialState.Cached;
			break;
		case InterstitialState.TryingToCache:
			m_interstitialState = InterstitialState.Cached;
			break;
		}
	}

	private void DidFailToCacheInterstitial(string location, string reason)
	{
		InterstitialState interstitialState = m_interstitialState;
		if (interstitialState != 0 && interstitialState == InterstitialState.TryingToCache)
		{
			m_interstitialState = InterstitialState.None;
		}
	}

	private void DidFinishInterstitial(string location, string reason)
	{
		m_interstitialState = InterstitialState.Finished;
		GUITutorials.Instance.UpdateAutoShow();
		m_interstitialState = InterstitialState.TryingToCache;
	}

	private void Awake()
	{
		Instance = this;
	}

	[Conditional("LOCAL_DEBUG")]
	private static void LocalLog(string log)
	{
	}
}
