using System;
using System.Collections.Generic;
using UnityEngine;

public class GUICommentator : MonoBehaviour
{
	public enum Expression
	{
		None = -1,
		Face_Blank_blink = 0,
		Face_Blank_half = 1,
		Face_Blank_wide = 2,
		Face_Grrr_blink = 3,
		Face_Grrr_half = 4,
		Face_Grrr_wide = 5,
		Face_Roar_blink = 6,
		Face_Roar_half = 7,
		Face_Roar_wide = 8,
		Face_Talk_blink = 9,
		Face_Talk_half = 10,
		Face_Talk_wide = 11
	}

	[Serializable]
	public class Message
	{
		public string text;

		public Expression expression;
	}

	public GUITransitionAnimator transitionAnimator;

	public MeshRenderer[] expressionMeshes;

	public Sound[] expressionSounds;

	public GUIDropShadowText messageText;

	public GameObject messageWindow;

	public float messageShowDuration = 3f;

	private Message m_currentMessage;

	private List<Message> m_queuedMessages = new List<Message>();

	private Expression m_lastExpression = Expression.None;

	public void Show()
	{
		transitionAnimator.Show();
		m_currentMessage = null;
	}

	public void Hide()
	{
		m_queuedMessages.Clear();
		ClearMessage();
	}

	public void ShowMessage(Message message)
	{
		if (!transitionAnimator.IsShowing)
		{
			transitionAnimator.Show();
			GUIScore.SupressStyleText = true;
		}
		if (m_currentMessage != null)
		{
			m_queuedMessages.Add(message);
			return;
		}
		if (string.IsNullOrEmpty(message.text))
		{
			messageWindow.SetActive(false);
		}
		else
		{
			messageText.Text = message.text;
			messageWindow.transform.localScale = Vector3.one;
			messageWindow.SetActive(true);
			messageWindow.transform.localScale = Vector3.one;
			Go.killAllTweensWithTarget(messageWindow.transform);
			GoTweenConfig config = new GoTweenConfig().scale(0.2f, true).setEaseType(GoEaseType.ElasticPunch);
			Go.to(messageWindow.transform, 0.5f, config);
		}
		if (message.expression != Expression.None && message.expression != m_lastExpression)
		{
			if (m_lastExpression != Expression.None)
			{
				expressionMeshes[(int)m_lastExpression].enabled = false;
			}
			expressionMeshes[(int)message.expression].enabled = true;
			SoundManager.Instance.PlaySound(expressionSounds[(int)message.expression]);
			m_lastExpression = message.expression;
		}
		m_currentMessage = message;
		CancelInvoke("ClearMessage");
		Invoke("ClearMessage", messageShowDuration);
	}

	public void ClearMessage()
	{
		m_currentMessage = null;
		if (m_queuedMessages.Count > 0)
		{
			Message message = m_queuedMessages[0];
			m_queuedMessages.RemoveAt(0);
			ShowMessage(message);
		}
		else
		{
			messageWindow.SetActive(false);
			transitionAnimator.Hide();
			GUIScore.SupressStyleText = false;
		}
	}
}
