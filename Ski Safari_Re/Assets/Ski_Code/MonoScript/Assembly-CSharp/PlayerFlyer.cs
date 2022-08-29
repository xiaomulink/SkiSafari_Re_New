public class PlayerFlyer : Player
{
	private Flyer m_flyer;

	public override string Category
	{
		get
		{
			return "flier";
		}
	}

	public override CircleCollider Collider
	{
		get
		{
			return m_flyer.Collider;
		}
	}

	public override float LiftInput
	{
		get
		{
			return m_flyer.LiftInput;
		}
		set
		{
			m_flyer.LiftInput = value;
		}
	}

	protected override void Start()
	{
		base.Start();
		m_flyer = GetComponent<Flyer>();
	}
}
