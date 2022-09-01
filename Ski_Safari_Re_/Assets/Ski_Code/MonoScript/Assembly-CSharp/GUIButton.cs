using UnityEngine;

public class GUIButton : MonoBehaviour
{
	public delegate void OnClickDelegate();

	public Vector3 center;

	public float radius = 2f;

	public float height;

	public Sound clickSound;

	public bool clickOnEnterPress;

	public bool clickOnBackPress;

	public int focusPriority = -1;

	public Transform scaleNode;

	public Renderer[] pulseRenderersOverride;

	public OnClickDelegate OnClick;

	private bool m_active;

	public virtual void Click(Vector3 position)
	{
        //if (GUITitleSign.titleSign.isshow)
        //{
            SoundManager.Instance.PlaySound(clickSound);
            GameState.UpdateQuiquibi();
            if (OnClick != null)
            {
                OnClick();
            }
        //}
	}

	public virtual void Drag(Vector3 positionDelta)
	{
	}

	public virtual void Release()
	{
	}

	public void Activate()
	{
		if (!m_active)
		{
			m_active = true;
			OnActivate();
		}
	}

	public void Deactivate()
	{
		CancelInvoke();
		if (m_active)
		{
			m_active = false;
			OnDeactivate();
		}
	}

	protected virtual void OnActivate()
	{
		SkiGameManager.Instance.AddActiveButton(this);
	}

	protected virtual void OnDeactivate()
	{
		SkiGameManager.Instance.RemoveActiveButton(this);
	}

	protected virtual void OnEnable()
	{
        if (!gameObject.GetComponent<BoxCollider>())
        {
            gameObject.AddComponent<BoxCollider>();
        }
        if(height==0)
            gameObject.GetComponent<BoxCollider>().size = new Vector3(radius, radius, 1);
        else
            gameObject.GetComponent<BoxCollider>().size = new Vector3(radius, height, 1);
        gameObject.GetComponent<BoxCollider>().center = new Vector3(center.x, center.y, center.z);
        Invoke("Activate", 0f);
	}

	protected virtual void OnDisable()
	{
		Deactivate();
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		if (height > 0f)
		{
			Gizmos.DrawWireCube(base.transform.position + center, new Vector3(radius * 2f, height * 2f));
		}
		else
		{
			Gizmos.DrawWireCube(base.transform.position + center, new Vector3(radius * 2f, radius * 2f));
		}
	}
    public void Awake()
    {
    }
}
