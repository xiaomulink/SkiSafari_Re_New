using System.Collections.Generic;

public class AbstractGoTweenCollection : AbstractGoTween
{
	protected class TweenFlowItem
	{
		public float startTime;

		public float duration;

		public AbstractGoTween tween;

		public TweenFlowItem(float startTime, AbstractGoTween tween)
		{
			this.tween = tween;
			this.startTime = startTime;
			duration = tween.totalDuration;
		}

		public TweenFlowItem(float startTime, float duration)
		{
			this.duration = duration;
			this.startTime = startTime;
		}
	}

	protected List<TweenFlowItem> _tweenFlows = new List<TweenFlowItem>();

	public AbstractGoTweenCollection()
	{
		base.timeScale = 1f;
		base.iterations = 1;
		base.state = GoTweenState.Paused;
		Go.addTween(this);
	}

	public List<GoTween> tweensWithTarget(object target)
	{
		List<GoTween> list = new List<GoTween>();
		foreach (TweenFlowItem tweenFlow in _tweenFlows)
		{
			if (tweenFlow.tween == null)
			{
				continue;
			}
			GoTween goTween = tweenFlow.tween as GoTween;
			if (goTween != null && goTween.target == target)
			{
				list.Add(goTween);
			}
			if (goTween != null)
			{
				continue;
			}
			AbstractGoTweenCollection abstractGoTweenCollection = tweenFlow.tween as AbstractGoTweenCollection;
			if (abstractGoTweenCollection != null)
			{
				List<GoTween> list2 = abstractGoTweenCollection.tweensWithTarget(target);
				if (list2.Count > 0)
				{
					list.AddRange(list2);
				}
			}
		}
		return list;
	}

	public override bool removeTweenProperty(AbstractTweenProperty property)
	{
		foreach (TweenFlowItem tweenFlow in _tweenFlows)
		{
			if (tweenFlow.tween == null || !tweenFlow.tween.removeTweenProperty(property))
			{
				continue;
			}
			return true;
		}
		return false;
	}

	public override bool containsTweenProperty(AbstractTweenProperty property)
	{
		foreach (TweenFlowItem tweenFlow in _tweenFlows)
		{
			if (tweenFlow.tween == null || !tweenFlow.tween.containsTweenProperty(property))
			{
				continue;
			}
			return true;
		}
		return false;
	}

	public override List<AbstractTweenProperty> allTweenProperties()
	{
		List<AbstractTweenProperty> list = new List<AbstractTweenProperty>();
		foreach (TweenFlowItem tweenFlow in _tweenFlows)
		{
			if (tweenFlow.tween != null)
			{
				list.AddRange(tweenFlow.tween.allTweenProperties());
			}
		}
		return list;
	}

	public override bool isValid()
	{
		return true;
	}

	public override bool update(float deltaTime)
	{
		base.update(deltaTime);
		float num = ((!_isLoopingBackOnPingPong) ? _elapsedTime : (base.duration - _elapsedTime));
		foreach (TweenFlowItem tweenFlow in _tweenFlows)
		{
			if (tweenFlow.tween != null && tweenFlow.startTime < num)
			{
				float time = num - tweenFlow.startTime;
				tweenFlow.tween.goTo(time);
			}
		}
		if (base.state == GoTweenState.Complete)
		{
			if (!_didComplete)
			{
				onComplete();
			}
			return true;
		}
		return false;
	}

	public override void rewind()
	{
		base.state = GoTweenState.Paused;
		_elapsedTime = (_totalElapsedTime = 0f);
		_isLoopingBackOnPingPong = false;
		_completedIterations = 0;
	}

	public override void complete()
	{
		if (base.iterations < 0)
		{
			return;
		}
		base.complete();
		foreach (TweenFlowItem tweenFlow in _tweenFlows)
		{
			if (tweenFlow.tween != null)
			{
				tweenFlow.tween.goTo(tweenFlow.tween.totalDuration);
			}
		}
	}
}
