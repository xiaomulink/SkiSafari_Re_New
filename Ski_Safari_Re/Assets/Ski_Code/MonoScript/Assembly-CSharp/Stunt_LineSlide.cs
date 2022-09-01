using System;
using UnityEngine;

public class Stunt_LineSlide : Stunt
{
	[Serializable]
	public class MountDescriptor
	{
		public string mountCategory;

		public string description = "Slide!";

		public float score = 50f;
	}

	[Serializable]
	public class LineDescriptor
	{
		public string colliderCategory;

		public float minSpeed = 20f;

		public bool allowTakeoff = true;

		public MountDescriptor[] mountDescriptors;

		public MountDescriptor defaultDescriptor;
	}

	public delegate void OnLineSlideDelegate(Player player, LineCollider lineCollider);

	public LineDescriptor[] lineDescriptors;

	public static OnLineSlideDelegate OnLineSlide;

	private LineCollider m_lastLineCollider;

	private LineDescriptor FindLineDescriptor(Player player)
	{
		float magnitude = player.Collider.velocity.magnitude;
		LineDescriptor[] array = lineDescriptors;
		foreach (LineDescriptor lineDescriptor in array)
		{
			if (lineDescriptor.colliderCategory == m_lastLineCollider.category && magnitude >= lineDescriptor.minSpeed)
			{
				return lineDescriptor;
			}
		}
		return null;
	}

	private void LeaveLine(Player player, bool takeoff)
	{
		LineDescriptor lineDescriptor = FindLineDescriptor(player);
		if (lineDescriptor != null && (!takeoff || lineDescriptor.allowTakeoff))
		{
			MountDescriptor mountDescriptor = lineDescriptor.defaultDescriptor;
			PlayerSkierMounted playerSkierMounted = player as PlayerSkierMounted;
			if ((bool)playerSkierMounted)
			{
				MountDescriptor[] mountDescriptors = lineDescriptor.mountDescriptors;
				foreach (MountDescriptor mountDescriptor2 in mountDescriptors)
				{
					if (playerSkierMounted.mountCategory == mountDescriptor2.mountCategory)
					{
						mountDescriptor = mountDescriptor2;
						break;
					}
				}
			}
			base.Manager.AddScore(mountDescriptor.score, mountDescriptor.description);
			if (OnLineSlide != null)
			{
				OnLineSlide(player, m_lastLineCollider);
			}
		}
		m_lastLineCollider = null;
	}

	private void OnContact(Player player, GeometryUtils.ContactInfo contactInfo)
	{
		if ((bool)contactInfo.collider && contactInfo.collider != m_lastLineCollider && !(player is PlayerSkierBellyslide) && Vector3.Dot(contactInfo.normal, Vector3.up) > 0f)
		{
			m_lastLineCollider = contactInfo.collider;
		}
		else if (contactInfo.collider != m_lastLineCollider && (bool)m_lastLineCollider)
		{
			LeaveLine(player, false);
		}
	}

	private void OnTakeoff(Player player)
	{
		if ((bool)m_lastLineCollider)
		{
			LeaveLine(player, true);
		}
	}

	protected override void OnEnable()
	{
		Player.OnPlayerContact = (Player.OnCollisionDelegate)Delegate.Combine(Player.OnPlayerContact, new Player.OnCollisionDelegate(OnContact));
		Player.OnPlayerTakeoff = (Player.SimplePlayerDelegate)Delegate.Combine(Player.OnPlayerTakeoff, new Player.SimplePlayerDelegate(OnTakeoff));
	}

	protected override void OnDisable()
	{
		Player.OnPlayerContact = (Player.OnCollisionDelegate)Delegate.Remove(Player.OnPlayerContact, new Player.OnCollisionDelegate(OnContact));
		Player.OnPlayerTakeoff = (Player.SimplePlayerDelegate)Delegate.Remove(Player.OnPlayerTakeoff, new Player.SimplePlayerDelegate(OnTakeoff));
	}
}
