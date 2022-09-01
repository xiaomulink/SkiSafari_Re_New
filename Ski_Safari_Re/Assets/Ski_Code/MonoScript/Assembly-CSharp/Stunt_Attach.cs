using System;

public class Stunt_Attach : Stunt
{
	[Serializable]
	public class AttachmentDescriptor
	{
		public string attachmentCategory;

		public string description = "Taxi!";

		public float score = 100f;
	}

	public AttachmentDescriptor[] attachmentDescriptors;

	protected void OnAttach(Player player, Attachment attachment)
	{
		AttachmentDescriptor attachmentDescriptor = null;
		AttachmentDescriptor[] array = attachmentDescriptors;
		foreach (AttachmentDescriptor attachmentDescriptor2 in array)
		{
			if (attachment.attachmentCategory == attachmentDescriptor2.attachmentCategory)
			{
				attachmentDescriptor = attachmentDescriptor2;
				break;
			}
		}
		if (attachmentDescriptor != null)
		{
			base.Manager.AddScore(attachmentDescriptor.score, attachmentDescriptor.description);
		}
	}

	protected override void OnEnable()
	{
		Player.OnPlayerAttach = (Player.OnAttachDelegate)Delegate.Combine(Player.OnPlayerAttach, new Player.OnAttachDelegate(OnAttach));
	}

	protected override void OnDisable()
	{
		Player.OnPlayerAttach = (Player.OnAttachDelegate)Delegate.Remove(Player.OnPlayerAttach, new Player.OnAttachDelegate(OnAttach));
	}
}
