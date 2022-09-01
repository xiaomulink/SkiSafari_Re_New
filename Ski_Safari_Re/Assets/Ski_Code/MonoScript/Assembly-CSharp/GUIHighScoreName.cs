using UnityEngine;

public class GUIHighScoreName : GUIButton
{
	public GUIHighScore highScore;

	public Font font;

	private TouchScreenKeyboard m_keyboard;

	public override void Click(Vector3 position)
	{
		base.Click(position);
		m_keyboard = TouchScreenKeyboard.Open(highScore.PlayerName, TouchScreenKeyboardType.ASCIICapable, false, false, false, false, string.Empty);
	}

	private void Update()
	{
		if (m_keyboard == null)
		{
			return;
		}
		string text = m_keyboard.text;
		bool flag = false;
		int num = 0;
		while (num < text.Length)
		{
			if (!font.HasCharacter(text[num]))
			{
				text = text.Remove(num, 1);
				flag = true;
			}
			else
			{
				num++;
			}
		}
		if (text.Length > highScore.maxPlayerNameLength)
		{
			text = text.Substring(0, highScore.maxPlayerNameLength);
			flag = true;
		}
		if (flag)
		{
			m_keyboard.text = text;
		}
		highScore.PlayerName = text;
		if (m_keyboard.done)
		{
			m_keyboard = null;
		}
	}

	public void Finish()
	{
		if (m_keyboard != null)
		{
			m_keyboard.active = false;
		}
	}

	private void Start()
	{
		if (string.IsNullOrEmpty(highScore.PlayerName))
		{
			Click(Vector3.zero);
		}
	}
}
