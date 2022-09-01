using System;
using UnityEngine;

public class Stunt_CeilingTap : Stunt
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

		public MountDescriptor[] mountDescriptors;

		public MountDescriptor defaultDescriptor;
	}

	public LineDescriptor[] lineDescriptors;

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

	private void LeaveLine(Player player)
	{
		LineDescriptor lineDescriptor = FindLineDescriptor(player);
		if (lineDescriptor == null)
		{
			return;
		}
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
			LeaveLine(player);
		}
	}

	private void OnTakeoff(Player player)
	{
		if ((bool)m_lastLineCollider)
		{
			LeaveLine(player);
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
