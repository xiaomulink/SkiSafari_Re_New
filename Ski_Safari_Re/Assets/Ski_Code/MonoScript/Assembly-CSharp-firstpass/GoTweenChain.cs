using UnityEngine;

public class GoTweenChain : AbstractGoTweenCollection
{
	public GoTweenChain setId(int id)
	{
		base.id = id;
		return this;
	}

	public GoTweenChain setIterations(int iterations)
	{
		base.iterations = iterations;
		return this;
	}

	public GoTweenChain setIterations(int iterations, GoLoopType loopType)
	{
		base.iterations = iterations;
		base.loopType = loopType;
		return this;
	}

	public GoTweenChain setUpdateType(GoUpdateType updateType)
	{
		base.updateType = updateType;
		return this;
	}

	private void append(TweenFlowItem item)
	{
		if (item.tween != null && !item.tween.isValid())
		{
			return;
		}
		if (float.IsInfinity(item.duration))
		{
			Debug.Log("adding a Tween with infinite iterations to a TweenChain is not permitted");
			return;
		}
		if (item.tween != null)
		{
			Go.removeTween(item.tween);
		}
		_tweenFlows.Add(item);
		base.duration += item.duration;
		if (base.iterations > 0)
		{
			base.totalDuration = base.duration * (float)base.iterations;
		}
		else
		{
			base.totalDuration = float.PositiveInfinity;
		}
	}

	private void prepend(TweenFlowItem item)
	{
		if (item.tween != null && !item.tween.isValid())
		{
			return;
		}
		if (float.IsInfinity(item.duration))
		{
			Debug.Log("adding a Tween with infinite iterations to a TweenChain is not permitted");
			return;
		}
		if (item.tween != null)
		{
			Go.removeTween(item.tween);
		}
		foreach (TweenFlowItem tweenFlow in _tweenFlows)
		{
			tweenFlow.startTime += item.duration;
		}
		_tweenFlows.Add(item);
		base.duration += item.duration;
		base.totalDuration = base.duration * (float)base.iterations;
	}

	public GoTweenChain append(AbstractGoTween tween)
	{
		TweenFlowItem item = new TweenFlowItem(base.duration, tween);
		append(item);
		return this;
	}

	public GoTweenChain appendDelay(float delay)
	{
		TweenFlowItem item = new TweenFlowItem(base.duration, delay);
		append(item);
		return this;
	}

	public GoTweenChain prepend(AbstractGoTween tween)
	{
		TweenFlowItem item = new TweenFlowItem(0f, tween);
		prepend(item);
		return this;
	}

	public GoTweenChain prependDelay(float delay)
	{
		TweenFlowItem item = new TweenFlowItem(0f, delay);
		prepend(item);
		return this;
	}
}
