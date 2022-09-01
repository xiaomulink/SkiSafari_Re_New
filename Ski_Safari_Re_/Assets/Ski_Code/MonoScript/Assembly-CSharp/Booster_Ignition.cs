public class Booster_Ignition : Booster
{
	public FlamePowerup.Level ignitionLevel = FlamePowerup.Level.SuperBlue;

	public override bool CanUse
	{
		get
		{
			Player instance = Player.Instance;
			if (!instance)
			{
				return false;
			}
			if (base.CurrentCount <= 0)
			{
				return false;
			}
			if (instance.IgnitionLevel >= ignitionLevel)
			{
				return false;
			}
			if (!instance.CanIgniteToLevel(ignitionLevel))
			{
				return false;
			}
			return true;
		}
	}

	protected override void OnUse()
	{
		Player.Instance.Ignite(ignitionLevel);
	}
}
