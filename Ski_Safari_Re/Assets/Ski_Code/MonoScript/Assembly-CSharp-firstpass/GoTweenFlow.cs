using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GoTweenFlow : AbstractGoTweenCollection
{
	[CompilerGenerated]
	private static Comparison<TweenFlowItem> _003C_003Ef__am_0024cache0;

	public GoTweenFlow setId(int id)
	{
		base.id = id;
		return this;
	}

	public GoTweenFlow setIterations(int iterations)
	{
		base.iterations = iterations;
		return this;
	}

	public GoTweenFlow setIterations(int iterations, GoLoopType loopType)
	{
		base.iterations = iterations;
		base.loopType = loopType;
		return this;
	}

	public GoTweenFlow setUpdateType(GoUpdateType updateType)
	{
		base.updateType = updateType;
		return this;
	}

	private void insert(TweenFlowItem item)
	{
		if (item.tween != null && !item.tween.isValid())
		{
			return;
		}
		if (float.IsInfinity(item.duration))
		{
			Debug.Log("adding a Tween with infinite iterations to a TweenFlow is not permitted");
			return;
		}
		if (item.tween != null)
		{
			Go.removeTween(item.tween);
		}
		_tweenFlows.Add(item);
		List<TweenFlowItem> tweenFlows = _tweenFlows;
		if (_003C_003Ef__am_0024cache0 == null)
		{
			_003C_003Ef__am_0024cache0 = _003Cinsert_003Em__1C;
		}
		tweenFlows.Sort(_003C_003Ef__am_0024cache0);
		base.duration = Mathf.Max(item.startTime + item.duration, base.duration);
		base.totalDuration = base.duration * (float)base.iterations;
	}

	public GoTweenFlow insert(float startTime, AbstractGoTween tween)
	{
		TweenFlowItem item = new TweenFlowItem(startTime, tween);
		insert(item);
		return this;
	}

	[CompilerGenerated]
	private static int _003Cinsert_003Em__1C(TweenFlowItem x, TweenFlowItem y)
	{
		return x.startTime.CompareTo(y.startTime);
	}
}
