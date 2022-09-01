using UnityEngine;

public class DebugMenu_TestFlightFeedback : DebugMenu
{
	private string m_feedbackStr = string.Empty;

	private bool m_feedbackSubmitted;

	public override string Name
	{
		get
		{
			return "Feedback";
		}
	}

	public override bool IsAvailable()
	{
		return base.IsAvailable() || base.Active;
	}

	public override void Draw()
	{
		GUILayout.Label("Enter feedback below (tap to start writing):");
		m_feedbackStr = GUILayout.TextArea(m_feedbackStr, GUILayout.Width(300f), GUILayout.Height(200f));
		if (!string.IsNullOrEmpty(m_feedbackStr))
		{
			GUI.color = Color.green;
			if (GUILayout.Button("Submit"))
			{
				m_feedbackStr = string.Empty;
				m_feedbackSubmitted = true;
			}
		}
		else
		{
			GUI.color = Color.grey;
			GUILayout.Button("Submit");
		}
		if (m_feedbackSubmitted)
		{
			if (string.IsNullOrEmpty(m_feedbackStr))
			{
				GUI.color = Color.green;
				GUILayout.Label("Feedback has been submitted, thank you!");
			}
			else
			{
				m_feedbackSubmitted = false;
			}
		}
	}
}
