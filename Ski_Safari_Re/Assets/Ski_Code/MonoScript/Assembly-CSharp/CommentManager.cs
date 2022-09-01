using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CommentManager : MonoBehaviour
{
	[CompilerGenerated]
	private sealed class _003CLoadComments_003Ec__AnonStorey19
	{
		internal Comment comment;

		internal bool _003C_003Em__D(Comment c)
		{
			return c.name == comment.name;
		}
	}

	public static CommentManager Instance;

	public Comment[] comments;

	public GUICommentator guiCommentatorPrefab;

	public List<Comment> m_activeComments = new List<Comment>();

	private GUICommentator m_guiCommentator;

	public void ShowMessage(GUICommentator.Message message)
	{
		m_guiCommentator.ShowMessage(message);
	}

	public void HideGUI()
	{
		if ((bool)m_guiCommentator)
		{
			m_guiCommentator.Hide();
		}
	}

	public void OnComplete(Comment comment)
	{
		m_activeComments.Remove(comment);
		Object.Destroy(comment.gameObject);
	}

	public void ResetComments()
	{
		Comment[] array = comments;
		foreach (Comment comment in array)
		{
			comment.Reset();
		}
	}

	public void LoadComments()
	{
		_003CLoadComments_003Ec__AnonStorey19 _003CLoadComments_003Ec__AnonStorey = new _003CLoadComments_003Ec__AnonStorey19();
		Comment[] array = comments;
		for (int i = 0; i < array.Length; i++)
		{
			_003CLoadComments_003Ec__AnonStorey.comment = array[i];
			if (_003CLoadComments_003Ec__AnonStorey.comment.CanActivate && m_activeComments.Find(_003CLoadComments_003Ec__AnonStorey._003C_003Em__D) == null)
			{
				Comment comment = TransformUtils.Instantiate(_003CLoadComments_003Ec__AnonStorey.comment, base.transform);
				comment.name = _003CLoadComments_003Ec__AnonStorey.comment.name;
				m_activeComments.Add(comment);
			}
		}
	}

	public void IncrementRuns()
	{
		Comment[] array = comments;
		foreach (Comment comment in array)
		{
			comment.IncrementRuns();
		}
		LoadComments();
	}

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		LoadComments();
		m_guiCommentator = TransformUtils.Instantiate(guiCommentatorPrefab, base.transform);
	}
}
