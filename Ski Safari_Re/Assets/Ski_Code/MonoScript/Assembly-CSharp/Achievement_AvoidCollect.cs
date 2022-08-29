using System;

public class Achievement_AvoidCollect : Achievement_Distance
{
	public string[] collectableCategories;

	private void OnPlayerCollect(Player player, Collectable collectable)
	{
		string[] array = collectableCategories;
		foreach (string text in array)
		{
			if (collectable.category == text)
			{
				ClearDistance();
				break;
			}
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		Collectable.OnPlayerCollect = (Collectable.OnPlayerCollectDelegate)Delegate.Combine(Collectable.OnPlayerCollect, new Collectable.OnPlayerCollectDelegate(OnPlayerCollect));
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		Collectable.OnPlayerCollect = (Collectable.OnPlayerCollectDelegate)Delegate.Remove(Collectable.OnPlayerCollect, new Collectable.OnPlayerCollectDelegate(OnPlayerCollect));
	}
}
