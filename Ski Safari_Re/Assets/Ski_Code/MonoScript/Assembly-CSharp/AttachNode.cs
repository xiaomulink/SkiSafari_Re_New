using UnityEngine;

public class AttachNode : MonoBehaviour
{
	public AttachmentGroupMask attachmentGroupMask;

	public Attachment initialAttachment;

	public bool allowOverwrite;

	public bool hidden;

	public string broadcastMessage;

	public string initialAnimation;

	private Attachment m_prefab;

	private Attachment m_attachment;

	public Attachment Attachment
	{
		get
		{
			return m_attachment;
		}
	}

	public bool CanAttach(Attachment attachment)
	{
		if (((uint)attachmentGroupMask & (uint)(1 << (int)attachment.attachmentGroup)) == 0)
		{
			return false;
		}
		return !m_attachment || allowOverwrite || attachment.attachmentGroup > m_attachment.attachmentGroup || attachment.alwaysOverwrite;
	}

	public void Attach(Attachment attachment)
	{
		if ((bool)m_attachment)
		{
			Detach();
		}
		CancelInvoke("Detach");
		m_prefab = attachment;
		m_attachment = TransformUtils.Instantiate(attachment, base.transform, true, true);
		if (hidden)
		{
			m_attachment.gameObject.SetActive(false);
		}
		if (m_attachment.autoDetachDelay > 0f)
		{
			Invoke("Detach", m_attachment.autoDetachDelay);
		}
		if (!string.IsNullOrEmpty(broadcastMessage))
		{
			m_attachment.BroadcastMessage(broadcastMessage, SendMessageOptions.DontRequireReceiver);
		}
		if (!string.IsNullOrEmpty(initialAnimation))
		{
			m_attachment.sprite.PlayAnimation(initialAnimation);
		}
	}

	public void Detach()
	{
		CancelInvoke("Detach");
		if ((bool)m_attachment)
		{
			m_prefab.Detach(base.transform);
			Pool.Despawn(m_attachment.gameObject);
			m_attachment = null;
			m_prefab = null;
		}
	}

	public Attachment Pop()
	{
		CancelInvoke("Detach");
		if ((bool)m_attachment)
		{
			Pool.Despawn(m_attachment.gameObject);
			m_attachment = null;
		}
		return m_prefab;
	}

	private void OnEnable()
	{
		if ((bool)initialAttachment)
		{
			Attach(initialAttachment);
		}
	}

	private void OnDisable()
	{
		if ((bool)m_attachment)
		{
			Pool.Despawn(m_attachment);
			m_attachment = null;
			m_prefab = null;
		}
	}
}
