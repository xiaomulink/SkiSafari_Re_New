using System;
using UnityEngine;

public class Achievement_LineContact : Achievement_Count
{
	public string lineColliderCategory;

	public string requiredMountCategory;

	public bool allowTakeoff = true;

	public bool resetOnLand;

	public FlamePowerup.Level requiredIgnitionLevel;

	private LineCollider m_lastLineCollider;

	private void OnContact(Player player, GeometryUtils.ContactInfo contactInfo)
	{
		if ((bool)contactInfo.collider && contactInfo.collider != m_lastLineCollider && contactInfo.collider.category == lineColliderCategory && (!(player is PlayerSkierBellyslide) || requiredMountCategory == "bellyslide") && Vector3.Dot(contactInfo.normal, Vector3.up) > 0f && Achievement.CheckMountCategory(player, requiredMountCategory) && player.IgnitionLevel >= requiredIgnitionLevel)
		{
			m_lastLineCollider = contactInfo.collider;
		}
		else
		{
			if (!(contactInfo.collider != m_lastLineCollider))
			{
				return;
			}
			if (resetOnLand)
			{
				if (!persistent)
				{
					ResetCount();
				}
				m_lastLineCollider = null;
			}
			else if ((bool)m_lastLineCollider)
			{
				IncrementCount(1);
				m_lastLineCollider = null;
			}
		}
	}

	private void OnTakeoff(Player player)
	{
		if ((bool)m_lastLineCollider && allowTakeoff && Achievement.CheckMountCategory(player, requiredMountCategory))
		{
			IncrementCount(1);
		}
		m_lastLineCollider = null;
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		Player.OnPlayerContact = (Player.OnCollisionDelegate)Delegate.Combine(Player.OnPlayerContact, new Player.OnCollisionDelegate(OnContact));
		Player.OnPlayerTakeoff = (Player.SimplePlayerDelegate)Delegate.Combine(Player.OnPlayerTakeoff, new Player.SimplePlayerDelegate(OnTakeoff));
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		Player.OnPlayerContact = (Player.OnCollisionDelegate)Delegate.Remove(Player.OnPlayerContact, new Player.OnCollisionDelegate(OnContact));
		Player.OnPlayerTakeoff = (Player.SimplePlayerDelegate)Delegate.Remove(Player.OnPlayerTakeoff, new Player.SimplePlayerDelegate(OnTakeoff));
	}
}
